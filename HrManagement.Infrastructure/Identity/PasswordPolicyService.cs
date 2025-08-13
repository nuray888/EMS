using System;
using System.Threading.Tasks;
using HrManagement.Domain.Entities;

namespace HrManagement.Infrastructure.Identity
{
    public interface IPasswordPolicyService
    {
        bool IsPasswordChangeRequired(ApplicationUser user);
    }

    public class PasswordPolicyService : IPasswordPolicyService
    {
        public bool IsPasswordChangeRequired(ApplicationUser user)
        {
            var daysSinceChange = (DateTimeOffset.UtcNow - user.LastPasswordChangeAt).TotalDays;
            return daysSinceChange >= 30;
        }
    }
}