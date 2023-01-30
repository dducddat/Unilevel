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
                Remove = false,
                View = false,
                Status = true,
            });
            await _context.SaveChangesAsync();
        }

        public async Task<List<ListNotification>> GetAllNotificationCreatedAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.CreateByUser)
                .Where(n => n.CreateByUserId == userId)
                .Where(n => n.Remove == false)
                .Where(n => n.Status == true)
                .OrderByDescending(n => n.CreateDate)
                .ToListAsync();

            return _mapper.Map<List<ListNotification>>(notifications);
        }

        public async Task<List<ListNotification>> GetAllNotificationReceivedAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.CreateByUser)
                .Where(n => n.RecipientUserId == userId)
                .Where(n => n.Remove == false)
                .Where(n => n.Status == true)
                .OrderByDescending(n => n.CreateDate)
                .ToListAsync();

            return _mapper.Map<List<ListNotification>>(notifications);
        }

        public async Task<List<UserIdAndFullName>> GetListUserAsync()
        {
            var users = await _context.Users.Where(u => u.Status == true).ToListAsync();

            return _mapper.Map<List<UserIdAndFullName>>(users);
        }
    }
}
