using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Testing;
using Employee.Host;

namespace Employee.Tests.Integration.TestUtilities
{
    public class TestBootstrapper : DefaultNancyBootstrapper
    {
        // Use the same bootstrapper as the main application
        // but with test-specific overrides if needed
        
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            // Add any test-specific startup logic here
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            // Override services with test implementations if needed
        }
    }

    public static class BrowserHelper
    {
        public static Browser CreateBrowser()
        {
            return new Browser(with => {
                with.Module<Employee.Host.Modules.HomeModule>();
                with.Module<Employee.Host.Modules.AuthModule>();
                with.Module<Employee.Host.Modules.EmployeeModule>();
                with.Bootstrapper<Bootstrapper>();
            });
        }

        public static Browser CreateBrowserWithTestBootstrapper()
        {
            return new Browser(with => {
                with.Bootstrapper<TestBootstrapper>();
            });
        }
    }
}