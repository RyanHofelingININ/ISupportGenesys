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

    public int intCodeRed;
    public int intCodeRedGoal;
    public float fltMTTR;
    public float fltMTTRGoal;
    public string strMTTRColor;
    public string strCodeRedColor;
    public int intTotalIncidents;
    public int intTotalIncidentsGoal;

    public string responseMessage { get; set; }
    public string gridData { get; set; }
    public HttpResponseMessage response;
    public Entities incidents;

    protected void Page_Load(object sender, EventArgs e)
    {
        Greeting = "Hello Genesys!";
        SetUpVariables();
        InitializeAPI();

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
        intCodeRed = 0;
        intCodeRedGoal = 0;
        intTotalIncidents = 0;
        intTotalIncidentsGoal = 0;

        fltMTTR = 14.2f;
        fltMTTRGoal = 14.9f;

        if (intCodeRed == 0)
        {
            strCodeRedColor = "Green";
        }
        else
        {
            strCodeRedColor = "Red";
        }

        if (fltMTTR <= fltMTTRGoal - 1)
        {
            strMTTRColor = "Green";
        }
        else if (fltMTTR <= fltMTTRGoal)
        {
            strMTTRColor = "Yellow";
        }
        else
        {
            strMTTRColor = "Red";
        }
    }

    private void InitializeAPI()
    {

        //"https://ininisisupportapitest.azurewebsites.net/api/v1/incidents?global=true&orgid={{base64encoded-htmlencoded-orgid}}"

        //    Requires 2 headers for auth:
        //Authorization Basic QVBJTUFOQUdFTUVOVFVTRVI6ZG9ZMjNLY2Iwb2pSc1l1SXBPaVc=
        // X-ININ-ISupport-Authorization Basic aW5pbi50ZXN0MDAxQGdtYWlsLmNvbTptc3lqc202MzQxIQ==

        WebRequest req = WebRequest.Create(@"https://ininisisupportapitest.azurewebsites.net/api/v1/incidents?global=true&orgid=KFwkNyU9Qi9GNUU9XltSZ2p7XHYmRw%3D%3D");
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

            test();
            
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
                createdDateTime = (string)p["createdDateTime"]
            };
            
        gridData = JsonConvert.SerializeObject(tickets);

    }

    private void test()
    {
        

        //string json = @"{
        //  'channel': {
        //    'title': 'James Newton-King',
        //    'link': 'http://james.newtonking.com',
        //    'description': 'James Newton-King\'s blog.',
        //    'item': [
        //      {
        //        'title': 'Json.NET 1.3 + New license + Now on CodePlex',
        //        'description': 'Annoucing the release of Json.NET 1.3, the MIT license and the source on CodePlex',
        //        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        //        'categories': [
        //          'Json.NET',
        //          'CodePlex'
        //        ]
        //      },
        //      {
        //        'title': 'LINQ to JSON beta',
        //        'description': 'Annoucing LINQ to JSON',
        //        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        //        'categories': [
        //          'Json.NET',
        //          'LINQ'
        //        ]
        //      }
        //    ]
        //  }
        //}";
        //
        //JObject rss = JObject.Parse(json);
        //string rssTitle = (string)rss["channel"]["title"];
        //// James Newton-King
        //
        //var postTitles =
        //from p in rss["channel"]["item"]
        //where (string)p["title"] == "LINQ to JSON beta"
        //select (string)p["title"];
        //
        //
        //
        //foreach (var item in postTitles)
        //{
        //    Console.WriteLine(item);
        //}
        //
        //var categories2 =
        //    from c in rss["channel"]["item"].SelectMany(i => i["categories"]).Values<string>()
        //    group c by c
        //    into g
        //    orderby g.Count() descending
        //    select new { Category = g.Key, Count = g.Count() };
        //
        //foreach (var c in categories2)
        //{
        //    Console.WriteLine(c.Category + " - Count: " + c.Count);
        //}

        //JObject rss = JObject.Parse(gridData);

        //string title = (string)rss["entities"][0]["id"];

        //rss["entities"][0].Remove();

        //incidents = JsonConvert.DeserializeObject<Entities>(responseMessage);
        //
        //for (int i = 0; i < incidents.Incidents.Count; i++)
        //{
        //    if (incidents.Incidents[i].status.ToString() != "Waiting Customer Contact")
        //    {
        //        incidents.Incidents.RemoveAt(i);
        //        i = 0;
        //    }
        //}


        JObject rss = JObject.Parse(responseMessage);

        var contacts =
            from c in rss["entities"]
            group c by new
            {
                primaryContactName = (string)c["primaryContact"]["name"]
            }
            into g
            select g;

        
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
        
        cmbCreatedDate.Items.Add(new ListItem("Select **", ""));
        cmbCreatedDate.Items.Add(new ListItem("Past Day", "Past Day"));
        cmbCreatedDate.Items.Add(new ListItem("Past Week", "Past Week"));
        cmbCreatedDate.Items.Add(new ListItem("Past Month", "Past Month"));
        cmbCreatedDate.Items.Add(new ListItem("Past Year", "Past Year"));

        cmbLastUpdate.Items.Add(new ListItem("Select **", ""));
        cmbLastUpdate.Items.Add(new ListItem("Past Day", "Past Day"));
        cmbLastUpdate.Items.Add(new ListItem("Past Week", "Past Week"));
        cmbLastUpdate.Items.Add(new ListItem("Past Month", "Past Month"));
        cmbLastUpdate.Items.Add(new ListItem("Past Year", "Past Year"));
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        /*
        incidents = JsonConvert.DeserializeObject<Entities>(responseMessage);

        for(int i = 0; i < incidents.Incidents.Count; i++)
        {
            if (incidents.Incidents[i].status.ToString() != "Waiting Customer Contact")
            {
                incidents.Incidents.RemoveAt(i);
                i = 0;
            } 
        }

        gridData = JsonConvert.SerializeObject(incidents);

        Page.ClientScript.RegisterStartupScript(GetType(), "key", "onLoad()", true);

        */
        gridData = responseMessage;
        JObject rss = JObject.Parse(gridData);

        string id = txtIncidentID.Text;
        string description = txtDescription.Text;
        string status = cmbStatus.Text;
        string primaryContactName = cmbContact.Text;
        string createdDateTime = cmbCreatedDate.Text;
        string priority = cmbPriority.Text;

        var tickets =
            from p in rss["entities"]
            select new {
                id = (string)p["id"],
                status = (string)p["status"],
                description = (string)p["description"],
                lastUpdatedDate = (string)p["lastUpdatedDate"],
                state = (string)p["state"],
                priority = (string)p["priority"],
                incidentType = (string)p["incidentType"],
                primaryContactName = (string)p["primaryContact"]["name"],
                createdDateTime = (string)p["createdDateTime"]
            };

        if (description != string.Empty)
        {
            tickets = tickets.Where(p => p.description == "%"+description+"%");
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

        if (createdDateTime != string.Empty)
        {
            tickets = tickets.Where(p => p.createdDateTime == createdDateTime);
        }

        if (priority != string.Empty)
        {
            tickets = tickets.Where(p => p.priority == priority);
        }

        gridData = JsonConvert.SerializeObject(tickets);

        Page.ClientScript.RegisterStartupScript(GetType(), "key", "onLoad()", true);




    }
}