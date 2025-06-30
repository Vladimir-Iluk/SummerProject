using DAL.EF.DbCreating;
using DAL.EF.Entities;
using DAL.EF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.EF.Repositories
{
    public class WorkerRepository : GenericRepository<Worker>, IWorkerRepository
    {
        private readonly SummerDbContext _context;

        public WorkerRepository(SummerDbContext context)
            : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Worker>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Workers
                .Include(w => w.ActivityType)
                .ToListAsync(cancellationToken);
        }

        public override async Task<Worker> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Workers
                .Include(w => w.ActivityType)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        }
    }
}
