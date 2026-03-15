using Microsoft.AspNetCore.Identity;

namespace Dima.Api.Models;

public class ApplicationUser : IdentityUser<long>
{
    public List<IdentityRole<long>>? Roles { get; set; }
}
