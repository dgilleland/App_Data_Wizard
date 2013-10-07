<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OMST_Database_Setup.aspx.cs" Inherits="OMST_Database_Setup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <h2>
            Database Configuration &amp; Setup</h2>
        <p>
            The expected connection string name is
            <asp:Label ID="ConnectionStringName" runat="server" Font-Bold="True" 
                ForeColor="Blue" Text="OMST_db"></asp:Label>
            .</p>
        <p>
            The actual connection string value is
            <asp:Label ID="ConnectionStringDetails" runat="server" Font-Bold="True" 
                Font-Italic="True" Font-Overline="True" Font-Underline="True" 
                ForeColor="#009933"></asp:Label>
            .</p>
        <p>
            The database content was<asp:Label ID="NotAccessible" 
                runat="server" Font-Italic="True" Text="&amp;nbsp;not" Visible="False"></asp:Label>&nbsp;accessible.&nbsp;</p>
        <p>
            <asp:Label ID="MessageLabel" runat="server" Font-Bold="True" 
        ForeColor="#CC0000"></asp:Label>
</p>
        <asp:Panel ID="DatabaseRebuildPanel" runat="server">
            <h4>
                Database Rebuild Details</h4>
            <p>
                Scripts Run:</p>
            <p>
                <asp:GridView ID="ScriptsRunGridview" runat="server" 
                    onrowdatabound="ScriptsRunGridview_RowDataBound">
                </asp:GridView>
            </p>
        </asp:Panel>
    
    </div>
    </form>
</body>
</html>
