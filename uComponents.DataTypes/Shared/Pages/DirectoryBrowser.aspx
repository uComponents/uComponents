<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/umbraco/masterpages/umbracoPage.Master" CodeBehind="DirectoryBrowser.aspx.cs" Inherits="uComponents.DataTypes.Shared.Pages.DirectoryBrowser" %>

<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="cdf" Namespace="ClientDependency.Core.Controls" Assembly="ClientDependency.Core" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
    
    <cdf:CssInclude ID="CssInclude1" runat="server" FilePath="Editors/DirectoryBrowser.css" PathNameAlias="UmbracoClient"></cdf:CssInclude>
    
    <script type="text/javascript">
        function postPath(path) {
            var elementId = '<%=Target%>';
            top.right.document.getElementById(elementId).value = path;
            UmbClientMgr.closeModalWindow();
        }
    </script>

</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <cc1:Pane runat="server" Width="100px" ID="pane">
        <asp:PlaceHolder runat="server" ID="Output"></asp:PlaceHolder>
    </cc1:Pane>
</asp:Content>
