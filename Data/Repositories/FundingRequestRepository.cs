using Microsoft.EntityFrameworkCore;
using SRPM.Data.Entities;

namespace SRPM.Data.Repositories
{
    public class FundingRequestRepository : IFundingRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public FundingRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FundingRequest?> GetByIdAsync(int id)
        {
            return await _context.FundingRequests
                .Include(fr => fr.Project)
                .Include(fr => fr.RequestedBy)
                .FirstOrDefaultAsync(fr => fr.Id == id);
        }

        public async Task<IEnumerable<FundingRequest>> GetAllAsync()
        {
            return await _context.FundingRequests
                .Include(fr => fr.Project)
                .Include(fr => fr.RequestedBy)
                .ToListAsync();
        }

        public async Task<IEnumerable<FundingRequest>> GetByProjectIdAsync(int projectId)
        {
            return await _context.FundingRequests
                .Include(fr => fr.RequestedBy)
                .Where(fr => fr.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FundingRequest>> GetByRequestedByIdAsync(int userId)
        {
            return await _context.FundingRequests
                .Include(fr => fr.Project)
                .Where(fr => fr.RequestedById == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FundingRequest>> GetByStatusAsync(string status)
        {
            return await _context.FundingRequests
                .Include(fr => fr.Project)
                .Include(fr => fr.RequestedBy)
                .Where(fr => fr.Status == status)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(FundingRequest fundingRequest)
        {
            _context.FundingRequests.Add(fundingRequest);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(FundingRequest fundingRequest)
        {
            fundingRequest.UpdatedAt = DateTime.UtcNow;
            _context.FundingRequests.Update(fundingRequest);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var fundingRequest = await _context.FundingRequests.FindAsync(id);
            if (fundingRequest == null)
                return false;

            _context.FundingRequests.Remove(fundingRequest);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
