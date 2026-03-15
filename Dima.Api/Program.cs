using System.Security.Claims;
using Dima.Api.Data;
using Dima.Api.Endpoints;
using Dima.Api.Handlers;
using Dima.Api.Models;
using Dima.Core.Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); //Adiciona o suporte ao OpenApi
builder.Services.AddSwaggerGen(x =>
{
    x.CustomSchemaIds(n => n.FullName);  //Pega o namespace inteiro do request
}); //Gera Html/Css/Js para API

//Sempre nessa ordem
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies(); //Quem é
builder.Services.AddAuthorization(); //O que pode fazer


var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection") ?? string.Empty;

//Adiciona o DbContext ao builder
builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(connectionString);
});
builder.Services    //Adiciona suporte ao Identity Framework
    .AddIdentityCore<ApplicationUser>()   //User customizado do app
    .AddRoles<IdentityRole<long>>()
    .AddEntityFrameworkStores<AppDbContext>() //Usa o EF para armazenar essas informações
    .AddApiEndpoints();  //Já cria tudo na api para o Identity


builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();//Dada a interface, esse vai ser o retorno
builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

//Gera a tela do swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapEndpoints();
app.MapGet("/", () => new { message = "Ok"});  //Health
app.MapGroup("v1/identity")
    .WithTags("Identity")
    .MapIdentityApi<ApplicationUser>();

app.MapGroup("v1/identity")
    .WithTags("Identity")
    .MapPost("/logout", async (
     SignInManager<ApplicationUser> signInManager) =>
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    })
    .RequireAuthorization();

app.MapGroup("v1/identity")
    .WithTags("Identity")
    .MapGet("/roles", (
        //Com o userManager desce no banco para pegar esses roles
        /* UserManager<ApplicationUser> userManager */
        ClaimsPrincipal user /*Usuário logado no momento */ ) =>
    {
        if (user.Identity is null
            || !user.Identity.IsAuthenticated)
            return Results.Unauthorized();

        var identity = (ClaimsIdentity)user.Identity;
        var roles = identity.FindAll(ClaimTypes.Role)
            .Select(c => new
            {
              c.Issuer,
              c.OriginalIssuer,
              c.Type,
              c.Value,
              c.ValueType,
            });

        return TypedResults.Json(roles);
    })
    .RequireAuthorization();

app.Run();
