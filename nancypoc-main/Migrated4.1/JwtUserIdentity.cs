using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Employee.Host
{
    public class JwtUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }

        public JwtUserIdentity(ClaimsPrincipal principal)
        {
            if (principal?.Identity?.IsAuthenticated == true)
            {
                UserName = principal.FindFirst("Username")?.Value ?? string.Empty;
                Claims = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
            }
            else
            {
                UserName = string.Empty;
                Claims = Enumerable.Empty<string>();
            }
        }

        public JwtUserIdentity()
        {
            UserName = string.Empty;
            Claims = Enumerable.Empty<string>();
        }
    }
}
