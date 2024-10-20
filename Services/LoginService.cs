using System.Data.Common;
using StarterKit.Models;
using StarterKit.Utils;

namespace StarterKit.Services;

public enum LoginStatus { IncorrectPassword, IncorrectUsername, UserSuccess, AdminSuccess, LoggedOut }

public enum ADMIN_SESSION_KEY { adminLoggedIn }

public class LoginService : ILoginService
{

    private readonly DatabaseContext _context;

    public LoginService(DatabaseContext context)
    {
        _context = context;
    }

    public LoginStatus CheckPassword(string username, string inputPassword)
    {
        // TODO: Make this method check the password with what is in the database
        if (_context.Admin.Any(x => x.UserName == username && x.Password == EncryptionHelper.EncryptPassword(inputPassword))) return LoginStatus.AdminSuccess;
        if (_context.Customer.Any(x => x.Email == username && x.Password == inputPassword)) return LoginStatus.UserSuccess;
        if (_context.Admin.Any(x => x.UserName != username && x.Password == EncryptionHelper.EncryptPassword(inputPassword))) return LoginStatus.IncorrectUsername;
        if (_context.Customer.Any(x => x.Email != username && x.Password == inputPassword)) return LoginStatus.IncorrectUsername;
        return LoginStatus.IncorrectPassword;
    }

    public Customer GetCustomerAccount(string username, string password) => _context.Customer.First(x => x.Email == username && x.Password == password);
}