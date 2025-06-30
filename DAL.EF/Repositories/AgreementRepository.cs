using DAL.EF.DbCreating;
using DAL.EF.Entities;
using DAL.EF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;

namespace DAL.EF.Repositories
{
    public class AgreementRepository : GenericRepository<Agreement>, IAgreementRepository
    {
        public AgreementRepository(SummerDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Agreement>> GetAllAsync(
            Func<IQueryable<Agreement>, IQueryable<Agreement>>? include = null,
            Expression<Func<Agreement, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            return await base.GetAllAsync(
                query => (include != null ? include(query) : query)
                    .Include(a => a.Worker)
                    .Include(a => a.Companie),
                filter,
                cancellationToken);
        }

        public override async Task<Agreement?> GetByIdAsync(
            Guid id,
            Func<IQueryable<Agreement>, IQueryable<Agreement>>? include = null,
            CancellationToken cancellationToken = default)
        {
            return await base.GetByIdAsync(
                id,
                query => (include != null ? include(query) : query)
                    .Include(a => a.Worker)
                    .Include(a => a.Companie),
                cancellationToken);
        }
    }
}
