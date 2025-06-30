using DAL.EF.DbCreating;
using DAL.EF.Entities;
using DAL.EF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Repositories
{
    public class CompanieRepository : GenericRepository<Companie>, ICompanieRepository
    {
        private readonly SummerDbContext _context;

        public CompanieRepository(SummerDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Companie>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .Include(c => c.ActivityType)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public override async Task<Companie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .Include(c => c.ActivityType)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}
