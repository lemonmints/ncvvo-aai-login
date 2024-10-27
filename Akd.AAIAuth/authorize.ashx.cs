using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.SessionState;
using dk.nita.saml20.identity;


namespace Akd.AAIAuth
{

    public class User
    {
        public int Id
        {
            get; set;
        }
        public string Ime
        {
            get; set;
        }
        public string Prezime
        {
            get; set;
        }
        public string OIB
        {
            get; set;
        }
        public bool IsActive { get; set; }
    }
    /// <summary>
    /// Summary description for authorize
    /// </summary>
    public class authorize : IHttpHandler, IRequiresSessionState
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["NCVVO"].ConnectionString;
        }
        private User GetUserByOIBFromDb(string oib)
        {
            User user = null;

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT TOP 1 Id, OIB, Ime, Prezime,IsActive FROM Users WHERE OIB = @OIB";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OIB", oib);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32(0),
                                OIB = reader.GetString(1),
                                Ime = reader.GetString(2),
                                Prezime = reader.GetString(3),
                                IsActive = reader.GetBoolean(4),

                            };
                        }
                    }
                }
            }

            return user;
        }

        private Guid InsertAAITicket(int userId, string oib)
        {
            Guid ticketId = Guid.NewGuid();

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                 connection.Open();
                string query = "INSERT INTO AAITicket (Id, UserId, OIB, CreatedAt) VALUES (@Id, @UserId, @OIB, @CreatedAt)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", ticketId);
                    command.Parameters.AddWithValue("@UserId", userId.ToString());
                    command.Parameters.AddWithValue("@OIB", oib);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }

            return ticketId;
        }


     
        public void ProcessRequest(HttpContext context)
        {
            
            if (Saml20Identity.Current != null)
            {

                var oib = Saml20Identity.Current["hrEduPersonOIB"][0].AttributeValue[0];
                var user = GetUserByOIBFromDb(oib);
                if(user != null && user.IsActive)
                {
                    var redirectDomain = ConfigurationManager.AppSettings["RedirectDomain"];
                    var ticketId = InsertAAITicket(user.Id, user.OIB);
                    context.Response.Redirect($"{redirectDomain}/aai-login?ticketId={ticketId}", endResponse: true);
                }
                else
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = 403;
                    context.Response.Write("Unauthorized");
                }
            }
            else {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Something went wrong, SAML identity is not set.");
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