using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Basics.AuthorizationRequirements;
using Basics.Controllers;
using Basics.CustomPolicyProvider;
using Basics.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basics
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Cookie Auth")
                .AddCookie("Cookie Auth", config =>
                 {
                     config.Cookie.Name = "Grandmas.cookie";
                     config.LoginPath = "/Home/Authenticate";
                 });
            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //.RequireAuthenticatedUser()
                //.RequireClaim(ClaimTypes.DateOfBirth)
                //.Build();
                //config.DefaultPolicy = defaultAuthPolicy;
                config.AddPolicy("saba", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "saba"));
                config.AddPolicy("Claim.DoB", policyBuilder =>
                 {
                     policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);

                 });

            });
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
            services.AddControllersWithViews(config =>
            {
                
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                .RequireAuthenticatedUser()
                .Build();

                //Global Authorization filter
                config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));

            });
            services.AddRazorPages()
                .AddRazorPagesOptions(confg =>
                {
                    confg.Conventions.AuthorizePage("/Razor/Secured");
                    confg.Conventions.AuthorizePage("/Razor/Policy","saba");
                    confg.Conventions.AuthorizeFolder("/RazorSecured");
                    confg.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //Who are you
            app.UseAuthentication();
            //Are you allowed
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
