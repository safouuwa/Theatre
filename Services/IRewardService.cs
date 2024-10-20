using StarterKit.Models;
using System;

namespace StarterKit.Services
{
    public interface IRewardService
    {

        bool IsSpecialOccasion(DateTime reservationDate);
        RewardDetails ApplySpecialOccasionRewards(string customerEmail, DateTime reservationDate);
        Customer RefreshCustomer(string email, string password);

    }
}