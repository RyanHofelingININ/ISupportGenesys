using SalesForceOAuth;
using System;
using System.Configuration;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using System.Net;
using System.Text;

using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;

class Incident
{
    string IncidentID;
    string Description;
    string Created;
    string LastUpdated;
    string State;
    string Status;
    string TimeStatus;
    string Priority;
    string IncidentType;
    string Categorization;
    string Contact;
    string Age;
}

public partial class _Default : System.Web.UI.Page
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

    public string responseMessage;
    public HttpResponseMessage response;

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
        string returnString =string.Empty;
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

        string URL = "https://ininisisupportapitest.azurewebsites.net/api/v1/incidents";
        string URLParameters = "?global=true&orgid={{base64encoded-htmlencoded-orgid}}";

        string BasicAuth = "QVBJTUFOQUdFTUVOVFVTRVI6ZG9ZMjNLY2Iwb2pSc1l1SXBPaVc=";
        string BasicISupport = "aW5pbi50ZXN0MDAxQGdtYWlsLmNvbTptc3lqc202MzQxIQ==";

        

        WebRequest req = WebRequest.Create(@"https://ininisisupportapitest.azurewebsites.net/api/v1/incidents?global=true&orgid=KFwkNyU9Qi9GNUU9XltSZ2p7XHYmRw%3D%3D");
        req.Method = "GET";
        req.Headers["Authorization"] = "Basic QVBJTUFOQUdFTUVOVFVTRVI6ZG9ZMjNLY2Iwb2pSc1l1SXBPaVc=";
        req.Headers["X-ININ-ISupport-Authorization"] = "Basic aW5pbi50ZXN0MDAxQGdtYWlsLmNvbTptc3lqc202MzQxIQ==";
        
        try
        {
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            responseMessage = resp.ToString();

            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(resp.GetResponseStream(), encode);
            Char[] read = new Char[256];
            int count = readStream.Read(read, 0, 64);

            while(count > 0)
            {
                String str = new String(read, 0, count);
                responseMessage = responseMessage + str;
                count = readStream.Read(read, 0, 64);
            }

            //Incident incident = JsonConvert.DeserializeObject<Incident>(responseMessage);

            //string returnedStuff = new StreamReader(resp.GetResponseStream()).ReadToEnd();

            resp.Close();
            readStream.Close();

        }
        catch(WebException ex)
        {
            if (ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError)
                throw;

           
            responseMessage = ex.Data.ToString();
        }

    }

}