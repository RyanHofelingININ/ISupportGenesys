<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Debug="true" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/ISupportGenesys.css" rel="stylesheet" />
    <title>ISupport Genesys</title>
    <style>
        table, th, tr, td {border: solid 1px black; border-collapse:collapse;}
    </style>

</head>


<body>
    <form id="form1" runat="server">
        <table style="width:100%">
            <tr>
                <th style="width:100px;">User:</th>
                <td><%=UserName %></td>
            </tr>
            <tr>
                <th>Account:</th>
                <td><%=accountName %></td>
            </tr>
        </table>


    </form>



</body>
</html>
