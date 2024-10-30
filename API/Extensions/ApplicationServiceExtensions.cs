using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
       
         public static IServiceCollection AddApplicationServices(this 
         IServiceCollection services, IConfiguration config)
         {
             //Note: if we are to build our application on mvc then we would have use 
             //the defualt Addidentity as everything would be build on .net serve
             //however since our applicationis built with external frontend angular we need the 
             //AddIdentityCore as it provide extra functions etc
             services.AddIdentityCore<AppUser>(opt => 
             {
                //Note: if we required a weak password then we have to turn off some of the required config option manually 
                //by using opt.Password.RequireDigit etc else it will required a strong password by default

                opt.Password.RequireNonAlphanumeric = false;
                
             })
              .AddRoles<AppRole>()
              .AddRoleManager<RoleManager<AppRole>>()
              .AddSignInManager<SignInManager<AppUser>>()
              .AddRoleValidator<RoleValidator<AppRole>>()
              .AddEntityFrameworkStores<DataContext>();


            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });                        
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<IUserRepository, UserRepository>(); 
            services.AddScoped<ILikesRepository, LikesRepository>();  
            services.AddScoped<IMessageRepository, MessageRepository>();            
            //services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly); //finding where we mapped our automapper class
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 
            return services;
         }
    }
}