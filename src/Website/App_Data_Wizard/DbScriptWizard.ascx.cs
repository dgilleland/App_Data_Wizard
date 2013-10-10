using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Xml;

public partial class App_Data_Wizard_DbScriptWizard : System.Web.UI.UserControl
{
    #region Event Hanlders
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Alert the developer if running in Visual Studio Web Development Server
            var manager = OpenConfigFile();
            WarningMessage.Visible = (manager == null);
            manager = null;

            InstallScripts.Visible = false;
            ReInstallScripts.Visible = false;

            // Show available connection strings
            List<string> connections = new List<string>();
            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
            {
                if ("System.Data.SqlClient".Equals(item.ProviderName))
                    connections.Add(item.Name);
            }
            AvailableConnections.DataSource = connections;
            AvailableConnections.DataBind();

            BindScriptInformation();
            //AppSettingsSection appset = WebConfigurationManager.AppSettings. as AppSettingsSection;
            //if (!appset.IsReadOnly)
            {
                WebConfigurationManager.AppSettings.Set("SqlScriptLastExecuted", "01.00.01.eSchool_data.sql");
                WebConfigurationManager.AppSettings.Set("Bogus", "Added to AppSettings");
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
    }

    protected void ChooseDbConnection_Click(object sender, EventArgs e)
    {
        ConnectionStringName.Value = AvailableConnections.SelectedValue;
        ConnectionStringProper.Text = DbConnectionSettings.ConnectionString;
        InstallScripts.Visible = true;
        ReInstallScripts.Visible = true;
    }

    protected void InstallScripts_Click(object sender, EventArgs e)
    {
        List<string> shortList = FileNames.Where(name => Path.GetFileName(name).CompareTo(SqlScriptLastExecuted) > 0).ToList<string>();
        int count = shortList.Count;
        Install(shortList);

    }

    protected void ReInstallScripts_Click(object sender, EventArgs e)
    {
        Install(FileNames);
    }

