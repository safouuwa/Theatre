using StarterKit.Models;
namespace StarterKit.Services;

public interface ILoginService {
    public LoginStatus CheckPassword(string username, string inputPassword);
    public Customer GetCustomerAccount(string username, string password);
}