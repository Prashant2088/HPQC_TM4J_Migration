<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="ALM_EXTRACT.View.Error1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="ErrorForm" runat="server">
        <asp:Image ID="imgForbidden" runat="server" Height="474px" ImageUrl="~/Images/404-error-page.jpg" Style="margin-left: 10pt" Width="845px" />
        <div style="margin-left:400px;" >
            <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/Login.aspx">Login</asp:HyperLink>
        </div>
    </form>
</body>
</html>
