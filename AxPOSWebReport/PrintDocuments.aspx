<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintDocuments.aspx.cs" Inherits="AxPOSWebReport.PrintDocuments" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #Button1 {
            width: 101px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <input id="printInvoice" type="button" value="Print Invoice" /><asp:TextBox ID="txtInvoice" runat="server"></asp:TextBox>
    
    </div>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
    </form>
</body>
</html>
