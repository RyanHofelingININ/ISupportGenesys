<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Debug="true" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="Scripts/jquery-3.1.0.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <link href="css/ISupportGenesys.css" rel="stylesheet" />
    <title>ISupport Genesys</title>
    <style>
        
    </style>

</head>


<body>
    <div class="container">
        <div class="row">
            <div class="col-lg-4">
                User:
            </div>
            <div class="col-lg-8">
                <%= UserName  %>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-4">
                Account:
            </div>
            <div class="col-lg-8">
                <%= accountName %>
            </div>
        </div>
    </div>

</body>
</html>
