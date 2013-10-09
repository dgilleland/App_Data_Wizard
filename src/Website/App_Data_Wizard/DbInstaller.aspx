<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DbInstaller.aspx.cs" Inherits="App_Data_Wizard_DbInstaller" %>

<%@ Register src="DbScriptWizard.ascx" tagname="DbScriptWizard" tagprefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1
        {
            width: 381px;
            height: 398px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Database Script Installer</h1>
       <asp:Label ID="Message" runat="server">
            <p>This script installer requires an ASP.NET hosted environment other than the <i>Visual Studio Development Server</i>. If you are running this in Visual Studio 2012, you can switch to the IIS Express host by right-clicking on the website in the Solution Explorer, and choosing "Use IIS Express".</p> <img alt="Use IIS Express" class="auto-style1" src="Use%20IIS%20Express.png" />
        </asp:Label>
        <uc1:DbScriptWizard ID="DbInstaller" runat="server" />
    
    </div>
    </form>
</body>
</html>
