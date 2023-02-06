using Unilevel.Models;

namespace Unilevel.Services
{
    public interface INotificationRepository
    {
        public Task AddNotificationAsync(AddNotification notification, string userId);

        public Task<List<UserIdAndNameAndEmail>> GetListUserAsync();

        public Task<List<ListNotification>> GetAllNotificationCreatedOrReceivedAsync(string userId, bool created);

        public Task<List<NewNotification>> GetNewNotificationReceivedAsync(string userId);

        public Task<NotificationDetail> GetNotificationByIdAsync(int id, bool view);

        public Task RemoveNotificationAsync(int id, string userId);
    }
}
