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

public partial class App_Data_Wizard_DbScriptWizard : System.Web.UI.UserControl
{
    #region Public Properties
    public string ConnectionStringName { get; set; }
    #endregion

    #region Event Hanlders
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Show available connection strings
            List<string> connections = new List<string>();
            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
            {
                if ("System.Data.SqlClient".Equals(item.ProviderName))
                    connections.Add(item.Name);
            }
            AvailableConnections.DataSource = connections;
            AvailableConnections.DataBind();

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
    }

    protected void ChooseDbConnection_Click(object sender, EventArgs e)
    {
        ConnectionStringName = AvailableConnections.SelectedValue;
        ConnectionStringProper.Text = DbConnectionSettings.ConnectionString;
    }

    protected void InstallScripts_Click(object sender, EventArgs e)
    {
        Install(FileNames.Where(name => name.CompareTo(SqlScriptLastExecuted) > 0));

    }

    protected void ReInstallScripts_Click(object sender, EventArgs e)
    {
        Install(FileNames);
    }

    protected void ScriptsRunGridview_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Text = Server.HtmlDecode(e.Row.Cells[0].Text);
            e.Row.Cells[2].Text = Server.HtmlDecode(e.Row.Cells[2].Text);
        }
    }
    #endregion

    #region Private Fields and Properties
    public string LocalWebConfig { get { return Path.Combine(Path.GetDirectoryName(this.AppRelativeVirtualPath), "Web.config"); } }
    private string SqlScriptLastExecuted { get { return ConfigurationManager.AppSettings["SqlScriptLastExecuted"]; } }
    private const string DefaultScriptFolder = @"~\App_Data\";
    private string SqlScriptFolder { get { return string.IsNullOrEmpty(ConfigurationManager.AppSettings["SqlScriptFolder"]) ? DefaultScriptFolder : ConfigurationManager.AppSettings["SqlScriptFolder"]; } }
    private string SqlScriptNamingPattern { get { return ConfigurationManager.AppSettings["SqlScriptNamingPattern"]; } }
    private Regex FilePattern { get { return new Regex(string.IsNullOrEmpty(SqlScriptNamingPattern) ? @".*.sql" : SqlScriptNamingPattern, RegexOptions.IgnoreCase); } }
    private List<string> FileNames { get { return Directory.GetFiles(Server.MapPath(SqlScriptFolder)).Where(file => FilePattern.IsMatch(file)).ToList<string>(); } }

    // Following line adapted from the DotNetNuke.Data.SqlDataProvider SqlDelimiterRegex property
    private static Regex SqlDelimiterRegex = new Regex(@"(?<=(?:[^\w]+|^))GO(?=(?: |\t)*?(?:\r?\n|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private ConnectionStringSettings DbConnectionSettings { get { return ConfigurationManager.ConnectionStrings[ConnectionStringName]; } }
    #endregion

    #region Private Methods (Script Installation)
    /// <summary>
    /// Processes a collection of script files to be run on the database.
    /// </summary>
    /// <param name="scripts">A collection of complete file paths to SQL script files.</param>
    private void Install(IEnumerable<string> scripts)
    {
        try
        {
            DatabaseRebuildPanel.Visible = true;
            List<ScriptFile> allScripts = new List<ScriptFile>();
            foreach (string script in scripts)
                allScripts.Add(new ScriptFile() { FilePath = script, ScriptBlocks = RunInstallationScript(script) });

            // TODO: Use the approach in these two spots rather than the commented out block:
            // https://raw.github.com/dnnsoftware/Dnn.Platform/b3752ac3e19b911f94e1c01e6bad07932ecbbbf2/DNN%20Platform/Library/Common/Utilities/Config.cs
            // http://www.codeproject.com/Articles/12589/Modifying-Configuration-Settings-at-Runtime
            /*
                // http://csharpdotnetfreak.blogspot.com/2011/12/write-modify-webconfig-programmatically.html
                var rewriter = WebConfigurationManager.OpenWebConfiguration(LocalWebConfig);
                rewriter.AppSettings.Settings.Add("SqlScriptLastExecuted", files.Last().FileName); //scripts.Last<string>());
                rewriter.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
             */
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
    #endregion
}