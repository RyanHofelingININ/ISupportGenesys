<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Debug="true" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/ISupportGenesys.css" rel="stylesheet" />
    <title>ISupport Genesys</title>
    <style>
        
        table, th, tr, td {border: solid 1px black; border-collapse:collapse;}
        #topTable th {text-align:left;}
    </style>

</head>


<body>
    <form id="form1" runat="server">
        <table id="topTable" style="width:50%">
            <tr>
                <th style="width:100px;">User:</th>
                <td><%=UserName %></td>
            </tr>
            <tr>
                <th style="width:100px;">User ID:</th>
                <td><%=root.context.user.userId %></td>
            </tr>
            <tr>
                <th style="width:100px;">Profile ID:</th>
                <td><%=root.context.user.profileId %></td>
            </tr>
            <tr>
                <th>Account:</th>
                <td><%=accountName %></td>
            </tr>
        </table>


    </form>



</body>
</html>
