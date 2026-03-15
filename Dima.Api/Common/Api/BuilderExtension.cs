using Dima.Api.Data;
using Dima.Api.Handlers;
using Dima.Api.Models;
using Dima.Core;
using Dima.Core.Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Common.Api;

public static class BuilderExtension
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        Configuration.ConnectionString = builder.Configuration
            .GetConnectionString("DefaultConnection") ?? string.Empty;

        Configuration.FrontendUrl = builder.Configuration.GetValue<string>("FrontendUrl")
                                    ?? string.Empty;
        Configuration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl")
                                   ?? string.Empty;;
    }

    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer(); //Adiciona o suporte ao OpenApi
        builder.Services.AddSwaggerGen(x =>
        {
            x.CustomSchemaIds(n => n.FullName);  //Pega o namespace inteiro do request
        }); //Gera Html/Css/Js para API
    }

    public static void AddSecurity(this WebApplicationBuilder builder)
    {
        //Sempre nessa ordem
        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies(); //Quem é
        builder.Services.AddAuthorization(); //O que pode fazer
    }

    public static void AddDataContexts(this WebApplicationBuilder builder)
    {
        //Adiciona o DbContext ao builder
        builder.Services.AddDbContext<AppDbContext>(x =>
        {
            x.UseSqlServer(Configuration.ConnectionString);
        });
        builder.Services    //Adiciona suporte ao Identity Framework
            .AddIdentityCore<ApplicationUser>()   //User customizado do app
            .AddRoles<IdentityRole<long>>()
            .AddEntityFrameworkStores<AppDbContext>() //Usa o EF para armazenar essas informações
            .AddApiEndpoints();  //Já cria tudo na api para o Identity
    }

    public static void AddCrossOrigin(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options => options.AddPolicy(
            ApiConfiguration.CorsPolicyName,
            policy => policy
                .WithOrigins([
                    Configuration.BackendUrl,
                    Configuration.FrontendUrl
                ])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
        ));
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();//Dada a interface, esse vai ser o retorno
        builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
    }
}
