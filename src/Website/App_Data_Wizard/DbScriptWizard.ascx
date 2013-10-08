<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DbScriptWizard.ascx.cs" Inherits="App_Data_Wizard_DbScriptWizard" %>
    <div>
        <link href="//netdna.bootstrapcdn.com/font-awesome/3.2.1/css/font-awesome.css" rel="stylesheet">
        <link rel="stylesheet" href="./font-mfizz-1.2/font-mfizz.css">
        <h2>
            <i class="icon-database-alt2" style="text-decoration:none; padding:0 5px;color:#00A1F1;font-size:50px;"></i>
            Database Configuration &amp; Setup</h2>
        <p>
            Last run script:
            <asp:Label ID="LastRunScript" runat="server"></asp:Label>
        </p>
            <p>
                ScScripts Available:</p>
            <p>
                <asp:GridView ID="ScriptFileGridView" runat="server">
                </asp:GridView>
            </p>
        <p>
            Select a database connection (Sql Server only):
            <asp:DropDownList ID="AvailableConnections" runat="server">
            </asp:DropDownList>
            &nbsp;<asp:LinkButton ID="ChooseDbConnection" runat="server" OnClick="ChooseDbConnection_Click">Choose Database Connection</asp:LinkButton>
        </p>
        <p><asp:Label ID="ConnectionStringProper" runat="server" /></p>
        <p>
            <asp:LinkButton ID="InstallScripts" runat="server" OnClick="InstallScripts_Click">Install Scripts</asp:LinkButton>
            <asp:LinkButton ID="ReInstallScripts" runat="server" OnClick="ReInstallScripts_Click">Re-Install ALL Scripts</asp:LinkButton>
        </p>
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
                <asp:GridView ID="ScriptsRunGridview" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="ScriptsRunGridview_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="FileName" HeaderText="File Name" />
                        <asp:TemplateField HeaderText="Script Blocks">
                            <ItemTemplate>
                                <asp:GridView ID="ScriptBlocks" runat="server" DataSource='<%# Eval("ScriptBlocks") %>'></asp:GridView>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </p>
        </asp:Panel>
    
    </div>
    <style type="text/css">
        footer.fontCredits
        {
            position:absolute;
            bottom:3px;
            right: 3px;
            width: 80px;
            font-size: 10px;
        }
            footer.fontCredits i
            {
                font-size: 25px;
                padding-right:5px;
                float:left;
                vertical-align:top;
            }
    </style>
    <footer class="fontCredits"><i class="icon-info-sign"></i>Icon fonts by  href="http://fortawesome.github.io/Font-Awesome/" target="_blank">Font Awesome</a> and <a href="http://mfizz.com/oss/font-mfizz" target="_blank">Font Mfizz</a>. For more on styling font icons, see <a href="http://css-tricks.com/examples/IconFont/" target="_blank">css-tricks.com</a>.</footer>
