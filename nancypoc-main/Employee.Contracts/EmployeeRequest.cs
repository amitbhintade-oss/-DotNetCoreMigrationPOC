using System;
using Newtonsoft.Json;

namespace Employee.Contracts
{
    public class EmployeeRequest
    {
        public int EmpId { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string Role { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("Password")]
        public string PasswordHash { get; set; }
    }
}