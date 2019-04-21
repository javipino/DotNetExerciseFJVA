using Business.Users.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Users
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // Current dependencies
            services.AddTransient(typeof(IUsersService), typeof(UserService));

            // Projects dependant
            Integration.DataInfraestructure.Startup.ConfigureServices(services);
        }
    }
}
