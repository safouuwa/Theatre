using StarterKit.Models;
namespace StarterKit.Services
{
    public interface IPointService
    {
        public bool GiftPoints(string fromEmail, string toEmail, int email);
        public Customer RefreshCustomer(string email, string password);
    }
}