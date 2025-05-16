using Microsoft.EntityFrameworkCore;
using SRPM.Data.Entities;

namespace SRPM.Data.Repositories
{
    public class ResearchTopicRepository : IResearchTopicRepository
    {
        private readonly ApplicationDbContext _context;

        public ResearchTopicRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResearchTopic?> GetByIdAsync(int id)
        {
            return await _context.ResearchTopics
                .Include(rt => rt.CreatedBy)
                .FirstOrDefaultAsync(rt => rt.Id == id);
        }

        public async Task<IEnumerable<ResearchTopic>> GetAllAsync()
        {
            return await _context.ResearchTopics
                .Include(rt => rt.CreatedBy)
                .ToListAsync();
        }

        public async Task<IEnumerable<ResearchTopic>> GetActiveAsync()
        {
            return await _context.ResearchTopics
                .Include(rt => rt.CreatedBy)
                .Where(rt => rt.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<ResearchTopic>> GetByCreatedByIdAsync(int userId)
        {
            return await _context.ResearchTopics
                .Where(rt => rt.CreatedById == userId)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(ResearchTopic researchTopic)
        {
            _context.ResearchTopics.Add(researchTopic);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(ResearchTopic researchTopic)
        {
            researchTopic.UpdatedAt = DateTime.UtcNow;
            _context.ResearchTopics.Update(researchTopic);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var researchTopic = await _context.ResearchTopics.FindAsync(id);
            if (researchTopic == null)
                return false;

            _context.ResearchTopics.Remove(researchTopic);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
