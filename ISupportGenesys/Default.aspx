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

        <form id="form1" runat="server">
        <div>
            <%= Greeting %>
        </div>

        <div>
            <%= signedRequest %>
        </div>


        <div class="AnalyticsSection">
            <center>    
                Total Code Reds<br />
                
                <font class="<%= strCodeRedColor%> LargeFont">
                    <h1 class="NoPadding">
                        <%= intCodeRed %><br />
                    </h1>
                </font>
                
                Goal: <%= intCodeRedGoal %><br />
            </center>
        </div>

        <div class="AnalyticsSection">
            <center>    
                MTTR (Days)<br />
                
                <font class="<%= strMTTRColor%> LargeFont">
                    <h1 class="NoPadding">
                        <%= fltMTTR %><br />
                    </h1>
                </font>
                
                Goal: <%= fltMTTRGoal %><br />
            </center>
        </div>

        <div class="AnalyticsSection">
            <center>    
                Open Cases<br />
                
                <font class="<%= strMTTRColor%> LargeFont">
                    <h1 class="NoPadding">
                        <%= intCodeRed %><br />
                    </h1>
                </font>
                
                Goal: <%= intCodeRedGoal %><br />
            </center>
        </div>

        <br /><br />
        <span id="Search">
            <!-- Saved Searches... lol -->


            New Search:<br />
            <div class="SearchField">

                <table cellpadding="10">
                    <tr>
                        <td align="right">
                            <label>Incident ID:</label>
                            <input type="text" name="txtIncident" />
                        </td>

                        <td align="right">
                            <label>State:</label>
                            <select class="SelectionWidth" name="cmbState" >
                                <option value="open" > Open </option>
                                <option value="closed" > Closed </option>
                            </select>
                        </td>

                        <td align="right">
                            <label>Contact:</label>
                            <select class="SelectionWidth" name="cmbState" >
                                <option value="1" > Todd Petry </option>
                                <option value="2" > Josh Lyons </option>
                            </select>
                        </td>
                    </tr>

                    <tr>
                        <td align="right">
                            <label>Descrition:</label>
                            <input type="text" name="txtDescription" />
                        </td>
                        <td colspan="2"></td>
                    </tr>

                    <tr>
                        <td align="right">
                            <label>Created Date:</label>
                             <select class="SelectionWidth" name="cmbCreated" >
                                <option value="1" > Today </option>
                                <option value="2" > Yesterday </option>
                            </select>
                        </td>

                        <td align="right">
                            <label>Status:</label>
                            <select class="SelectionWidth" name="cmbStatus" >
                                <option value="1" > Reported </option>
                                <option value="2" > Monitoring </option>
                            </select>
                        </td>

                        <td align="right">
                            <label>Incident Type:</label>
                            <select class="SelectionWidth" name="cmbIncident" >
                                <option value="1" > Customer Support </option>
                                <option value="2" > Professional Services </option>
                            </select>
                        </td>
                    </tr>

                    <tr>
                        <td align="right">
                            <label>Last Updated:</label>
                             <select class="SelectionWidth" name="cmbCreated" >
                                <option value="1" > Today </option>
                                <option value="2" > Yesterday </option>
                            </select>
                        </td>

                        <td align="right">
                            <label>Priority:</label>
                            <select class="SelectionWidth" name="cmbPriority" >
                                <option value="1" > Code Red </option>
                                <option value="2" > Code Red RCA </option>
                            </select>
                        </td>

                        <td align="right">
                            <label>Categorization: </label>
                            <select class="SelectionWidth" name="cmbCategorization" >
                                <option value="1" > Pure Cloud </option>
                                <option value="2" > Dialer </option>
                            </select>
                        </td>
                    </tr>
                </table>

                <table width="100%" cellpadding="10">
                    <tr>
                        <!-- lol 
                        
                        <td width="50%" align="left">
                            <label>Save Search:</label>
                            <input type="text" name="txtSearchName" />
                            <input type="submit" value="Save" name="cmdSaveSearch" />
                        </td>
                        -->

                        <td width="50%" align="right">
                            <input type="submit" value="Search" name="cmdSearch" />
                        </td>
                    </tr>
                </table>
            </div>
        </span>

        <!-- All Open Tickets -->
        <span id="OpenTickets">
        </span>

    </form>
    </div>

</body>
</html>
