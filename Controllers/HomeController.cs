using AutoServicee.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System;



namespace AutoServicee.Controllers
{
    public class HomeController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Dummy credentials - use database lookup if needed
            if (username == "admin" && password == "1234")
            {
                Session["user"] = username;
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Invalid username or password";
            return View();
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // Protect the Index page
        public ActionResult Index()
        {
            if (Session["user"] == null)
                return RedirectToAction("Login");

            List<Customer> customers = new List<Customer>();

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM customers";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            TypeOfCar = reader["TypeOfCar"].ToString(),
                            Description = reader["Description"].ToString(),
                            Pay = Convert.ToDecimal(reader["Pay"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error message to console or debug output
                Console.WriteLine($"Error: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while fetching data.";
            }

            return View(customers);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }


}