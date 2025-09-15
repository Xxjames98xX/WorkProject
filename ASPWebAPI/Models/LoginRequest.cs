using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Serilog;

namespace ASPWebAPI.Models
{
    public class LoginRequest
    {
        // CANNOT HAVE CONSTRUCTOR FOR JSON DESERIALIZATION
        // Default constructor is provided by C# if no constructors are defined
        // LoginRequest(string Username, string Password)
        // {
        //     this.Username = Username;
        //     this.Password = Password;
        // }

        // To make Id the primary key and auto-incrementing
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
