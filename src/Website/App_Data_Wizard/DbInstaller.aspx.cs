using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class App_Data_Wizard_DbInstaller : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        //{
        //    Message.Visible = !IsUnderIISProcess();//!HostingEnvironment.IsHosted;
        //    DbInstaller.Visible = IsUnderIISProcess(); // HostingEnvironment.IsHosted;
        //}
    }

    //private bool IsUnderIISProcess()
    //{
    //    Type hosting = typeof(HostingEnvironment);
    //    return (bool)(hosting.GetProperty("IsUnderIISProcess", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null, null));
    //}
}