using SalesForceOAuth;
using System;
using System.Configuration;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ISupportGenesys.Models;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;

public partial class DataGrid : System.Web.UI.Page
{
    public string SignedRequestStatus;
    public string UserName = string.Empty;
    public string accountId = string.Empty;
    public string accountName = string.Empty;
    public string accountBCFId = string.Empty;
    public string Greeting = string.Empty;
    public string signedRequest;
    public string fullRequest;
    public RootObject root;

    static public int intCodeRed;
    static public int intNonCodeRed;
    static public int intCodeRedGoal;
    static public float intAverageLength;
    static public float intAverageLengthGoal;
    static public string strMTTRColor;
    static public string strOpenCases;
    static public string strCodeRedColor;
    static public int intTotalIncidents;
    static public int intTotalIncidentsGoal;
    static public string responseMessage { get; set; }
    static public string gridData { get; set; }
    static public string codeRedData { get; set; }
    static public HttpResponseMessage response;
    static public Entities incidents;

    protected void Page_Load(object sender, EventArgs e)
    {
        Greeting = "Hello Genesys!";
        //SetUpVariables();

        if(responseMessage == null)
        {
            InitializeAPI();
        }
        

        signedRequest = Request.Params["signed_request"];

        SignedRequestStatus = CheckSignedRequest(Request.Form["signed_request"]);
        if (root == null)
        {
            Greeting = "The root is null.";
        }
        else
        {
            UserName = root.context.user.fullName;
            if (root.context.environment.parameters.ContainsKey("acctId"))
            {
                accountId = root.context.environment.parameters["acctId"];
            }
            if (root.context.environment.parameters.ContainsKey("acctName"))
            {
                accountName = root.context.environment.parameters["acctName"];
            }
            if (root.context.environment.parameters.ContainsKey("bcfOrgId"))
            {
                accountBCFId = root.context.environment.parameters["bcfOrgId"];
            }

        }

        // Hack to get the page to refresh
        if(cmbCreatedDate.Items.Count == 0)
        {
            SetForm();
            SetUpVariables();
        }

    }
    private string CheckSignedRequest(string encodedSignedRequest)
    {
        string returnString = string.Empty;
        if (String.IsNullOrEmpty(encodedSignedRequest))
        {
            // Failed because we are not in canvas, so exit early
            returnString = "Did not find 'signed_request' POSTed in the HttpRequest. Either we are not being called by a SalesForce Canvas, or its associated Connected App isn't configured properly.";
            return returnString;
        }

        // Validate the signed request using the consumer secret
        string secret = GetConsumerSecret();
        var auth = new SalesForceOAuth.SignedAuthentication(secret, encodedSignedRequest);
        if (!auth.IsAuthenticatedCanvasUser)
        {
            // failed because the request is either a forgery or the connected app doesn't match our consumer secret
            returnString = "SECURITY ALERT: We received a signed request, but it did not match our consumer secret. We should treat this as a forgery and stop processing the request.";
            return returnString;
        }

        root = auth.CanvasContextObject;
        returnString = String.Format("SUCCESS! Here is the signed request decoded as JSON:\n{0}", auth.CanvasContextJson);
        return returnString;

    }

    private string GetConsumerSecret()
    {
        // Since the consumer secret shouldn't change often, we'll put it in the Application Cache. You may want cache it differently in a production application.
        string cachedConsumerSecret = (HttpContext.Current.Application["ConsumerSecret"] ?? String.Empty).ToString();
        if (!String.IsNullOrEmpty(cachedConsumerSecret))
        {
            return cachedConsumerSecret;
        }

        // We use key names in the format "cs-key:<server>:<port><app-path>" so that we 
        // can maintain a consumer secret per server + port + app instance
        //string key = String.Format("cs-key:{0}:{1}{2}",
        //    Request.ServerVariables["SERVER_NAME"],
        //    Request.Url.Port,
        //    Request.ApplicationPath);
        string key = "cs-key";
        string secret = ConfigurationManager.AppSettings[key];
        if (!String.IsNullOrEmpty(secret))
        {
            HttpContext.Current.Application["ConsumerSecret"] = secret;
        }
        return secret;
    }

    private void SetUpVariables()
    {
        intCodeRedGoal = 0;
        intTotalIncidentsGoal = 0;

        //intAverageLength = 14.2f;
        intAverageLengthGoal = 14.9f;

        if (intCodeRed == 0)
        {
            strCodeRedColor = "Green";
        }
        else
        {
            strCodeRedColor = "Red";
        }

        if (intAverageLength <= intAverageLengthGoal - 1)
        {
            strMTTRColor = "Green";

        }
        else if (intAverageLength <= intAverageLengthGoal)
        {
            strMTTRColor = "Yellow";
        }
        else
        {
            strMTTRColor = "Red";
        }

        if(intTotalIncidents > 10)
        {
            strOpenCases = "Red";
        }
        else if(intTotalIncidents > 5)
        {
            strOpenCases = "Yellow";
        }
        else
        {
            strOpenCases = "Green";
        }
    }

