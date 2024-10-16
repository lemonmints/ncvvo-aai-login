using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dk.nita.saml20.identity;


namespace WebApplication1510
{
    /// <summary>
    /// Summary description for authorize
    /// </summary>
    public class authorize : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (Saml20Identity.Current != null)
            {
                Saml20Identity test = Saml20Identity.Current;

            }
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");



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