using SRPM.API.Models;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Services
{
    public interface IFundingRequestService
    {
        Task<FundingRequestDto?> GetByIdAsync(int id);
        Task<IEnumerable<FundingRequestDto>> GetAllAsync();
        Task<IEnumerable<FundingRequestDto>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<FundingRequestDto>> GetByRequestedByIdAsync(int userId);
        Task<IEnumerable<FundingRequestDto>> GetByStatusAsync(string status);
        Task<FundingRequestDto?> CreateAsync(CreateFundingRequestRequest request, int userId);
        Task<bool> UpdateAsync(int id, UpdateFundingRequestRequest request, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<bool> ApproveAsync(int id, ApproveFundingRequestRequest request, int userId);
        Task<bool> RejectAsync(int id, RejectFundingRequestRequest request, int userId);
    }
}