    private void InitializeAPI()
    {

        //"https://ininisisupportapitest.azurewebsites.net/api/v1/incidents?global=true&orgid={{base64encoded-htmlencoded-orgid}}"

        //    Requires 2 headers for auth:
        //Authorization Basic QVBJTUFOQUdFTUVOVFVTRVI6ZG9ZMjNLY2Iwb2pSc1l1SXBPaVc=
        // X-ININ-ISupport-Authorization Basic aW5pbi50ZXN0MDAxQGdtYWlsLmNvbTptc3lqc202MzQxIQ==

        WebRequest req = WebRequest.Create(@"https://incidentinfoapitest.azurewebsites.net/api/v1/incidents?global=true&orgid=KFwkNyU9Qi9GNUU9XltSZ2p7XHYmRw%3D%3D");
        req.Method = "GET";
        req.Headers["Authorization"] = "Basic QVBJTUFOQUdFTUVOVFVTRVI6ZG9ZMjNLY2Iwb2pSc1l1SXBPaVc=";
        req.Headers["X-ININ-ISupport-Authorization"] = "Basic aW5pbi50ZXN0MDAxQGdtYWlsLmNvbTptc3lqc202MzQxIQ==";

        try
        {
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

            using (Stream stream = resp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                responseMessage = reader.ReadToEnd();
                reader.Close();
            }
            resp.Close();

            incidents = JsonConvert.DeserializeObject<Entities>(responseMessage);
         
            if (incidents.total != 0)
            {
                intTotalIncidents = incidents.total;
            }

            SetGridData();
            SetForm();
            SetUpVariables();
        }
        catch (WebException ex)
        {
            if (ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError)
                throw;

            responseMessage = ex.Data.ToString();
        }

    }

    public void SetGridData()
    {
        gridData = responseMessage;
        JObject rss = JObject.Parse(gridData);

        var tickets =
            from p in rss["entities"]
            select new
            {
                id = (string)p["id"],
                status = (string)p["status"],
                description = (string)p["description"],
                lastUpdatedDate = (string)p["lastUpdatedDate"],
                state = (string)p["state"],
                priority = (string)p["priority"],
                incidentType = (string)p["incidentType"],
                primaryContactName = (string)p["primaryContact"]["name"],
                createdDateTime = (string)p["createdDateTime"],
                problemCategorization = (string)p["problemCategorization"],
                sfId = (string)p["primaryContact"]["sfId"]
            };

        var tickets2 = tickets.Where(p => p.priority != "Code Red" || p.priority != "Code Red RCA");

        intNonCodeRed = tickets2.Count();

        gridData = JsonConvert.SerializeObject(tickets2);

        tickets = tickets.Where(p => p.priority == "Code Red" || p.priority == "Code Red RCA");

        intCodeRed = tickets.Count();

        codeRedData = JsonConvert.SerializeObject(tickets);
    }

