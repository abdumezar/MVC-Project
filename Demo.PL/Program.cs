using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using Demo.PL.Mapping_Profiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services that Allows Dependency Injection

            builder.Services.AddControllersWithViews();

            /// Service LifeTime
            /// Scoped    : Create Object per Request
            /// Singleton : Create just only one Object per User
            /// Transient : Create Object per operation


            // Allow Dependency Injection
            builder.Services.AddDbContext<MVCAppDbContext>(options
                => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            /*, ServiceLifetime.Scoped*/);

            
            // services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            // services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            #region Auto Mapper
            //builder.Services.AddAutoMapper(M => M.AddProfile(new UserProfile()));
            //builder.Services.AddAutoMapper(E => E.AddProfile(new EmployeeProfile()));
            //builder.Services.AddAutoMapper(D => D.AddProfile(new DepartmentProfile()));
            //builder.Services.AddAutoMapper(R => R.AddProfile(new RoleProfile()));
            builder.Services.AddAutoMapper(typeof(MappingProfiles)); 
            #endregion

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
            })
                    .AddEntityFrameworkStores<MVCAppDbContext>()
                    .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "Account/Login";
                options.AccessDeniedPath = "Home/Error";
            });
            //services.AddScoped<UserManager<ApplicationBuilder>>();
            //services.AddScoped<UserManager<ApplicationUser>>();
            //services.AddScoped<RoleManager<IdentityRole>>();

            #endregion

            var app = builder.Build();

            #region Configure Http Request Pipeline

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });

            #endregion

            app.Run();
        }
    }
}
