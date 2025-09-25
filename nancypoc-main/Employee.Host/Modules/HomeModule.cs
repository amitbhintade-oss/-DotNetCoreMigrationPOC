using Nancy;

namespace Employee.Host.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", _ => "Api-Employee");
        }
    }
}