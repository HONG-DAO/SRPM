using SRPM.API.Models;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Services
{
    public interface INotificationService
    {
        Task<NotificationDto?> GetByIdAsync(int id);
        Task<IEnumerable<NotificationDto>> GetAllAsync();
        Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId);
        Task<bool> MarkAsReadAsync(int id, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
