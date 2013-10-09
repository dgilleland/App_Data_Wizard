using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HandleAppSettings : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        // Get the application configuration file.
        System.Configuration.Configuration config =
          System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");

        // Add an entry to the appSettings section.
        int appStgCnt =
            System.Configuration.ConfigurationManager.AppSettings.Count;
        string newKey = "NewKey" + appStgCnt.ToString();

        string newValue = DateTime.Now.ToLongDateString() +
          " " + DateTime.Now.ToLongTimeString();

        // Update the configuration file appSettings section.
        config.AppSettings.Settings.Add(newKey, newValue);

        // Save the configuration file.
        config.Save(System.Configuration.ConfigurationSaveMode.Modified);

    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        // Read the appSettings section.
        System.Text.StringBuilder buffer = new System.Text.StringBuilder();
        System.Collections.Specialized.NameValueCollection appSettings =
           System.Web.Configuration.WebConfigurationManager.AppSettings;
        for (int i = 0; i < appSettings.Count; i++)
        {
            string appEntry = String.Format("#{0} Key: {1} Value: {2} <br/>",
            i, appSettings.GetKey(i), appSettings[i]);
            buffer.Append(appEntry);
        }
        Label1.Text = buffer.ToString();
    }
}