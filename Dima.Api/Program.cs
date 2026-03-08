using Dima.Api.Data;
using Dima.Api.Handlers;
using Dima.Core.Enums;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
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
var app = builder.Build();

//Gera a tela do swagger
app.UseSwagger();
app.UseSwaggerUI();


app.MapGet(
        "/v1/categories",
        async (ICategoryHandler handler) =>
        {
            var request = new GetAllCategoriesRequest() { UserId = "teste@gmail.com"};
            return await handler.GetAllAsync(request);
        }
    )
    .WithName("Categories: Get All")
    .WithSummary("Retorna todas as categorias de usuário.")
    .Produces<PagedResponse<List<Category>?>>();

app.MapGet(
        "/v1/categories/{id}",
        async (long id,
            ICategoryHandler handler) =>
        {
            var request = new GetCategoryByIdRequest { Id = id , UserId = "teste@gmail.com"};
            return await handler.GetByIdAsync(request);
        }
    )
    .WithName("Categories: Get By Id")
    .WithSummary("Retorna categoria.")
    .Produces<Response<Category?>>();

app.MapPost(
    "/v1/categories", async (CreateCategoryRequest request,
                ICategoryHandler handler) =>
                    await handler.CreateAsync(request)
            )
        .WithName("Categories: Create")
        .WithSummary("Cria uma nova categoria.")
        .Produces<Response<Category?>>();

app.MapPut(
        "/v1/categories/{id}",
        async (long id,
            UpdateCategoryRequest request,
            ICategoryHandler handler) =>
        {
            request.Id = id;
            return await handler.UpdateAsync(request);
        }
    )
    .WithName("Categories: Update")
    .WithSummary("Atualiza categoria.")
    .Produces<Response<Category?>>();

app.MapDelete(
        "/v1/categories/{id}",
        async (long id,
            ICategoryHandler handler) =>
        {
            var request = new DeleteCategoryRequest { Id = id };
            return await handler.DeleteAsync(request);
        }
    )
    .WithName("Categories: Delete")
    .WithSummary("Exclui categoria.")
    .Produces<Response<Category?>>();


app.Run();
