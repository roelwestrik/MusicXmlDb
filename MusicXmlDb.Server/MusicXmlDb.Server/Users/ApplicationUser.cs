using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MusicXmlDb.Server.Users;

public class ApplicationUser
{
    public string Id { get; }

    public string? UserName { get; }

    public string? Email { get; }


    public ApplicationUser(string id, string? userName, string? email)
    {
        Id = id;
        UserName = userName;
        Email = email;
    }


    public static ApplicationUser? CreateLoggedInUser(ClaimsPrincipal principal)
    {
        if (!principal.Identity?.IsAuthenticated ?? true)
        {
            return null;
        }

        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if(id is null)
        {
            return null;
        }

        var userName = principal.FindFirstValue("name");
        var email = principal.FindFirstValue(ClaimTypes.Email);
        if ((userName is null) && (email is null))
        {
            return null;
        }

        return new ApplicationUser(id, userName, email);
    }
}