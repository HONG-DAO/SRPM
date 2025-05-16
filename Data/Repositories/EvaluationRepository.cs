using Microsoft.EntityFrameworkCore;
using SRPM.Data.Entities;

namespace SRPM.Data.Repositories
{
    public class EvaluationRepository : IEvaluationRepository
    {
        private readonly ApplicationDbContext _context;

        public EvaluationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Evaluation?> GetByIdAsync(int id)
        {
            return await _context.Evaluations
                .Include(e => e.Project)
                .Include(e => e.Task)
                .Include(e => e.EvaluatedBy)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Evaluation>> GetAllAsync()
        {
            return await _context.Evaluations
                .Include(e => e.Project)
                .Include(e => e.Task)
                .Include(e => e.EvaluatedBy)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evaluation>> GetByProjectIdAsync(int projectId)
        {
            return await _context.Evaluations
                .Include(e => e.Task)
                .Include(e => e.EvaluatedBy)
                .Where(e => e.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evaluation>> GetByTaskIdAsync(int taskId)
        {
            return await _context.Evaluations
                .Include(e => e.Project)
                .Include(e => e.EvaluatedBy)
                .Where(e => e.TaskId == taskId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evaluation>> GetByEvaluatedByIdAsync(int userId)
        {
            return await _context.Evaluations
                .Include(e => e.Project)
                .Include(e => e.Task)
                .Where(e => e.EvaluatedById == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evaluation>> GetByTypeAsync(string type)
        {
            return await _context.Evaluations
                .Include(e => e.Project)
                .Include(e => e.Task)
                .Include(e => e.EvaluatedBy)
                .Where(e => e.Type == type)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(Evaluation evaluation)
        {
            _context.Evaluations.Add(evaluation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Evaluation evaluation)
        {
            evaluation.UpdatedAt = DateTime.UtcNow;
            _context.Evaluations.Update(evaluation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var evaluation = await _context.Evaluations.FindAsync(id);
            if (evaluation == null)
                return false;

            _context.Evaluations.Remove(evaluation);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
