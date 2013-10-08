<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DbScriptWizard.ascx.cs" Inherits="App_Data_Wizard_DbScriptWizard" %>
    <div>
    
        <h2>
            Database Configuration &amp; Setup</h2>
        <p>
            Select a database connection (Sql Server only):
            <asp:DropDownList ID="AvailableConnections" runat="server">
            </asp:DropDownList>
        </p>
        <p>
            Last run script:
            <asp:Label ID="LastRunScript" runat="server"></asp:Label>
        </p>
        <p>
            &nbsp;</p>
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
<asp:Wizard ID="Wizard1" runat="server">
    <WizardSteps>
        <asp:WizardStep runat="server" title="Database Connection">
            Select a database connection (Sql Server only):
            <asp:DropDownList ID="AvailableConnections0" runat="server">
            </asp:DropDownList>
        </asp:WizardStep>
        <asp:WizardStep runat="server" title="Database Objects">
        </asp:WizardStep>
        <asp:WizardStep runat="server" Title="Available Scripts">
        </asp:WizardStep>
        <asp:WizardStep runat="server" Title="Execution Plan">
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>

