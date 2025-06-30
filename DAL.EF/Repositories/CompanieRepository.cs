using DAL.EF.DbCreating;
using DAL.EF.Entities;
using DAL.EF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Repositories
{
    public class CompanieRepository : GenericRepository<Companie>, ICompanieRepository
    {
        public CompanieRepository(SummerDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Companie>> GetAllAsync(
            Func<IQueryable<Companie>, IQueryable<Companie>>? include = null,
            Expression<Func<Companie, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            return await base.GetAllAsync(include, filter, cancellationToken);
        }

        public override async Task<Companie?> GetByIdAsync(
            Guid id,
            Func<IQueryable<Companie>, IQueryable<Companie>>? include = null,
            CancellationToken cancellationToken = default)
        {
            return await base.GetByIdAsync(id, include, cancellationToken);
        }
    }
}
