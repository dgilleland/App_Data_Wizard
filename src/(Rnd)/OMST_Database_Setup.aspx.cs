using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

public partial class OMST_Database_Setup : System.Web.UI.Page
{
    #region Private Fields
    // Following line adapted from the DotNetNuke.Data.SqlDataProvider SqlDelimiterRegex property
    private static Regex SqlDelimiterRegex = new Regex(@"(?<=(?:[^\w]+|^))GO(?=(?: |\t)*?(?:\r?\n|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
    private Database _DataStore = null;
    public Database DataStore
    {
        get
        {
            if (_DataStore == null)
                try
                {
                    _DataStore = DatabaseFactory.CreateDatabase(ConnectionStringName.Text);
                }
                catch (ConfigurationErrorsException ex)
                {
                    ConnectionStringDetails.Text = "Missing: " + ex.Message;
                    ConnectionStringDetails.Text += "<blockquote>(Falling back to hard-coded connection string details)</blockquote>";
                    try
                    {
                        _DataStore = new SqlDatabase(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|OMST_Database.mdf;Integrated Security=True;User Instance=True");
                    }
                    catch (SqlException ex2)
                    {
                        ConnectionStringDetails.Text += "<p>Fallback connection failed: " + ex2.Message + "</p>";
                    }
                }               
            return _DataStore;
        }
    }
    public string ConnectionString
    {
        get
        {
            return DataStore.ConnectionString;
        }
    }
    #endregion

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            try
            {
                ConnectionStringDetails.Text = ConnectionString;
                MessageLabel.Text = "";
                using (IDataReader reader = DataStore.ExecuteReader(CommandType.Text, "SELECT * FROM Movie"))
                {
                    MessageLabel.Text = "Able to read from Movie table.<br/>";
                }
                using (IDataReader reader = DataStore.ExecuteReader(CommandType.Text, "SELECT * FROM Purchase"))
                {
                    MessageLabel.Text += "Able to read from Purchase table.<br/>";
                }
                using (IDataReader reader = DataStore.ExecuteReader(CommandType.Text, "SELECT * FROM ShowTime"))
                {
                    MessageLabel.Text += "Able to read from ShowTime table.<br/>";
                }
                NotAccessible.Visible = false;
                DatabaseRebuildPanel.Visible = false;
            }
            catch (SqlException ex)
            {
                NotAccessible.Visible = true;
                DatabaseRebuildPanel.Visible = true;
                RunInstallationScript();
            }
            catch (Exception ex)
            {
                NotAccessible.Visible = true;
                DatabaseRebuildPanel.Visible = false;
                MessageLabel.Text = "Serious error: cannot continue. <blockquote>" + ex.Message + "<blockquote>" + ex.StackTrace + "</blockquote></blockquote>";
            }
        }
    }
    #endregion

    #region Private Methods (Script Installation)
    private void RunInstallationScript()
    {
        try
        {
            string scriptPath = Server.MapPath("App_Data/OMST_Database_Install.sql");
            string script = File.ReadAllText(scriptPath);
            string[] scripts = SqlDelimiterRegex.Split(script);
            List<RunnableScript> results = new List<RunnableScript>();
            foreach (string item in scripts)
            {
                RunnableScript thisScript = new RunnableScript();
                thisScript.SQLScript = item.Replace("\r\n", "<br/>");
                try
                {
                    DataStore.ExecuteNonQuery(CommandType.Text, item);
                    thisScript.Installed = true;
                }
                catch (Exception ex)
                {
                    thisScript.Installed = false;
                    thisScript.FailureMessage = ex.Message.Replace("\r\n", "<br/>");
                }
                results.Add(thisScript);
            }
            ScriptsRunGridview.DataSource = results;
            ScriptsRunGridview.DataBind();
            MessageLabel.Text = "Script to create database items has completed successfully.";
        }
        catch (SqlException ex)
        {
            MessageLabel.Text = ex.Message + "<br/><blockquote><h5>Stack Trace</h5>" + ex.StackTrace + "</blockquote>";
        }
    }
    #endregion

    #region Inner Class (for GridView data)
    public class RunnableScript
    {
        public string SQLScript { get; set; }
        public bool Installed { get; set; }
        public string FailureMessage { get; set; }
    }
    #endregion
    protected void ScriptsRunGridview_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Text = Server.HtmlDecode(e.Row.Cells[0].Text);
            e.Row.Cells[2].Text = Server.HtmlDecode(e.Row.Cells[2].Text);
        }
    }
}
