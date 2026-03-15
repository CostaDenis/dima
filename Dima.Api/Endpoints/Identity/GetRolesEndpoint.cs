using System.Security.Claims;
using Dima.Api.Common.Api;

namespace Dima.Api.Endpoints.Identity;

public class GetRolesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/logout", Handle)
            .RequireAuthorization();

    public static Task<IResult> Handle(ClaimsPrincipal user)
    {
        if (user.Identity is null
            || !user.Identity.IsAuthenticated)
            return Task.FromResult(Results.Unauthorized());

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

        return Task.FromResult<IResult>(TypedResults.Json(roles));
    }
}
