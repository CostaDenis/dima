using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Dima.Api.Common.Endpoints.Categories;

public class GetCategoryByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id}", HandleAsync)
            .WithName("Categories: Get By Id")
            .WithSummary("Recupera categoria.")
            .WithDescription("Recupera categoria.")
            // .WithOrder(3)
            .Produces<Response<Category?>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] ICategoryHandler handler,
        [FromRoute] long id)
    {
        var request = new GetCategoryByIdRequest()
        {
            UserId = "TestUser",
            Id = id
        };

        var result = await handler.GetByIdAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
