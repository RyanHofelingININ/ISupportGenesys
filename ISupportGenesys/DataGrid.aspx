<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataGrid.aspx.cs" Debug="true" Inherits="DataGrid" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="Scripts/jquery-3.1.0.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>


    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/bootstrap-theme.min.css" rel="stylesheet" />
    <link href="css/ISupportGenesys.css" rel="stylesheet" />


    <link rel="stylesheet" type="text/css" href="DataGrid/jquery.dynatable.css">
    <script src="DataGrid/vendor/jquery-1.7.2.min.js"></script>
    <script src="DataGrid/jquery.dynatable.js"></script>

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

            Lifespan (Days)<br />
            <font class="<%= strMTTRColor%> LargeFont">
                <h1 class="NoPadding">
                    <%= intAverageLength %><br />
                </h1>
            </font>
            Goal: <%= intAverageLengthGoal %><br />

        </div>
    </div>

    <div class="col-md-4 pull-right">
        <div class="AnalyticsSection text-center">

            Open Cases<br />
            <font class="<%= strOpenCases%> LargeFont">
                <h1 class="NoPadding">
                    <%= intTotalIncidents %><br />
                </h1>
            </font>
            Goal: <%= intTotalIncidentsGoal %><br />

        </div>
    </div>

    <form runat="server">
        New Search:
        <div class="SearchField col-md-12">
            <table width="100%" >
                <tr>
                    <td class="text-right">
                        <label>Incident ID:</label>
                        <asp:TextBox ID="txtIncidentID" runat="server" CssClass="SelectionWidth"></asp:TextBox>
                    </td>

                    <td class="text-right">
                        <label>State:</label>
                        <asp:DropDownList ID="cmbState" runat="server" CssClass="SelectionWidth"></asp:DropDownList>
                    </td>

                    <td class="text-right">
                        <label>Contact:</label>
                        <asp:DropDownList ID="cmbContact" runat="server" CssClass="SelectionWidth"></asp:DropDownList>
                    </td>
                </tr>

                <tr>
                    <td class="text-right">
                        <label>Description:</label>
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="SelectionWidth"></asp:TextBox>
                    </td>
                    <td colspan="2"></td>
                </tr>

                <tr>
                    <td class="text-right">
                        <label>Created Date:</label>
                        <asp:DropDownList ID="cmbCreatedDate" runat="server" CssClass="SelectionWidth"></asp:DropDownList>
                    </td>

                    <td class="text-right">
                        <label>Status:</label>
                        <asp:DropDownList ID="cmbStatus" runat="server" CssClass="SelectionWidth"></asp:DropDownList>
                    </td>

                    <td class="text-right">
                        <label>Incident Type:</label>
                        <asp:DropDownList ID="cmbIncident" runat="server" CssClass="SelectionWidth"></asp:DropDownList>
                    </td>
                </tr>

                <tr>
                    <td class="text-right">
                        <label>Last Updated:</label>
                        <asp:DropDownList ID="cmbLastUpdate" runat="server" CssClass="SelectionWidth"></asp:DropDownList>
                    </td>

                    <td class="text-right">
                        <label>Priority:</label>
                        <asp:DropDownList ID="cmbPriority" runat="server" CssClass="SelectionWidth"></asp:DropDownList>
                    </td>

                    <td class="text-right">
                        <label>Categorization: </label>
                        <asp:TextBox ID="txtCategorization" runat="server" CssClass="SelectionWidth"></asp:TextBox>
                    </td>
                </tr>

                <tr>
                    <td colspan="3" class="text-right">
                        <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
                
                    </td>
                </tr>
            </table>
        </div>
    </form>

    <!-- All Open Tickets -->
    <span id="Tickets">
        <asp:DataGrid ID="OpenTickets" runat="server" AllowSorting="True">

        </asp:DataGrid>
    </span>
    <br /><br />
    
    <% if (intCodeRed > 0)
             { %>

    <div class="col-md-12" style="border: 1px black solid; padding:3px;">
        <table id="CodeRedTable" class="table">
            <thead>
                <th data-dynatable-column="Link">Link</th>
                <th>Description</th>
                <th>CreatedDateTime</th>
                <th>LastUpdatedDate</th>
                <th>State</th>
                <th>Status</th>
                <th>problemCategorization</th>
                <th>PrimaryContactName</th>
                <th>Priority</th>
                <th>IncidentType</th>
            </thead>
            <tbody>
            </tbody>
        </table>
    </div>

    <% }
    else
    { %>
    <div class="col-md-12" style="padding:3px;">
        No Red Tickets!
    <%} %>

     <% if (intNonCodeRed > 0)
             { %>
    <div class="col-md-12" style="border: 1px black solid; padding:3px;">
        <table id="my-final-table" class="table">
            <thead>
                <th data-dynatable-column="Link">Link</th>
                <th>Description</th>
                <th>CreatedDateTime</th>
                <th>LastUpdatedDate</th>
                <th>State</th>
                <th>Status</th>
                <th>problemCategorization</th>
                <th>PrimaryContactName</th>
                <th>Priority</th>
                <th>IncidentType</th>
            </thead>
            <tbody>
            </tbody>
        </table>
    </div>

    <% }
    else
    { %>
    <div class="col-md-12" style="padding:3px;">
       No Tickets!
    <%} %>

    <script>
        var jsonData;
        $(function onLoad() {
            jsonData = <%= gridData%>;
            console.log(jsonData);
            jsonDataCodeRed = <%= codeRedData%>;
            processTable(jsonData, jsonDataCodeRed);
        });

        function processTable(jsonData, jsonDataCodeRed)
        {
		    jsonData = flattenJson(jsonData);
		    console.log('JsonData: ' + JSON.stringify(jsonData[0].orgName));
		    var dynatable = $('#my-final-table').dynatable({
			    dataset: {
			    records: jsonData
			    }
		    }).data('dynatable');

		    jsonDataCodeRed = flattenJson(jsonDataCodeRed);
		    dynatable2 = $('#CodeRedTable').dynatable({
		        dataset: {
		            records: jsonDataCodeRed
		        }
		    }).data('dynatable2');		   
	    }

        function flattenJson(data)
        {
           // alert('hit');
	        var index, len;
	        for (index = 0, len = data.length; index < len; ++index) {
	            data[index].Link = '<a href="https://isupportweb-t.inin.com/ViewIncident.aspx?id="' + data[index].id + '">' + data[index].id + '</a>';
	            data[index].primaryContactName = '<a href="https://cs19.salesforce.com/"' + data[index].sfId + '">' + data[index].primaryContactName + '</a>';
		        //data[index].orgName = data[index].organization.name;
		        //data[index].orgId = data[index].organization.id;
		        //data[index].primaryContactName = data[index].primaryContact.name;
		        //data[index].primaryContactId = data[index].primaryContact.id;
	            //data[index].primaryContactId = data[index].primaryContact.id;
	        }
	        console.log(data);
	        return data;
        }

    </script>


</body>
</html>
