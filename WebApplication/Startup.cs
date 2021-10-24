using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MobileTracking.Core.Application;
using MobileTracking.Core.Application.Services;
using MobileTracking.Core.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebApplication.Application.Services;
using WebApplication.BackgroundServices;
using WebApplication.Infrastructure;
using WebApplication.Middlewares;

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
            services.AddCors();
            services.AddHealthChecks();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
            {
                // System.Text.Json does not have an option to ignore reference loop handling
                // In .NET 5 we may use the native System.Text.Json instead of NewtonsoftJson
                options.SerializerSettings.ReferenceLoopHandling =
                    ReferenceLoopHandling.Ignore;

                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            this.AddApplicationServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.Migrate();
            }

            //app.UseHttpsRedirection();
            app.UseHsts();
            app.UseCors(builder =>
            {
                builder
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
            app.UseRouting();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/health");
                endpoints.MapControllers();
            });
        }

        private void AddApplicationServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ILocaleService, LocaleService>();
            services.AddScoped<IZoneService, ZoneService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<ICalibrationService, CalibrationService>();
            services.AddScoped<IPositionSignalDataService, PositionSignalDataService>();
            services.AddScoped<IPositionEstimationService, PositionEstimationService>();
            services.AddHostedService<PositionSignalDataUpdater>();
        }

        private void AddDatabaseContext(
            IServiceCollection services)
        {
            var connectionString = this.Configuration.GetConnectionString("postgres");

            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString));
        }
    }
}
