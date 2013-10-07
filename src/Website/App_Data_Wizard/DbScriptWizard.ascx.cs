using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class App_Data_Wizard_DbScriptWizard : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            List<string> connections = new List<string>();
            foreach(ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
            {
                if ("System.Data.SqlClient".Equals(item.ProviderName))
                    connections.Add(item.Name);
            }
            AvailableConnections.DataSource = connections;
            AvailableConnections.DataBind();
        }
    }

    #region Private Fields
    // Following line adapted from the DotNetNuke.Data.SqlDataProvider SqlDelimiterRegex property
    private static Regex SqlDelimiterRegex = new Regex(@"(?<=(?:[^\w]+|^))GO(?=(?: |\t)*?(?:\r?\n|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /*
        <add key="SqlScriptFolder"
             value="~/App_Data/Scripts/Sql/"/>
        <!--Script Naming Pattern. E.g.: 01.00.00.school.sql-->
        <add key="SqlScriptNamingPattern"
             value="\d{1,2}\.\d{1,2}\.\d{1,2}\..*.sql"/>
        <add key="SqlScriptLastExecuted"
             value=""/>
         */
    private string SqlScriptFolder { get { return ConfigurationManager.AppSettings["SqlScriptFolder"]; } }
    private string SqlScriptNamingPattern { get { return ConfigurationManager.AppSettings["SqlScriptNamingPattern"]; } }
    private string SqlScriptLastExecuted { get { return ConfigurationManager.AppSettings["SqlScriptLastExecuted"]; } }
    //private Database _DataStore = null;
    //public Database DataStore
    //{
    //    get
    //    {
    //        if (_DataStore == null)
    //            try
    //            {
    //                _DataStore = DatabaseFactory.CreateDatabase(ConnectionStringName.Text);
    //            }
    //            catch (ConfigurationErrorsException ex)
    //            {
    //                ConnectionStringDetails.Text = "Missing: " + ex.Message;
    //                ConnectionStringDetails.Text += "<blockquote>(Falling back to hard-coded connection string details)</blockquote>";
    //                try
    //                {
    //                    _DataStore = new SqlDatabase(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|OMST_Database.mdf;Integrated Security=True;User Instance=True");
    //                }
    //                catch (SqlException ex2)
    //                {
    //                    ConnectionStringDetails.Text += "<p>Fallback connection failed: " + ex2.Message + "</p>";
    //                }
    //            }               
    //        return _DataStore;
    //    }
    //}
    //public string ConnectionString
    //{
    //    get
    //    {
    //        return DataStore.ConnectionString;
    //    }
    //}
    #endregion

    #region Event Handlers
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    if (!Page.IsPostBack)
    //    {
    //        try
    //        {
    //            ConnectionStringDetails.Text = ConnectionString;
    //            MessageLabel.Text = "";
    //            using (IDataReader reader = DataStore.ExecuteReader(CommandType.Text, "SELECT * FROM Movie"))
    //            {
    //                MessageLabel.Text = "Able to read from Movie table.<br/>";
    //            }
    //            using (IDataReader reader = DataStore.ExecuteReader(CommandType.Text, "SELECT * FROM Purchase"))
    //            {
    //                MessageLabel.Text += "Able to read from Purchase table.<br/>";
    //            }
    //            using (IDataReader reader = DataStore.ExecuteReader(CommandType.Text, "SELECT * FROM ShowTime"))
    //            {
    //                MessageLabel.Text += "Able to read from ShowTime table.<br/>";
    //            }
    //            NotAccessible.Visible = false;
    //            DatabaseRebuildPanel.Visible = false;
    //        }
    //        catch (SqlException ex)
    //        {
    //            NotAccessible.Visible = true;
    //            DatabaseRebuildPanel.Visible = true;
    //            RunInstallationScript();
    //        }
    //        catch (Exception ex)
    //        {
    //            NotAccessible.Visible = true;
    //            DatabaseRebuildPanel.Visible = false;
    //            MessageLabel.Text = "Serious error: cannot continue. <blockquote>" + ex.Message + "<blockquote>" + ex.StackTrace + "</blockquote></blockquote>";
    //        }
    //    }
    //}
    #endregion

    #region Private Methods (Script Installation)
    //private void RunInstallationScript()
    //{
    //    try
    //    {
    //        string scriptPath = Server.MapPath("App_Data/OMST_Database_Install.sql");
    //        string script = File.ReadAllText(scriptPath);
    //        string[] scripts = SqlDelimiterRegex.Split(script);
    //        List<RunnableScript> results = new List<RunnableScript>();
    //        foreach (string item in scripts)
    //        {
    //            RunnableScript thisScript = new RunnableScript();
    //            thisScript.SQLScript = item.Replace("\r\n", "<br/>");
    //            try
    //            {
    //                DataStore.ExecuteNonQuery(CommandType.Text, item);
    //                thisScript.Installed = true;
    //            }
    //            catch (Exception ex)
    //            {
    //                thisScript.Installed = false;
    //                thisScript.FailureMessage = ex.Message.Replace("\r\n", "<br/>");
    //            }
    //            results.Add(thisScript);
    //        }
    //        ScriptsRunGridview.DataSource = results;
    //        ScriptsRunGridview.DataBind();
    //        MessageLabel.Text = "Script to create database items has completed successfully.";
    //    }
    //    catch (SqlException ex)
    //    {
    //        MessageLabel.Text = ex.Message + "<br/><blockquote><h5>Stack Trace</h5>" + ex.StackTrace + "</blockquote>";
    //    }
    //}
    #endregion

    protected void ScriptsRunGridview_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Text = Server.HtmlDecode(e.Row.Cells[0].Text);
            e.Row.Cells[2].Text = Server.HtmlDecode(e.Row.Cells[2].Text);
        }
    }

    #region Inner Classes
    public class RunnableScript
    {
        public string SQLScript { get; set; }
        public bool Installed { get; set; }
        public string FailureMessage { get; set; }
    }

    #region Copyright
    // 
    // DotNetNuke® - http://www.dotnetnuke.com
    // Copyright (c) 2002-2013
    // by DotNetNuke Corporation
    // 
    // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
    // documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
    // the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
    // to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in all copies or substantial portions 
    // of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
    // TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
    // THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
    // CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
    // DEALINGS IN THE SOFTWARE.
    #endregion

    //public sealed class SqlDataProvider
    //{
    //    #region Private Members
    //    private const string _scriptDelimiterRegex = "(?<=(?:[^\\w]+|^))GO(?=(?: |\\t)*?(?:\\r?\\n|$))";

    //    private static readonly Regex ScriptWithRegex = new Regex("WITH\\s*\\([\\s\\S]*?((PAD_INDEX|ALLOW_ROW_LOCKS|ALLOW_PAGE_LOCKS)\\s*=\\s*(ON|OFF))+[\\s\\S]*?\\)",
    //                                                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
    //    private static readonly Regex ScriptOnPrimaryRegex = new Regex("(TEXTIMAGE_)*ON\\s*\\[\\s*PRIMARY\\s*\\]", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    //    #endregion

    //    #region Public Properties
    //    public string ConnectionString { get; set; }
    //    public override bool IsConnectionValid
    //    {
    //        get
    //        {
    //            return CanConnect(ConnectionString);
    //        }
    //    }

    //    #endregion

    //    #region Private Methods

    //    private static bool CanConnect(string connectionString)
    //    {
    //        bool connectionValid = true;

    //        try
    //        {
    //            PetaPocoHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, "GetDatabaseVersion");
    //        }
    //        catch (SqlException ex)
    //        {
    //            if (ex.Errors.Cast<SqlError>().Any(c => !(c.Number == 2812 && c.Class == 16)))
    //            {
    //                connectionValid = false;
    //            }
    //        }

    //        return connectionValid;
    //    }

    //    private string ExecuteScriptInternal(string connectionString, string script)
    //    {
    //        string exceptions = "";

    //        string[] sqlStatements = SqlDelimiterRegex.Split(script);
    //        foreach (string statement in sqlStatements)
    //        {
    //            var sql = statement.Trim();
    //            if (!String.IsNullOrEmpty(sql))
    //            {
    //                //Clean up some SQL Azure incompatabilities
    //                var query = GetAzureCompactScript(sql);

    //                if (query != sql)
    //                {
    //                    var props = new LogProperties { new LogDetailInfo("SQL Script Modified", query) };

    //                    var elc = new EventLogController();
    //                    elc.AddLog(props,
    //                                PortalController.GetCurrentPortalSettings(),
    //                                UserController.GetCurrentUserInfo().UserID,
    //                                EventLogController.EventLogType.HOST_ALERT.ToString(),
    //                                true);
    //                }

    //                try
    //                {
    //                    Logger.Trace("Executing SQL Script " + query);

    //                    //Create a new connection
    //                    var connection = new SqlConnection(connectionString);
    //                    //Create a new command (with no timeout)
    //                    var command = new SqlCommand(query, connection) { CommandTimeout = 0 };

    //                    connection.Open();
    //                    command.ExecuteNonQuery();
    //                    connection.Close();

    //                }
    //                catch (SqlException objException)
    //                {
    //                    Logger.Debug(objException);

    //                    exceptions += objException + Environment.NewLine + Environment.NewLine + query + Environment.NewLine + Environment.NewLine;
    //                }
    //            }
    //        }

    //        return exceptions;
    //    }

    //    private IDataReader ExecuteSQLInternal(string connectionString, string sql)
    //    {
    //        try
    //        {
    //            sql = DataUtil.ReplaceTokens(sql);
    //            return PetaPocoHelper.ExecuteReader(connectionString, CommandType.Text, sql);
    //        }
    //        catch
    //        {
    //            //error in SQL query
    //            return null;
    //        }
    //    }

    //    private string GetConnectionStringUserID()
    //    {
    //        string DBUser = "public";

    //        //If connection string does not use integrated security, then get user id.
    //        if (ConnectionString.ToUpper().Contains("USER ID") || ConnectionString.ToUpper().Contains("UID") || ConnectionString.ToUpper().Contains("USER"))
    //        {
    //            string[] ConnSettings = ConnectionString.Split(';');

    //            foreach (string s in ConnSettings)
    //            {
    //                if (s != string.Empty)
    //                {
    //                    string[] ConnSetting = s.Split('=');
    //                    if ("USER ID|UID|USER".Contains(ConnSetting[0].Trim().ToUpper()))
    //                    {
    //                        DBUser = ConnSetting[1].Trim();
    //                    }
    //                }
    //            }
    //        }
    //        return DBUser;
    //    }

    //    private string GrantStoredProceduresPermission(string Permission, string LoginOrRole)
    //    {
    //        string SQL = string.Empty;
    //        string Exceptions = string.Empty;

    //        try
    //        {
    //            //grant rights to a login or role for all stored procedures
    //            SQL += "if exists (select * from dbo.sysusers where name='" + LoginOrRole + "')";
    //            SQL += "  begin";
    //            SQL += "    declare @exec nvarchar(2000) ";
    //            SQL += "    declare @name varchar(150) ";
    //            SQL += "    declare sp_cursor cursor for select o.name as name ";
    //            SQL += "    from dbo.sysobjects o ";
    //            SQL += "    where ( OBJECTPROPERTY(o.id, N'IsProcedure') = 1 or OBJECTPROPERTY(o.id, N'IsExtendedProc') = 1 or OBJECTPROPERTY(o.id, N'IsReplProc') = 1 ) ";
    //            SQL += "    and OBJECTPROPERTY(o.id, N'IsMSShipped') = 0 ";
    //            SQL += "    and o.name not like N'#%%' ";
    //            SQL += "    and (left(o.name,len('" + ObjectQualifier + "')) = '" + ObjectQualifier + "' or left(o.name,7) = 'aspnet_') ";
    //            SQL += "    open sp_cursor ";
    //            SQL += "    fetch sp_cursor into @name ";
    //            SQL += "    while @@fetch_status >= 0 ";
    //            SQL += "      begin";
    //            SQL += "        select @exec = 'grant " + Permission + " on [' +  @name  + '] to [" + LoginOrRole + "]'";
    //            SQL += "        execute (@exec)";
    //            SQL += "        fetch sp_cursor into @name ";
    //            SQL += "      end ";
    //            SQL += "    deallocate sp_cursor";
    //            SQL += "  end ";

    //            SqlHelper.ExecuteNonQuery(UpgradeConnectionString, CommandType.Text, SQL);
    //        }
    //        catch (SqlException objException)
    //        {
    //            Logger.Debug(objException);

    //            Exceptions += objException + Environment.NewLine + Environment.NewLine + SQL + Environment.NewLine + Environment.NewLine;
    //        }
    //        return Exceptions;
    //    }

    //    private string GrantUserDefinedFunctionsPermission(string ScalarPermission, string TablePermission, string LoginOrRole)
    //    {
    //        string SQL = string.Empty;
    //        string Exceptions = string.Empty;
    //        try
    //        {
    //            //grant EXECUTE rights to a login or role for all functions
    //            SQL += "if exists (select * from dbo.sysusers where name='" + LoginOrRole + "')";
    //            SQL += "  begin";
    //            SQL += "    declare @exec nvarchar(2000) ";
    //            SQL += "    declare @name varchar(150) ";
    //            SQL += "    declare @isscalarfunction int ";
    //            SQL += "    declare @istablefunction int ";
    //            SQL += "    declare sp_cursor cursor for select o.name as name, OBJECTPROPERTY(o.id, N'IsScalarFunction') as IsScalarFunction ";
    //            SQL += "    from dbo.sysobjects o ";
    //            SQL += "    where ( OBJECTPROPERTY(o.id, N'IsScalarFunction') = 1 OR OBJECTPROPERTY(o.id, N'IsTableFunction') = 1 ) ";
    //            SQL += "      and OBJECTPROPERTY(o.id, N'IsMSShipped') = 0 ";
    //            SQL += "      and o.name not like N'#%%' ";
    //            SQL += "      and (left(o.name,len('" + ObjectQualifier + "')) = '" + ObjectQualifier + "' or left(o.name,7) = 'aspnet_') ";
    //            SQL += "    open sp_cursor ";
    //            SQL += "    fetch sp_cursor into @name, @isscalarfunction ";
    //            SQL += "    while @@fetch_status >= 0 ";
    //            SQL += "      begin ";
    //            SQL += "        if @IsScalarFunction = 1 ";
    //            SQL += "          begin";
    //            SQL += "            select @exec = 'grant " + ScalarPermission + " on [' +  @name  + '] to [" + LoginOrRole + "]'";
    //            SQL += "            execute (@exec)";
    //            SQL += "            fetch sp_cursor into @name, @isscalarfunction  ";
    //            SQL += "          end ";
    //            SQL += "        else ";
    //            SQL += "          begin";
    //            SQL += "            select @exec = 'grant " + TablePermission + " on [' +  @name  + '] to [" + LoginOrRole + "]'";
    //            SQL += "            execute (@exec)";
    //            SQL += "            fetch sp_cursor into @name, @isscalarfunction  ";
    //            SQL += "          end ";
    //            SQL += "      end ";
    //            SQL += "    deallocate sp_cursor";
    //            SQL += "  end ";

    //            SqlHelper.ExecuteNonQuery(UpgradeConnectionString, CommandType.Text, SQL);
    //        }
    //        catch (SqlException objException)
    //        {
    //            Logger.Debug(objException);

    //            Exceptions += objException + Environment.NewLine + Environment.NewLine + SQL + Environment.NewLine + Environment.NewLine;
    //        }
    //        return Exceptions;
    //    }

    //    private string GetAzureCompactScript(string script)
    //    {
    //        if (ScriptWithRegex.IsMatch(script))
    //        {
    //            script = ScriptWithRegex.Replace(script, string.Empty);
    //        }

    //        if (ScriptOnPrimaryRegex.IsMatch(script))
    //        {
    //            script = ScriptOnPrimaryRegex.Replace(script, string.Empty);
    //        }

    //        return script;
    //    }

    //    private Regex SqlDelimiterRegex
    //    {
    //        get
    //        {
    //            var objRegex = (Regex)DataCache.GetCache("SQLDelimiterRegex");
    //            if (objRegex == null)
    //            {
    //                objRegex = new Regex(_scriptDelimiterRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
    //                DataCache.SetCache("SQLDelimiterRegex", objRegex);
    //            }
    //            return objRegex;
    //        }
    //    }

    //    #endregion

    //    #region Abstract Methods

    //    public override void ExecuteNonQuery(string procedureName, params object[] commandParameters)
    //    {
    //        PetaPocoHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, DatabaseOwner + ObjectQualifier + procedureName, commandParameters);
    //    }

    //    public override IDataReader ExecuteReader(string procedureName, params object[] commandParameters)
    //    {
    //        return PetaPocoHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, DatabaseOwner + ObjectQualifier + procedureName, commandParameters);
    //    }

    //    public override T ExecuteScalar<T>(string procedureName, params object[] commandParameters)
    //    {
    //        return PetaPocoHelper.ExecuteScalar<T>(ConnectionString, CommandType.StoredProcedure, DatabaseOwner + ObjectQualifier + procedureName, commandParameters);
    //    }

    //    public override string ExecuteScript(string script)
    //    {
    //        string exceptions = ExecuteScriptInternal(UpgradeConnectionString, script);

    //        //if the upgrade connection string is specified or or db_owner setting is not set to dbo
    //        if (UpgradeConnectionString != ConnectionString || DatabaseOwner.Trim().ToLower() != "dbo.")
    //        {
    //            try
    //            {
    //                //grant execute rights to the public role or userid for all stored procedures. This is
    //                //necesary because the UpgradeConnectionString will create stored procedures
    //                //which restrict execute permissions for the ConnectionString user account. This is also
    //                //necessary when db_owner is not set to "dbo" 
    //                exceptions += GrantStoredProceduresPermission("EXECUTE", GetConnectionStringUserID());
    //            }
    //            catch (SqlException objException)
    //            {
    //                Logger.Debug(objException);

    //                exceptions += objException + Environment.NewLine + Environment.NewLine + script + Environment.NewLine + Environment.NewLine;
    //            }

    //            try
    //            {
    //                //grant execute or select rights to the public role or userid for all user defined functions based
    //                //on what type of function it is (scalar function or table function). This is
    //                //necesary because the UpgradeConnectionString will create user defined functions
    //                //which restrict execute permissions for the ConnectionString user account.  This is also
    //                //necessary when db_owner is not set to "dbo" 
    //                exceptions += GrantUserDefinedFunctionsPermission("EXECUTE", "SELECT", GetConnectionStringUserID());
    //            }
    //            catch (SqlException objException)
    //            {
    //                Logger.Debug(objException);

    //                exceptions += objException + Environment.NewLine + Environment.NewLine + script + Environment.NewLine + Environment.NewLine;
    //            }
    //        }
    //        return exceptions;
    //    }

    //    public override string ExecuteScript(string connectionString, string script)
    //    {
    //        return ExecuteScriptInternal(connectionString, script);
    //    }

    //    public override IDataReader ExecuteSQL(string sql)
    //    {
    //        return ExecuteSQLInternal(ConnectionString, sql);
    //    }

    //    public override IDataReader ExecuteSQLTemp(string connectionString, string sql)
    //    {
    //        return ExecuteSQLInternal(connectionString, sql);
    //    }


    //    #endregion

    //}
    #endregion
}