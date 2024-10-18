using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using dk.nita.saml20.identity;


namespace WebApplication1510.Controllers
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
    }

    public class HomeController : Controller
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["MyDatabase"].ConnectionString;
        }
        public ActionResult Index() 
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult GetUserByOIB(string oib)
        {
            User user = GetUserByOIBFromDb(oib);

            if (user != null)
            {
                Guid ticketId = InsertAAITicket(user.Id, user.OIB);

                return Json(new { TicketId = ticketId }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return HttpNotFound("Nepostojeći korisnik");
            }
        }

        private User GetUserByOIBFromDb(string oib)
        {
            User user = null;

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT TOP 1 Id, OIB, Ime, Prezime FROM Users WHERE OIB = @OIB";
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
                                Prezime = reader.GetString(3)
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


    }
}
