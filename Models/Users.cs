using System.ComponentModel.DataAnnotations;

namespace MeterChangeApi.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}