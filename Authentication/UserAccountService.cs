namespace Sc3S.Authentication;

public class UserAccountService
{
    private readonly List<UserAccount> _users;



    public UserAccountService()
    {
        _users = new List<UserAccount>
        {
            new UserAccount { UserName = "admin", Password = "admin", Role = "Admin" },
            new UserAccount { UserName = "user", Password = "user", Role = "User" },
            new UserAccount { UserName = "manager", Password = "manager", Role = "Manager" }
        };
    }

    public bool ValidateUser(string userName, string password)
    {
        bool isValid = false;

        foreach (var user in _users)
        {
            if (user.UserName == userName && user.Password == password)
            {
                isValid = true;
                break;
            }
        }

        return isValid;
    }
    public UserAccount? GetByUserName(string userName)
    {
        return _users.FirstOrDefault(u => u.UserName == userName);
    }

    public string GetRole(string userName)
    {
        var user = GetByUserName(userName);
        return user?.Role??string.Empty;
    }
}
