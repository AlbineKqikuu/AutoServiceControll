using System.ComponentModel.DataAnnotations;
namespace AutoServicee.Models
{
    public class User
    {
        public int Id { get; set; }  // Assuming Id is an integer, change the type if it's different in your DB
        public string Username { get; set; }
        public string Password { get; set; }  // Store hashed passwords in the database
    }
}