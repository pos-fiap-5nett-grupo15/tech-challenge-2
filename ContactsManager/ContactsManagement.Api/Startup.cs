


using ContactsManagement.Application.Handlers.Auth;
using ContactsManagement.Application.Handlers.Contact.CreateContact;
using ContactsManagement.Application.Handlers.Contact.DeleteContactById;
using ContactsManagement.Application.Handlers.Contact.GetContactBydId;
using ContactsManagement.Application.Handlers.Contact.GetContatListPaginatedByFilters;
using ContactsManagement.Application.Handlers.Contact.UpdateContactById;
using ContactsManagement.Application.Handlers.User.CreateUser;
using ContactsManagement.Application.Handlers.User.DeleteUser;
using ContactsManagement.Application.Handlers.User.GetUser;
using ContactsManagement.Application.Handlers.User.UpdateUser;
using ContactsManagement.Application.Handlers.User.ValidateUser;
using ContactsManagement.Application.Interfaces.Auth;
using ContactsManagement.Application.Interfaces.Contact.CreateContact;
using ContactsManagement.Application.Interfaces.Contact.DeleteContactById;
using ContactsManagement.Application.Interfaces.Contact.GetContactBydId;
using ContactsManagement.Application.Interfaces.Contact.GetContatListPaginatedByFilters;
using ContactsManagement.Application.Interfaces.Contact.UpdateContactById;
using ContactsManagement.Application.Interfaces.User.CreateUser;
using ContactsManagement.Application.Interfaces.User.DeleteUser;
using ContactsManagement.Application.Interfaces.User.GetUser;
using ContactsManagement.Application.Interfaces.User.UpdateUser;
using ContactsManagement.Application.Interfaces.User.ValidateUser;
using ContactsManagement.Infrastructure.Crypto;
using ContactsManagement.Infrastructure.Middlewares;
using ContactsManagement.Infrastructure.Services.Contact;
using ContactsManagement.Infrastructure.Settings;
using ContactsManagement.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ContactsManagement.Infrastructure.HealthCheck;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ContactsManagement.Api
{
    internal class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        internal void ConfigureService(IServiceCollection services)
        {
            var crypto = new CryptoService();
            services.AddSingleton<ICryptoService>(crypto);

            services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy("Application is running"), tags: new[] { "app" });
            services.AddHealthChecks().AddCheck("sqlserver", new SqlConnectionHealthCheck(this.Configuration, crypto), tags: new[] {"db"});



            var jwtSettings = this.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

            ArgumentNullException.ThrowIfNull(jwtSettings);
            ArgumentNullException.ThrowIfNull(jwtSettings.SecretKey);

            services.Configure<JwtSettings>(this.Configuration.GetSection("JwtSettings"));

            ConfigureAuthentication(services, jwtSettings.SecretKey);



            ConfigureDatabaseService(services, this.Configuration);
            services.AddEndpointsApiExplorer();

            ConfigureSwagger(services);
            ConfigureHandleServices(services);
            ConfigureContactServices(services);

        }

        private void ConfigureContactServices(IServiceCollection services)
        {
            services.AddScoped<IContactService, ContactService>();
        }

        static public void ConfigureHandleServices(IServiceCollection services)
        {
            services.AddScoped<ICreateContactHandler, CreateContactHandler>();
            services.AddScoped<IGetContactBydIdHandler, GetContactBydIdHandler>();
            services.AddScoped<IDeleteContactByIdHandler, DeleteContactByIdHandler>();
            services.AddScoped<IUpdateContactByIdHandler, UpdateContactByIdHandler>();
            services.AddScoped<IGetContactListPaginatedByFiltersHandler, GetContatListPaginatedByFiltersHandler>();

            services.AddScoped<IPasswordHandler, PasswordHandler>();
            services.AddScoped<ITokenHandler, Application.Handlers.Auth.TokenHandler>();
            services.AddScoped<ICreateUserHandler, CreateUserHandler>();
            services.AddScoped<IDeleteUserHandler, DeleteUserHandler>();
            services.AddScoped<IGetUserByIdHandler, GetUserByIdHandler>();
            services.AddScoped<IGetUserByNameHandler, GetUserByNameHandler>();
            services.AddScoped<IGetUserListHandler, GetUserListHandler>();
            services.AddScoped<IUpdateUserHandler, UpdateUserHandler>();
            services.AddScoped<IValidateUserHandler, ValidateUserHandler>();
        }

        public static void ConfigureSwagger(IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Contact Manager API",
                    Version = "v1",
                    Description = "Manager contact API - FIAP Students Project"
                });

                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insert token following the pattern: \"Bearer your_authentication_token\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                    },
                    Array.Empty<string>()
                }
            });
                setup.EnableAnnotations();
            });
        }

        static public void ConfigureDatabaseService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITech1Database, Tech1Database>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        static public void ConfigureAuthentication(IServiceCollection services, string key)
        {
            var decryptedKey = new CryptoService().Decrypt(key);

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(decryptedKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {

            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<RequestCounterMiddleware>();

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");

                endpoints.MapHealthChecks("/healthcheck/app", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("app")
                });

                endpoints.MapHealthChecks("/healthcheck/db", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("db")
                });
            });

            app.UseMetricServer();
        }
    }
}