<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DbInstaller.aspx.cs" Inherits="App_Data_Wizard_DbInstaller" %>

<%@ Register src="DbScriptWizard.ascx" tagname="DbScriptWizard" tagprefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <uc1:DbScriptWizard ID="DbScriptWizard1" runat="server" />
    
    </div>
    </form>
</body>
</html>
