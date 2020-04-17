using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Server
{
    public class Startup
    {
     
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("OAuth")
            .AddJwtBearer("OAuth",config => 
            {
                var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
                var key = new SymmetricSecurityKey(secretBytes);
                config.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Query.ContainsKey("access_token"))
                        {
                            context.Token = context.Request.Query["access_token"];
                        }
                        return Task.CompletedTask;
                    }
                };
                config.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Constants.Issuer,
                    ValidAudience = Constants.Audiance,

                    IssuerSigningKey = key,
                };
            });
              
               
            
            services.AddControllersWithViews();
            //services.AddRazorPages()
            //    .AddRazorPagesOptions(confg =>
            //    {
            //        confg.Conventions.AuthorizePage("/Razor/Secured");
            //        confg.Conventions.AuthorizePage("/Razor/Policy", "saba");
            //        confg.Conventions.AuthorizeFolder("/RazorSecured");
            //        confg.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
            //    });
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
                //endpoints.MapRazorPages();
            });
        }
    }
}
