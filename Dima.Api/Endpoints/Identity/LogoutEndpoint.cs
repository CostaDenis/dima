using Dima.Api.Common.Api;
using Dima.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Dima.Api.Endpoints.Identity;

public class LogoutEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/logout", HandleAsync)
            .RequireAuthorization();

    public static async Task<IResult> HandleAsync(SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
}