    private void SetForm()
    {
        JObject rss = JObject.Parse(responseMessage);

        var contacts =
            from c in rss["entities"]
            group c by (string)c["primaryContact"]["name"]
            into g
            orderby g.Key
            select g;


        var createdDates = from cd in rss["entities"]
                           group cd by (DateTime)cd["createdDateTime"]
                           into g
                           orderby g.Key
                           select g;

        intAverageLength = (int)createdDates.Average(g => DateTime.Now.Subtract(g.Key).TotalDays);

       

        // ["lastUpdatedDate"]

        cmbContact.Items.Add(new ListItem("Select **", ""));

        foreach ( var g in contacts)
        {
            cmbContact.Items.Add(new ListItem(g.Key, g.Key));
        }
        // string testing = JsonConvert.SerializeObject(contacts);

        cmbState.Items.Add(new ListItem("Open", "Open"));

        // Add Statuses
        cmbStatus.Items.Add(new ListItem("Select **", ""));
        cmbStatus.Items.Add(new ListItem("Reported", "Reported"));
        cmbStatus.Items.Add(new ListItem("Waiting on Customer Contact", "Waiting on Customer Contact"));
        cmbStatus.Items.Add(new ListItem("Waiting on Vendor Contact", "Waiting on Vendor Contact"));
        cmbStatus.Items.Add(new ListItem("Waiting Engineer", "Waiting Engineer"));
        cmbStatus.Items.Add(new ListItem("Waiting Development", "Waiting Development"));
        cmbStatus.Items.Add(new ListItem("Waiting Internal Contributor", "Reported"));
        cmbStatus.Items.Add(new ListItem("Resolved", "Resolved"));

        cmbPriority.Items.Add(new ListItem("Select **", ""));
        cmbPriority.Items.Add(new ListItem("Code Red", "Code Red"));
        cmbPriority.Items.Add(new ListItem("Code Red RCA", "Code Red RCA"));
        cmbPriority.Items.Add(new ListItem("High", "High"));
        cmbPriority.Items.Add(new ListItem("Medium", "Medium"));
        cmbPriority.Items.Add(new ListItem("Low", "Low"));

        cmbIncident.Items.Add(new ListItem("Select **", ""));
        cmbIncident.Items.Add(new ListItem("Professional Services", "Professional Services"));
        cmbIncident.Items.Add(new ListItem("Managed Services", "Managed Services"));
        cmbIncident.Items.Add(new ListItem("Membership Services", "Membership Services"));
        cmbIncident.Items.Add(new ListItem("SaaS Operations", "SaaS Operations"));
        cmbIncident.Items.Add(new ListItem("Latitude Support", "Latitude Support"));
        cmbIncident.Items.Add(new ListItem("Bay Bridge Support", "Bay Bridge Support"));
        cmbIncident.Items.Add(new ListItem("Customer Support", "Customer Support"));
        cmbIncident.Items.Add(new ListItem("AcroSoft Support", "AcroSoft Support"));
        cmbIncident.Items.Add(new ListItem("Unknown", "Unknown"));
        
        cmbCreatedDate.Items.Add(new ListItem("Select **", "0"));
        cmbCreatedDate.Items.Add(new ListItem("Past Day", "1"));
        cmbCreatedDate.Items.Add(new ListItem("Past Week", "7"));
        cmbCreatedDate.Items.Add(new ListItem("Past Month", "30"));
        cmbCreatedDate.Items.Add(new ListItem("Past Year", "365"));

        cmbLastUpdate.Items.Add(new ListItem("Select **", "0"));
        cmbLastUpdate.Items.Add(new ListItem("Past Day", "1"));
        cmbLastUpdate.Items.Add(new ListItem("Past Week", "7"));
        cmbLastUpdate.Items.Add(new ListItem("Past Month", "30"));
        cmbLastUpdate.Items.Add(new ListItem("Past Year", "365"));
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        gridData = responseMessage;
        JObject rss = JObject.Parse(gridData);

        string id = txtIncidentID.Text;
        string description = txtDescription.Text;
        string status = cmbStatus.Text;
        string primaryContactName = cmbContact.Text;
        string priority = cmbPriority.Text;
        string problemCategorization = txtCategorization.Text;
        int lastUpdatedDate = Convert.ToInt32(cmbLastUpdate.SelectedValue);
        int createddate = Convert.ToInt32(cmbCreatedDate.SelectedValue);

        var tickets =
            from p in rss["entities"]
            select new {
                id = (string)p["id"],
                status = (string)p["status"],
                description = (string)p["description"],
                lastUpdatedDate = (DateTime)p["lastUpdatedDate"],
                state = (string)p["state"],
                priority = (string)p["priority"],
                incidentType = (string)p["incidentType"],
                primaryContactName = (string)p["primaryContact"]["name"],
                createdDateTime = (DateTime)p["createdDateTime"],
                problemCategorization = (string)p["problemCategorization"],
                sfId = (string)p["primaryContact"]["sfId"]
            };

        if (description != string.Empty)
        {
            tickets = tickets.Where(p => p.description.Contains(description));
        }

        if (id != string.Empty)
        {
            tickets = tickets.Where(p => p.id == id);
        }

        if (status != string.Empty)
        {
            tickets = tickets.Where(p => p.status == status);
        }

        if (primaryContactName != string.Empty)
        {
            tickets = tickets.Where(p => p.primaryContactName == primaryContactName);
        }
        
        if (lastUpdatedDate != 0)
        {
            tickets = tickets.Where(p => DateTime.Now.Subtract(Convert.ToDateTime(p.lastUpdatedDate)).TotalDays <= lastUpdatedDate);
        }

        if (createddate != 0)
        {
            tickets = tickets.Where(p => DateTime.Now.Subtract(Convert.ToDateTime(p.createdDateTime)).TotalDays <= createddate);
        }

        if (priority != string.Empty)
        {
            tickets = tickets.Where(p => p.priority == priority);
        }

        if (problemCategorization != string.Empty)
        {
            tickets = tickets.Where(p => p.problemCategorization.Contains(problemCategorization));
        }

        var tickets2 = tickets.Where(p => p.priority != "Code Red" || p.priority != "Code Red RCA");

        intNonCodeRed = tickets2.Count();

        gridData = JsonConvert.SerializeObject(tickets2);

        tickets = tickets.Where(p => p.priority == "Code Red" || p.priority == "Code Red RCA");

        intCodeRed = tickets.Count();

        codeRedData = JsonConvert.SerializeObject(tickets);

        Page.ClientScript.RegisterStartupScript(GetType(), "key", "onLoad()", true);
    }
}