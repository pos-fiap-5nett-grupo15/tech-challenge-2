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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using System.Text;

namespace ContactsManagement.Api;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();

        var startup = new Startup(builder.Configuration);
        startup.ConfigureService(builder.Services);
        
        var app = builder.Build();

        startup.Configure(app,app.Environment);

        app.Run();
    }
}