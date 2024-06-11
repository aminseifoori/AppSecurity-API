
using AppSecurity_API.Entities;
using AppSecurity_API.JwtFeatures;
using AppSecurity_API.Repository;
using AppSecurity_API.Settings;
using EmailService.Interface;
using EmailService.Service;
using EmailServiceAPI.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AppSecurity_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var congiguration = builder.Configuration;

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            congiguration.
                AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", true, true);

            // Add services to the container.

            //Add CORS Setting
            builder.Services.AddCors(option =>
            {
                option.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();

                });
            });
            //Add Identity
            builder.Services.AddIdentity<User, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 6;

                option.User.RequireUniqueEmail = true;
  
                option.Lockout.AllowedForNewUsers = true;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                option.Lockout.MaxFailedAccessAttempts = 3;
            }).AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();

            //Add Token Life Span (Reset Password)
            builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2));

            //Read JWT Setting and add Authentication
            var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidAudience = jwtSettings.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(jwtSettings.JwtSecters))
                };
            });

            //Add JWT Handler (JWT Generator)
            builder.Services.AddScoped<JwtHandler>();

            //Add DBContext - SQL Server
            builder.Services.AddDbContext<RepositoryContext>(option =>
            {
                option.UseSqlServer(congiguration.GetConnectionString("default"));
            });

            //Email Sender Configuration
            var emailConfig = builder.Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);

            //Add Email Service (We use local Nuget Package)
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            //Add AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || env == "Local")
            {
                //Enable CORS - Middleware
                app.UseCors("EnableCORS");
            }
            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            // Redirect root URL to Swagger
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/swagger");
                return Task.CompletedTask;
            });

            app.Run();
        }
    }
}