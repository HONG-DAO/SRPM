using SRPM.Data.Entities;

namespace SRPM.Data.Repositories
{
    public interface IFundingRequestRepository
    {
        Task<FundingRequest?> GetByIdAsync(int id);
        Task<IEnumerable<FundingRequest>> GetAllAsync();
        Task<IEnumerable<FundingRequest>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<FundingRequest>> GetByRequestedByIdAsync(int userId);
        Task<IEnumerable<FundingRequest>> GetByStatusAsync(string status);
        Task<bool> CreateAsync(FundingRequest fundingRequest);
        Task<bool> UpdateAsync(FundingRequest fundingRequest);
        Task<bool> DeleteAsync(int id);
    }
}
