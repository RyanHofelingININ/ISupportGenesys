using SalesForceOAuth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web;

public partial class _Default : System.Web.UI.Page
{

    public List<String> log = new List<String>();
    public string SignedRequestStatus;
    public string UserName = string.Empty;
    public string accountId = string.Empty;
    public string accountName = string.Empty;
    public string Greeting = string.Empty;
    public string signedRequest;
    public string fullRequest;
    private RootObject root;
    protected void Page_Load(object sender, EventArgs e)
    {
        log.Add("Page_Load()");
        Greeting = "Hello Genesys!";
        
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
        }

    }
    private string CheckSignedRequest(string encodedSignedRequest)
    {
        log.Add("CheckSignedRequest()");
        string returnString =string.Empty;
        if (String.IsNullOrEmpty(encodedSignedRequest))
        {
            // Failed because we are not in canvas, so exit early
            returnString = "Did not find 'signed_request' POSTed in the HttpRequest. Either we are not being called by a SalesForce Canvas, or its associated Connected App isn't configured properly.";
            log.Add(string.Format("--> returnString: {0}", returnString));
            return returnString;
        }

        // Validate the signed request using the consumer secret
        string secret = GetConsumerSecret();
        var auth = new SalesForceOAuth.SignedAuthentication(secret, encodedSignedRequest);
        if (!auth.IsAuthenticatedCanvasUser)
        {
            // failed because the request is either a forgery or the connected app doesn't match our consumer secret
            returnString = "SECURITY ALERT: We received a signed request, but it did not match our consumer secret. We should treat this as a forgery and stop processing the request.";
            log.Add(string.Format("--> returnString: {0}", returnString));
            return returnString;
        }

        root = auth.CanvasContextObject;
        returnString = String.Format("SUCCESS! Here is the signed request decoded as JSON:\n{0}", auth.CanvasContextJson);
        log.Add(string.Format("--> returnString: {0}", returnString));
        return returnString;

    }

    private string GetConsumerSecret()
    {
        log.Add("GetConsumerSecret()");
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
}