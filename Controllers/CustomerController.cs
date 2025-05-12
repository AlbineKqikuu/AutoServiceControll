using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using AutoServicee.Models; // Add this to reference the Customer model
using System;

namespace AutoServicee.Controllers
{
    public class CustomerController : Controller
    {
        private string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

        // Action to display customers
        public ActionResult Index()
        {
            var customers = new List<Customer>();

            // Database interaction
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM Customers", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        TypeOfCar = reader.GetString("TypeOfCar"),
                        Description = reader.GetString("Description"),
                        Pay = reader.GetDecimal("Pay")
                    });
                }
            }

            // Return the customers list to the view
            return View(customers);
        }
        // GET: Customer/Create
        public ActionResult Create()
        {
            if (Session["user"] == null)
                return RedirectToAction("Login");

            return View();
        }
        // Action to add a customer
        [HttpPost]
        public ActionResult Create(Customer c)
        {

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO Customers (Name, TypeOfCar, Description, Pay) VALUES (@n, @t, @d, @p)", conn);
                cmd.Parameters.AddWithValue("@n", c.Name);
                cmd.Parameters.AddWithValue("@t", c.TypeOfCar);
                cmd.Parameters.AddWithValue("@d", c.Description);
                cmd.Parameters.AddWithValue("@p", c.Pay);
                cmd.ExecuteNonQuery();
            }

            // Redirect back to Index after adding a customer
            return RedirectToAction("Index", "Home");

        }
        // GET: Update
        public ActionResult Update(int id)
        {
            // Check if the user is logged in
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }

            // Retrieve customer details from the database
            Customer customer = null;
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM Customers WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            TypeOfCar = reader["TypeOfCar"].ToString(),
                            Description = reader["Description"].ToString(),
                            Pay = Convert.ToDecimal(reader["Pay"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while fetching customer details.";
            }

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // POST: Update (Handle form submission)
        [HttpPost]
        public ActionResult Update(Customer c)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                // If model is invalid, return to the update form with error messages
                return View(c);
            }

            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("UPDATE Customers SET Name=@n, TypeOfCar=@t, Description=@d, Pay=@p WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@n", c.Name);
                    cmd.Parameters.AddWithValue("@t", c.TypeOfCar);
                    cmd.Parameters.AddWithValue("@d", c.Description);
                    cmd.Parameters.AddWithValue("@p", c.Pay);
                    cmd.Parameters.AddWithValue("@id", c.Id);
                    cmd.ExecuteNonQuery();
                }

                // Redirect back to Index after updating the customer
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while updating the customer.";
                return View(c);  // Return the view with the customer object in case of an error
            }
        }

        // GET: Confirm Delete
        public ActionResult Delete(int id)
        {
            if (Session["user"] == null)
                return RedirectToAction("Login");

            Customer customer = null;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM Customers WHERE ID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            Name = reader["Name"].ToString(),
                            TypeOfCar = reader["TypeOfCar"].ToString(),
                            Description = reader["Description"].ToString(),
                            Pay = Convert.ToDecimal(reader["Pay"])
                        };
                    }
                }
            }

            return View(customer);
        }

        // POST: Delete Customer
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM Customers WHERE ID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
