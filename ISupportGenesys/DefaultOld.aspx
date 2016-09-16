<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Debug="true" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="Scripts/jquery-3.1.0.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>


    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/bootstrap-theme.min.css" rel="stylesheet" />
    <link href="css/ISupportGenesys.css" rel="stylesheet" />

    <title>ISupport Genesys</title>
</head>

<body>

    <div class="col-md-4 pull-left">
        <div class="AnalyticsSection text-center">

            Total Code Reds<br />
            <font class="<%= strCodeRedColor%> LargeFont">
                <h1 class="NoPadding">
                    <%= intCodeRed %><br />
                </h1>
            </font>
            Goal: <%= intCodeRedGoal %><br />

        </div>
    </div>

    <div class="col-md-4">
        <div class="AnalyticsSection text-center">

            MTTR (Days)<br />
            <font class="<%= strMTTRColor%> LargeFont">
                <h1 class="NoPadding">
                    <%= fltMTTR %><br />
                </h1>
            </font>
            Goal: <%= fltMTTRGoal %><br />

        </div>
    </div>

    <div class="col-md-4 pull-right">
        <div class="AnalyticsSection text-center">

            Open Cases<br />
            <font class="<%= strMTTRColor%> LargeFont">
                <h1 class="NoPadding">
                    <%= intTotalIncidents %><br />
                </h1>
            </font>
            Goal: <%= intTotalIncidentsGoal %><br />

        </div>
    </div>

    <br /><br />
    New Search:
    <div class="SearchField">
        <table >
            <tr>
                <td class="text-right">
                    <label>Incident ID:</label>
                    <input class="SelectionWidth" type="text" name="txtIncident" />
                </td>

                <td class="text-right">
                    <label>State:</label>
                    <select class="SelectionWidth" name="cmbState" >
                        <option value="open" > Open </option>
                        <option value="closed" > Closed </option>
                    </select>
                </td>

                <td class="text-right">
                    <label>Contact:</label>
                    <select class="SelectionWidth" name="cmbState" >
                        <option value="1" > Todd Petry </option>
                        <option value="2" > Josh Lyons </option>
                    </select>
                </td>
            </tr>

            <tr>
                <td class="text-right">
                    <label>Description:</label>
                    <input class="SelectionWidth" type="text" name="txtDescription" />
                </td>
                <td colspan="2"></td>
            </tr>

            <tr>
                <td class="text-right">
                    <label>Created Date:</label>
                        <select class="SelectionWidth" name="cmbCreated" >
                        <option value="1" > Today </option>
                        <option value="2" > Yesterday </option>
                    </select>
                </td>

                <td class="text-right">
                    <label>Status:</label>
                    <select class="SelectionWidth" name="cmbStatus" >
                        <option value="1" > Reported </option>
                        <option value="2" > Monitoring </option>
                    </select>
                </td>

                <td class="text-right">
                    <label>Incident Type:</label>
                    <select class="SelectionWidth" name="cmbIncident" >
                        <option value="1" > Customer Support </option>
                        <option value="2" > Professional Services </option>
                    </select>
                </td>
            </tr>

            <tr>
                <td class="text-right">
                    <label>Last Updated:</label>
                        <select class="SelectionWidth" name="cmbCreated" >
                        <option value="1" > Today </option>
                        <option value="2" > Yesterday </option>
                    </select>
                </td>

                <td class="text-right">
                    <label>Priority:</label>
                    <select class="SelectionWidth" name="cmbPriority" >
                        <option value="1" > Code Red </option>
                        <option value="2" > Code Red RCA </option>
                    </select>
                </td>

                <td class="text-right">
                    <label>Categorization: </label>
                    <select class="SelectionWidth" name="cmbCategorization" >
                        <option value="1" > Pure Cloud </option>
                        <option value="2" > Dialer </option>
                    </select>
                </td>
            </tr>

            <tr>
                <td colspan="3" class="text-right">
                    <input type="submit" value="Search" name="cmdSearch" />
                </td>
            </tr>
        </table>
    </div>


    <!-- All Open Tickets -->
    <span id="Tickets">
        <asp:DataGrid ID="OpenTickets" runat="server" AllowSorting="True">

        </asp:DataGrid>
    </span>


</body>
</html>
