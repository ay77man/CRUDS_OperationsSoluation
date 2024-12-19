using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;

namespace CRUDS_Operations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IPersonService, PersonService>();
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddDbContext<PersonsDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefalutConnection"));
            });

            var app = builder.Build();
            if(app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
