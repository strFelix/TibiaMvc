using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TibiaMvc.Models
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordString { get; set; } = string.Empty;
        public DateTime? AcessDate { get; set; }
        public string Token { get; set; } = string.Empty; 
    }
}