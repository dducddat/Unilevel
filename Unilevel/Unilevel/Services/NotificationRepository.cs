using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly UnilevelContext _context;

        private readonly IMapper _mapper;

        public NotificationRepository(UnilevelContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddNotificationAsync(AddNotification notification, string userId)
        {
            await _context.AddAsync(new Notification
            {
                Title = notification.Title,
                Description = notification.Description,
                RecipientUserId = notification.UserId,
                CreateByUserId = userId,
                CreateDate = DateTime.Now,
                View = false
            });
            await _context.SaveChangesAsync();
        }

        public async Task<List<ListNotification>> GetAllNotificationCreatedOrReceivedAsync(string userId, bool created)
        {
            List<Notification> notifications = new List<Notification>();

            if (created)
            {
                notifications = await _context.Notifications
                .Include(n => n.CreateByUser)
                .Where(n => n.CreateByUserId == userId)
                .OrderByDescending(n => n.CreateDate)
                .ToListAsync();
            }
            else
            {
                notifications = await _context.Notifications
                .Include(n => n.CreateByUser)
                .Where(n => n.RecipientUserId == userId)
                .OrderByDescending(n => n.CreateDate)
                .ToListAsync();
            }

            return _mapper.Map<List<ListNotification>>(notifications);
        }

        public async Task<List<UserIdAndNameAndEmail>> GetListUserAsync()
        {
            var users = await _context.Users.Where(u => u.Status == true).ToListAsync();

            return _mapper.Map<List<UserIdAndNameAndEmail>>(users);
        }

        public async Task<List<NewNotification>> GetNewNotificationReceivedAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.CreateByUser)
                .Where(n => n.RecipientUserId == userId)
                .Where(n => n.View == false)
                .OrderByDescending(n => n.CreateDate)
                .Take(10)
                .ToListAsync();

            return _mapper.Map<List<NewNotification>>(notifications);
        }

        public async Task<NotificationDetail> GetNotificationByIdAsync(int id, bool view)
        {
            var notification = await _context.Notifications.Include(n => n.CreateByUser).SingleOrDefaultAsync(n => n.Id == id);

            if (notification is null)
                throw new Exception("Not found");

            if (view)
            {
                notification.View = true;

                _context.Update(notification);
                await _context.SaveChangesAsync();
            }    

            return _mapper.Map<NotificationDetail>(notification);
        }

        public async Task RemoveNotificationAsync(int id)
        {
            var notification = await _context.Notifications.SingleOrDefaultAsync(n => n.Id == id);

            if (notification is null)
                throw new Exception("Not found");

            _context.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