    protected void ScriptBlocks_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) 
            if (!(e.Row.Cells[0].Controls[0] as CheckBox).Checked)
                e.Row.BackColor = System.Drawing.Color.LightYellow;
    }
    #endregion

    #region Private Fields and Properties
    //public string LocalWebConfig { get { return Path.Combine(Path.GetDirectoryName(this.AppRelativeVirtualPath), "Web.config"); } }
    public string LocalWebConfig { get { return Path.GetDirectoryName(this.AppRelativeVirtualPath); } }
    private string SqlScriptLastExecuted { get { return ConfigurationManager.AppSettings["SqlScriptLastExecuted"]; } }
    private const string DefaultScriptFolder = @"~\App_Data\";
    private string SqlScriptFolder { get { return string.IsNullOrEmpty(ConfigurationManager.AppSettings["SqlScriptFolder"]) ? DefaultScriptFolder : ConfigurationManager.AppSettings["SqlScriptFolder"]; } }
    private string SqlScriptNamingPattern { get { return ConfigurationManager.AppSettings["SqlScriptNamingPattern"]; } }
    private Regex FilePattern { get { return new Regex(string.IsNullOrEmpty(SqlScriptNamingPattern) ? @".*.sql" : SqlScriptNamingPattern, RegexOptions.IgnoreCase); } }
    private List<string> FileNames { get { return Directory.GetFiles(Server.MapPath(SqlScriptFolder)).Where(file => FilePattern.IsMatch(file)).ToList<string>(); } }

    // Following line adapted from the DotNetNuke.Data.SqlDataProvider SqlDelimiterRegex property
    private static Regex SqlDelimiterRegex = new Regex(@"(?<=(?:[^\w]+|^))GO(?=(?: |\t)*?(?:\r?\n|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private ConnectionStringSettings DbConnectionSettings { get { return ConfigurationManager.ConnectionStrings[ConnectionStringName.Value]; } }
    #endregion

    #region Protected Methods
    protected string HtmlDecode(object text)
    {
        if (text == null)
            text = "";
        return Server.HtmlDecode(text.ToString());
    }
    #endregion

    #region Private Methods (general)
    /// <summary>
    /// Display information about the scripts available and applied.
    /// </summary>
    private void BindScriptInformation()
    {
        // Last run script
        LastRunScript.Text = string.IsNullOrEmpty(SqlScriptLastExecuted) ? "-no scripts run-" : SqlScriptLastExecuted;


        // Available Script Files
        var files = from item in FileNames
                    select new
                    {
                        FileName = Path.GetFileName(item),
                        Installed = Path.GetFileName(item).CompareTo(SqlScriptLastExecuted) <= 0
                    };
        ScriptFileGridView.EmptyDataText = "No SQL files available in " + SqlScriptFolder;
        ScriptFileGridView.DataSource = files; //.OrderBy(each => each.FileName);
        ScriptFileGridView.DataBind();
    }
    #endregion

    #region Private Methods (Script Installation)
    /// <summary>
    /// Processes a collection of script files to be run on the database.
    /// </summary>
    /// <param name="scripts">A collection of complete file paths to SQL script files.</param>
    private void Install(IList<string> scripts)
    {
        try
        {
            DatabaseRebuildPanel.Visible = true;
            List<ScriptFile> allScripts = new List<ScriptFile>();
            if (scripts.Count > 0)
            {
                foreach (string script in scripts)
                    allScripts.Add(new ScriptFile() { FilePath = script, ScriptBlocks = RunInstallationScript(script) });

                SaveAppSetting("SqlScriptLastExecuted", Path.GetFileName(scripts.Last<string>()));

                BindScriptInformation();
            }

            int errorCount = 0;
            foreach (ScriptFile script in allScripts)
                foreach (var executedBlock in script.ScriptBlocks)
                    if (!executedBlock.Installed)
                        errorCount++;

            if (errorCount > 0)
                MessageLabel.Text = String.Format("The scripts executed with {0} errors. See the table below for details.", errorCount);

            ScriptsRunGridview.DataSource = allScripts;
            ScriptsRunGridview.DataBind();
        }
        catch (Exception ex)
        {
            DatabaseRebuildPanel.Visible = false;
            MessageLabel.Text = String.Format("Serious error: cannot continue. <blockquote>{0}<blockquote>{1}</blockquote></blockquote>", ex.Message, ex.StackTrace);
        }
    }

    /// <summary>
    /// Opens the file at the provided scriptPath and splits it into individual blocks of SQL commands to be run on the database.
    /// </summary>
    /// <param name="scriptPath">The complete file path of the SQL script file.</param>
    private List<RunnableScript> RunInstallationScript(string scriptPath)
    {
        string script = File.ReadAllText(scriptPath);
        string[] scripts = SqlDelimiterRegex.Split(script);
        List<RunnableScript> results = new List<RunnableScript>();
        foreach (string item in scripts)
        {
            RunnableScript thisScript = new RunnableScript();
            thisScript.SQLScript = item.Replace("\r\n", "<br/>");
            try
            {
                ExecuteNonQuery(CommandType.Text, item);
                thisScript.Installed = true;
            }
            catch (Exception ex)
            {
                thisScript.Installed = false;
                thisScript.FailureMessage = ex.Message.Replace("\r\n", "<br/>");
            }
            results.Add(thisScript);
        }
        return results;
    }

    /// <summary>
    /// Executes an individual SQL command text against the database.
    /// </summary>
    /// <param name="commandType">The type of command to execute (Text, StoredProcedure, or TableDirect)</param>
    /// <param name="text">The SQL script to execute.</param>
    private void ExecuteNonQuery(CommandType commandType, string text)
    {
        if (String.IsNullOrEmpty(text))
            throw new ArgumentException("SQL text is null or empty.", "text");

        // 0) Setup for generic ADO.NET
        var databaseFactory = DbProviderFactories.GetFactory(DbConnectionSettings.ProviderName);

        // 1) Create the Connection Object
        DbConnection conn = databaseFactory.CreateConnection();
        conn.ConnectionString = DbConnectionSettings.ConnectionString;

        // 2) Create the Command Object
        DbCommand cmd = databaseFactory.CreateCommand();
        cmd.Connection = conn;
        cmd.CommandType = commandType;
        cmd.CommandText = text;

        // 3) Open the connection to get & display the data
        using (cmd.Connection)
        {
            cmd.Connection.Open();

            IDataReader reader = cmd.ExecuteReader();
            cmd.Connection.Close();
        }
    }
    
    #region Private Methods - Update Configuration File
    private void SaveAppSetting(string key, string value)
    {
        Configuration manager = OpenConfigFile();
        if (manager != null)
        {
            // NOTE: Most likey scenario in a production environment
            if (!string.IsNullOrEmpty(manager.AppSettings.Settings[key].Value))
                manager.AppSettings.Settings.Remove(key);
            manager.AppSettings.Settings.Add(key, value);
            manager.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        else
        {
            // NOTE: Alternate scenario when settings cannot be saved through a ConfigurationManager, particulary when running under the Visual Studio Web Development Server (as opposed to IIS or IIS Express).
            ConfigurationEditor editor = new ConfigurationEditor(Path.Combine(Server.MapPath(LocalWebConfig), "Web.config"));
            if (editor.KeyExists(key))
                editor.UpdateKey(key, value);
            else
                editor.AddKey(key, value);
            editor.SaveDoc();
        }
    }

    private Configuration OpenConfigFile()
    {
        try
        {
            return WebConfigurationManager.OpenWebConfiguration(LocalWebConfig);
        }
        catch (Exception)
        {
            return null;
        }
    }
    #endregion
    #endregion

    #region Inner Classes
    public class ScriptFile
    {
        public List<RunnableScript> ScriptBlocks { get; set; }
        public string FilePath { get; set; }
        public string FileName { get { return Path.GetFileName(FilePath); } }
    }

    public class RunnableScript
    {
        public string SQLScript { get; set; }
        public bool Installed { get; set; }
        public string FailureMessage { get; set; }
    }

    private class ConfigurationEditor : IDisposable
    {
        public string WebConfigPath { get; private set; }
        private XmlDocument xmlDoc;

        /// <summary>
        /// Create an instance of the ConfigurationEditor to manipulate XML config files.
        /// </summary>
        /// <param name="physicalWebConfigPath">The physical path to the configuration file.</param>
        public ConfigurationEditor(string physicalWebConfigPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(physicalWebConfigPath);
            WebConfigPath = physicalWebConfigPath;
        }

        public void SaveDoc()
        {
            xmlDoc.Save(WebConfigPath);
        }

        #region CodeProject sample code
        // The methods in this region are adapted from the CodeProject article "Modifying Configuration Settings at Runtime"
        // http://www.codeproject.com/Articles/12589/Modifying-Configuration-Settings-at-Runtime

        // Adds a key and value to the App.config
        public void AddKey(string strKey, string strValue)
        {
            XmlNode appSettingsNode =
              xmlDoc.SelectSingleNode("configuration/appSettings");
            try
            {
                if (KeyExists(strKey))
                    throw new ArgumentException("Key name: <" + strKey +
                              "> already exists in the configuration.");
                XmlNode newChild = appSettingsNode.FirstChild.Clone();
                newChild.Attributes["key"].Value = strKey;
                newChild.Attributes["value"].Value = strValue;
                appSettingsNode.AppendChild(newChild);

                // CLEAN:
                //We have to save the configuration in two places, 
                //because while we have a root App.config,
                //we also have an ApplicationName.exe.config.
                //xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory +
                //                             "..\\..\\App.config");
                //xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Updates a key within the App.config
        public void UpdateKey(string strKey, string newValue)
        {
            if (!KeyExists(strKey))
                throw new ArgumentNullException("Key", "<" + strKey +
                      "> does not exist in the configuration. Update failed.");
            XmlNode appSettingsNode =
               xmlDoc.SelectSingleNode("configuration/appSettings");
            // Attempt to locate the requested setting.
            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    childNode.Attributes["value"].Value = newValue;
            }
            // CLEAN:
            //xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory +
            //                             "..\\..\\App.config");
            //xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        // Deletes a key from the App.config
        public void DeleteKey(string strKey)
        {
            if (!KeyExists(strKey))
                throw new ArgumentNullException("Key", "<" + strKey +
                      "> does not exist in the configuration. Update failed.");
            XmlNode appSettingsNode =
               xmlDoc.SelectSingleNode("configuration/appSettings");
            // Attempt to locate the requested setting.
            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    appSettingsNode.RemoveChild(childNode);
            }
            // CLEAN:
            //xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");
            //xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        // Determines if a key exists within the App.config
        public bool KeyExists(string strKey)
        {
            XmlNode appSettingsNode =
              xmlDoc.SelectSingleNode("configuration/appSettings");
            // Attempt to locate the requested setting.
            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    return true;
            }
            return false;
        }
        #endregion

        public void Dispose()
        {
            xmlDoc = null;
        }
    }
    #endregion
}