
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using API.Data;
using Microsoft.EntityFrameworkCore;


namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
           // BuildWebHost(args).Run();
           var host  = CreateHostBuilder(args).Build();
           //ways of adding seeding to our db
            using var scope = host.Services.CreateScope(); //Creates a new IServiceScope that can be used to resolve scoped services.
            var services = scope.ServiceProvider;
            try
            {
                //Get service of type DataContext from the IServiceProvider and
                //Returns a service object of type DataContext.
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync(); //this line create db if it doesn't exist and seed data if there is pending one
                await Seed.SeedUsers(context); //seeding our data
            }
            catch (Exception ex)
            {
                
                var logger  =  services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
            }

            await host.RunAsync();
        }

        // public static IWebHost BuildWebHost(string[] args) =>
        //     WebHost.CreateDefaultBuilder(args)
        //         .UseStartup<Startup>()
        //         .Build();
        
        public static IHostBuilder  CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>{
                  webBuilder.UseStartup<Startup>();
            });
                

    }
}            
