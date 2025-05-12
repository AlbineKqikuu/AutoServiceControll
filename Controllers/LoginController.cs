using MySql.Data.MySqlClient;
using System.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace AutoServicee.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username=@username AND Password=@password";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password); // No hashing

                    var result = Convert.ToInt32(cmd.ExecuteScalar());
                    if (result > 0)
                    {
                        Session["user"] = username;
                        return RedirectToAction("Index", "Home"); // Redirect to Home controller's Index
                    }
                }
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

    }
}
