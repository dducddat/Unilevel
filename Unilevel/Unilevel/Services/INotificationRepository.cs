using Unilevel.Models;

namespace Unilevel.Services
{
    public interface INotificationRepository
    {
        public Task AddNotificationAsync(AddNotification notification, string userId);

        public Task<List<UserIdAndFullName>> GetListUserAsync();

        public Task<List<ListNotification>> GetAllNotificationCreatedAsync(string userId);

        public Task<List<ListNotification>> GetAllNotificationReceivedAsync(string userId);
    }
}
