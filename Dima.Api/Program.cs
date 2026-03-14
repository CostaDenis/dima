using Dima.Api.Data;
using Dima.Api.Endpoints;
using Dima.Api.Handlers;
using Dima.Core.Handlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection") ?? string.Empty;

//Adiciona o DbContext ao builder
builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(connectionString);
});

builder.Services.AddEndpointsApiExplorer(); //Adiciona o suporte ao OpenApi
builder.Services.AddSwaggerGen(x =>
{
    x.CustomSchemaIds(n => n.FullName);  //Pega o namespace inteiro do request
}); //Gera Html/Css/Js para API

builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();//Dada a interface, esse vai ser o retorno
builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
var app = builder.Build();

//Gera a tela do swagger
app.UseSwagger();
app.UseSwaggerUI();
app.MapEndpoints();
app.MapGet("/", () => new { message = "Ok"});  //Health

app.Run();
