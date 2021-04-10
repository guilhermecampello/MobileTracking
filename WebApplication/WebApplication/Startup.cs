using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication.Infrastructure;
using WebApplication.Services;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            this.AddDatabaseContext(services);
            services.AddControllers();
            this.AddApplicationServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddApplicationServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ILocaleService, LocaleService>();
        }

        private void AddDatabaseContext(
            IServiceCollection services)
        {
            var connectionString = this.Configuration.GetConnectionString("postgres");

            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString));
        }
    }
}
