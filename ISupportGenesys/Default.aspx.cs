using SalesForceOAuth;
using System;
using System.Configuration;
using System.Web;
using System.Linq;
using System.Collections.Generic;

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

    protected void Page_Load(object sender, EventArgs e)
    {
        Greeting = "Hello Genesys!";

        SetUpVariables();
        
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
}