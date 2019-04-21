using Integration.DataInfraestructure.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.DataInfraestructure
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(IQueryHelper<>), typeof(SqlQueryHelper<>));
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(IDbConnectionHelper), typeof(DbConnectionHelper));
        }
    }
}
