using System.Data.Common;
using StarterKit.Models;
using StarterKit.Utils;

namespace StarterKit.Services
{
    public class PointService : IPointService
    {
        private readonly DatabaseContext context;
        public readonly ILoginService loginservice;
        public PointService(DatabaseContext _context, ILoginService loginService)
        {
            context = _context;
            loginservice = loginService;
        }
        public bool GiftPoints(string fromEmail, string toEmail, int amount)
        {
            if (!context.Customer.Any(x => x.Email == toEmail)) return false;
            context.Customer.First(x => x.Email == fromEmail).Points -= amount;
            context.Customer.First(x => x.Email == toEmail).Points += amount;
            context.SaveChanges();
            return true;
        }
        public Customer RefreshCustomer(string email, string password) => context.Customer.First(x => x.Email == email && x.Password == password);
    }
}