using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1510
{
    /// <summary>
    /// Summary description for login
    /// </summary>
    public class login : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

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