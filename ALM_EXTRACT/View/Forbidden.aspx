<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forbidden.aspx.cs" Inherits="ALM_EXTRACT.View.Error" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #form1 {
            height: 761px;
        }
    </style>
</head>
<body>
    <form id="ForbiddenForm" runat="server">
        <div style="height: 729px">
            <asp:Image ID="imgUnauthorized" runat="server" Height="447px" ImageUrl="~/Images/Unauthorized-Access.png" Style="margin-left: 10pt; margin-right: 0px" Width="859px" />
            <div style="margin-left: 400px;">
                <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/Login.aspx">Login</asp:HyperLink>
            </div>
        </div>
    </form>
</body>
</html>
