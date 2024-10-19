using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.SessionState;
using dk.nita.saml20.identity;


namespace Akd.AAIAuth
{
    /// <summary>
    /// Summary description for authorize
    /// </summary>
    public class authorize : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var ctx = HttpContext.Current;
            if(ctx.Session ==null)
            {
                Console.WriteLine("No session");
            }
            
            
            if (Saml20Identity.Current != null)
            {

                var oib = Saml20Identity.Current["hrEduPersonOIB"][0].AttributeValue[0];
                
                context.Response.ContentType = "text/plain";
                context.Response.Write(oib);
                return;

            }
            else {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Current is null");

            }



        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }



    }
}