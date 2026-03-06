using Dima.Api.Data;
using Dima.Core.Enums;
using Dima.Core.Models;
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

var app = builder.Build();

//Gera a tela do swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost(
    "/v1/transactions",
    () => "Hello World!")
    // .WithName("Transactions: Create")
    // .WithSummary("Transactions Summary")
    // .Produces<Response>()3
    ;

// public class Request()
// {
//     public string Title { get; set; } = string.Empty;
//     public DateTime CreatedAt { get; set; } = DateTime.Now;
//     public ETransactionType Type { get; set; } = ETransactionType.Withdraw;
//     public decimal Amount { get; set; }
//
//     public long CategoryId { get; set; }
//
//     public string UserId { get; set; } = string.Empty;
// }

// public class Response
// {
//
// }



// class Handler
// {
//     public Response Handle(Request request)
//     {
//         return new Response();
//     }
// }

app.Run();
