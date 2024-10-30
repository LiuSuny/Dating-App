using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices
         (this IServiceCollection services, IConfiguration config)
         {
            //Adding injecting authentication service for users
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {

                        // Validate signature
                        ValidateIssuerSigningKey = true,

                         // Set signing key
                        IssuerSigningKey = new SymmetricSecurityKey(
                            // Get our secret key from configuration
                            Encoding.UTF8.GetBytes(config["TokenKey"])
                        ),

                        // Validate issuer
                        ValidateIssuer = false, //the issuer is our API server

                        // Validate audience
                        ValidateAudience = false //our angular application --client side 
                        };

                    }); 

                    //configuring policy authorization
                    services.AddAuthorization(opt => 
                    {
                        opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                        opt.AddPolicy("moderatePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
                    });
                    
                    return services;                                        
         }
    }
}