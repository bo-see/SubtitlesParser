<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Display.aspx.cs" Inherits="AwesomeO.Display" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            <asp:Table ID="Table1" runat="server">
            </asp:Table>
            <br />
        </div>
        <div>
            <asp:TextBox ID="TextBox1" runat="server" Text="Start Frame"></asp:TextBox>
            <asp:TextBox ID="TextBox2" runat="server" Text="End Frame"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Generate" />
        </div>
    </form>
</body>
</html>
