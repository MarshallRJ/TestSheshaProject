using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace test.TestProject.Web.Host.Startup
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Run();
        }

        public static IWebHost CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true")
                .UseStartup<Startup>()
                .Build();
        }
    }
}
