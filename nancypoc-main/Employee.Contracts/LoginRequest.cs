namespace Employee.Contracts
{
    public class LoginRequest
    {
        public int EmpId { get; set; }
        
        public string Password { get; set; }
    }
}