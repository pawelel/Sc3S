namespace Sc3S.CQRS.Queries;

public class UserSession
{
    public string UserName { get; set; } = string.Empty;
<<<<<<< HEAD
    public string Role { get; set; } = string.Empty;
}
=======
    public List<string> Roles { get; set; } = new();
}
>>>>>>> 9324f5ea5a31ced57ebbea41ef3ebf749d2db1e7
