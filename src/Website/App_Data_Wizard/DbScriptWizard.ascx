<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DbScriptWizard.ascx.cs" Inherits="App_Data_Wizard_DbScriptWizard" %>
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
<asp:Wizard ID="Wizard1" runat="server">
    <WizardSteps>
        <asp:WizardStep runat="server" title="Database Connection">
        </asp:WizardStep>
        <asp:WizardStep runat="server" title="Database Objects">
        </asp:WizardStep>
        <asp:WizardStep runat="server" Title="Available Scripts">
        </asp:WizardStep>
        <asp:WizardStep runat="server" Title="Execution Plan">
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>

