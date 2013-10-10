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
        <uc1:DbScriptWizard ID="DbInstaller" runat="server" />
    
    </div>
    </form>
</body>
</html>
