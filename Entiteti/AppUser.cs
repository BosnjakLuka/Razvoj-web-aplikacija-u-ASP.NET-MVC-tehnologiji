using Microsoft.AspNetCore.Identity;

namespace planinarenje.Entiteti;

public class AppUser : IdentityUser
{
    public string OIB { get; set; } = string.Empty;

    public string JMBG { get; set; } = string.Empty;
}
