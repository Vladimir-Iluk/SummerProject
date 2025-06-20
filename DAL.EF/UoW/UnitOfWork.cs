using DAL.EF.DbCreating;
using DAL.EF.Repositories;
using DAL.EF.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SummerDbContext context;
        public UnitOfWork(SummerDbContext context)
        {
            this.context = context;
        }

        public IActivityTypeRepository ActivityTypes => new ActivityTypeRepository(context);

        public IAgreementRepository Agreements => new AgreementRepository(context);

        public ICompanieRepository Companies => new CompanieRepository(context);

        public IResponseRepository Responses => new ResponseRepository(context);

        public IVacancyRepository Vacancy => new VacancyRepository(context);

        public IWorkerRepository Workers => new WorkerRepository(context);

        public void Dispose()
        {
         context.Dispose();
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
