using DAL.EF.DbCreating;
using DAL.EF.Entities;
using DAL.EF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.EF.Repositories
{
    public class WorkerRepository : GenericRepository<Worker>, IWorkerRepository
    {
        public WorkerRepository(SummerDbContext context)
            : base(context)
        {
        }

        public override async Task<IEnumerable<Worker>> GetAllAsync(
            Func<IQueryable<Worker>, IQueryable<Worker>>? include = null,
            Expression<Func<Worker, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            return await base.GetAllAsync(include, filter, cancellationToken);
        }

        public override async Task<Worker?> GetByIdAsync(
            Guid id,
            Func<IQueryable<Worker>, IQueryable<Worker>>? include = null,
            CancellationToken cancellationToken = default)
        {
            return await base.GetByIdAsync(id, include, cancellationToken);
        }
    }
}
