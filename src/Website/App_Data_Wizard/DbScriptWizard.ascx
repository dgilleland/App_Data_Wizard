<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DbScriptWizard.ascx.cs" Inherits="App_Data_Wizard_DbScriptWizard" %>
<div>
    <style type="text/css">
        /* From http://css-tricks.com/snippets/css/truncate-string-with-ellipsis/ */
        td.truncate
        {
            width: 350px;
            height: 40px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        td.truncate:hover
        {
            width: 350px;
            height: auto;
            white-space: normal;
            overflow: auto;
            text-overflow: initial;
        }

        /* From http://stackoverflow.com/a/7650986/2154662 */
        .scriptBlockGrid {
            table-layout:fixed; 
            width:750px; 
        }

        .dbIcon
        {
            text-decoration: none; 
            padding: 0 5px; 
            color: #00A1F1; 
            font-size: 50px;
        }
    </style>
<%--    <script type="text/javascript">

        // From http://stackoverflow.com/a/4523417/2154662
        function loadScript(src, callback) {
            var head = document.getElementsByTagName('head')[0];
            var script = document.createElement('script');
            script.type = 'text/javascript';
            script.onreadystatechange = function () {
                if (this.readyState == 'complete' || this.readyState == 'loaded') {
                    callback();
                }
            }
            script.onload = callback;
            script.src = src;
            head.appendChild(script);
        }

        function isjQueryLoaded() {
            return (typeof jQuery !== 'undefined');
        }

        function tryLoadChain() {
            var chain = arguments;
            if (!isjQueryLoaded()) {
                if (chain.length) {
                    loadScript(
                        chain[0],
                        function () {
                            tryLoadChain.apply(this, Array.prototype.slice.call(chain, 1));
                        }
                    );
                } else {
                    alert('not loaded!');
                }
            } else {
                alert('loaded!');
            }
        }

        tryLoadChain(
            'https://ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js',
            'http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.4.min.js',
            'http://cdnjs.cloudflare.com/ajax/libs/jquery/2.0.3/jquery.min.js',
            'mine.js');

    </script>--%>
    <link href="//netdna.bootstrapcdn.com/font-awesome/3.2.1/css/font-awesome.css" rel="stylesheet">
    <link rel="stylesheet" href="./font-mfizz-1.2/font-mfizz.css">
    <h2>
        <i class="icon-database-alt2 dbIcon"></i>
        Database Configuration &amp; Setup</h2>
    <asp:Label ID="WarningMessage" runat="server">
            <p><span class="Warning">Please note:</span> This script installer requires an ASP.NET hosted environment other than the <i>Visual Studio Development Server</i>. If you are running this in Visual Studio 2012, you can switch to the IIS Express host by right-clicking on the website in the Solution Explorer, and choosing "Use IIS Express".</p> <img alt="Use IIS Express" class="auto-style1" src="Use%20IIS%20Express.png" />
    </asp:Label>
    <p>
        Last run script:
            <asp:Label ID="LastRunScript" runat="server"></asp:Label>
    </p>
    <p>
        Scripts Available:
    </p>
    <p>
        <asp:GridView ID="ScriptFileGridView" runat="server">
        </asp:GridView>
    </p>
    <p>
        Select a database connection (Sql Server only):
            <asp:DropDownList ID="AvailableConnections" runat="server">
            </asp:DropDownList>
        <asp:LinkButton ID="ChooseDbConnection" runat="server" OnClick="ChooseDbConnection_Click">Choose Database Connection</asp:LinkButton>
        <asp:HiddenField ID="ConnectionStringName" runat="server" />
    </p>
    <p>
        <asp:Label ID="ConnectionStringProper" runat="server" /></p>
    <p>
        <asp:LinkButton ID="InstallScripts" runat="server" OnClick="InstallScripts_Click">Install Scripts</asp:LinkButton>
        <asp:LinkButton ID="ReInstallScripts" runat="server" OnClick="ReInstallScripts_Click">Re-Install ALL Scripts</asp:LinkButton>
    </p>
    <p>
        <asp:Label ID="MessageLabel" runat="server" Font-Bold="True"
            ForeColor="#CC0000"></asp:Label>
    </p>
    <asp:Panel ID="DatabaseRebuildPanel" runat="server">
        <h4>Database Rebuild Details</h4>
        <p>
            Scripts Run:
        </p>
        <%--<p class="truncate">
            <span>IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_StudentPayment_Amount]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudentPayment]'))<br>ALTER TABLE [dbo].[StudentPayment] DROP CONSTRAINT [CK_StudentPayment_Amount]<br></span>
        </p>--%>
        <asp:GridView ID="ScriptsRunGridview" runat="server" AutoGenerateColumns="false" RowStyle-VerticalAlign="Top">
            <EmptyDataTemplate>No scripts were executed for the database.</EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="FileName" HeaderText="File Name" />
                <asp:TemplateField HeaderText="Script Blocks">
                    <ItemTemplate>
                        <asp:GridView ID="ScriptBlocks" runat="server" AutoGenerateColumns="false"
                            DataSource='<%# Eval("ScriptBlocks") %>'
                            RowStyle-VerticalAlign="Top" CssClass="scriptBlockGrid"
                            OnRowDataBound="ScriptBlocks_RowDataBound">
                            <Columns>
                                <asp:CheckBoxField HeaderText="Installed" DataField="Installed" />
                                <asp:TemplateField HeaderText="Sql Script" ItemStyle-Width="350px" ItemStyle-CssClass="truncate">
                                    <ItemTemplate>
                                        <asp:Label ID="Sql" runat="server" CssClass=""><%# HtmlDecode(Eval("SQLScript")) %></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sql Script" ItemStyle-Width="350px" ItemStyle-CssClass="truncate">
                                    <ItemTemplate>
                                        <asp:Label ID="FailMsg" runat="server" CssClass="" Text='<%# HtmlDecode(Eval("FailureMessage")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>

</div>
<style type="text/css">
    footer.fontCredits
    {
        position: absolute;
        top: 3px;
        right: 3px;
        width: 80px;
        font-size: 10px;
        padding:4px;
        background-color: #F0E68C;
    }

        footer.fontCredits i
        {
            font-size: 25px;
            padding-right: 5px;
            float: left;
            vertical-align: top;
        }
</style>
<footer class="fontCredits"><i class="icon-info-sign"></i>Icon fonts by <a href="http://fortawesome.github.io/Font-Awesome/" target="_blank">Font Awesome</a> and <a href="http://mfizz.com/oss/font-mfizz" target="_blank">Font Mfizz</a>. For more on styling font icons, see <a href="http://css-tricks.com/examples/IconFont/" target="_blank">css-tricks.com</a>.</footer>
