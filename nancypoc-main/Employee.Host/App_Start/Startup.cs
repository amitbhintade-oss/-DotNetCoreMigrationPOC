using Microsoft.Owin;
using Nancy.Owin;
using Owin;

[assembly: OwinStartup(typeof(Employee.Host.Startup))]

namespace Employee.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(options => options.Bootstrapper = new Bootstrapper());
        }
    }
}