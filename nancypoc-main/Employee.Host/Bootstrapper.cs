using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.StructureMap;
using Nancy.Configuration;
using Nancy.Authentication.Stateless;
using Nancy.Json;
using StructureMap;
using Employee.Services.Service;
using Employee.Services.AuthService;
using Employee.Host.Validators;
using System.Security.Claims;

namespace Employee.Host
{
    public class Bootstrapper : StructureMapNancyBootstrapper
    {
        protected override void ConfigureContainer(IContainer container)
        {
            base.ConfigureContainer(container);

            container.Configure(x =>
            {
                x.For<IEmployeeService>().Use<EmployeeService>();
                x.For<IJwtTokenService>().Use<JwtTokenService>();
                x.For<EmployeeValidator>().Use<EmployeeValidator>();
                x.For<LoginValidator>().Use<LoginValidator>();
            });
        }

        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(enabled: false, displayErrorTraces: true);
            
            JsonSettings.MaxJsonLength = int.MaxValue;
            JsonSettings.DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        }

        protected override void RequestStartup(IContainer container, IPipelines pipelines, NancyContext context)
        {
            var jwtTokenService = container.GetInstance<IJwtTokenService>();

            var configuration = new StatelessAuthenticationConfiguration(ctx =>
            {
                var authHeader = ctx.Request.Headers.Authorization;
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return null;
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var principal = jwtTokenService.ValidateToken(token);
                
                if (principal?.Identity?.IsAuthenticated == true)
                {
                    return new JwtUserIdentity(principal);
                }

                return null;
            });

            StatelessAuthentication.Enable(pipelines, configuration);
        }
    }
}