using System.Runtime.InteropServices;
using System.Security.Claims;
using Dima.Api.Common.Api;
using Dima.Core;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Dima.Api.Endpoints.Orders;

public class GetAllProductsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("", HandleAsync)
            .WithName("Products: Get All")
            .WithSummary("Pega todos os produtos")
            .WithDescription("Pega todos os produtos")
            .Produces<PagedResponse<List<Order>?>>();

    private static async Task<IResult> HandleAsync(
        IProductHandler handler,
        [FromQuery] int pageSize = Configuration.DefaultPageSize,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber
    )
    {
        var request = new GetAllProductsRequest
        {
            PageSize = pageSize,
            PageNumber = pageNumber
        };
        
        var result = await handler.GetAllAsync(request);
        
        return result.IsSuccess 
            ? TypedResults.Ok(result) 
            : TypedResults.BadRequest(result);
    }
}