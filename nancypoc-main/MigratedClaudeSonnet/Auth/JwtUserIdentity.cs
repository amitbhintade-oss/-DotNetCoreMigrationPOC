using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MigratedClaudeSonnet.Auth
{
    public class JwtUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public JwtUserIdentity(ClaimsPrincipal principal)
        {
            if (principal?.Identity?.IsAuthenticated == true)
            {
                UserName = principal.FindFirst("Username")?.Value ?? string.Empty;
                Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
            }
            else
            {
                UserName = string.Empty;
                Roles = Enumerable.Empty<string>();
            }
        }

        public JwtUserIdentity()
        {
            UserName = string.Empty;
            Roles = Enumerable.Empty<string>();
        }

        public ClaimsPrincipal ToClaimsPrincipal()
        {
            var claims = new List<Claim>
            {
                new Claim("Username", UserName)
            };

            claims.AddRange(Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, "JWT");
            return new ClaimsPrincipal(identity);
        }
    }
}