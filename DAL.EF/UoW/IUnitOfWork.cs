using DAL.EF.Entities;
using DAL.EF.Repositories.Interfaces;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        IActivityTypeRepository ActivityTypes { get; }
        IAgreementRepository Agreements { get; }
        ICompanieRepository Companies { get; }
        IResponseRepository Responses { get; }
        IVacancyRepository Vacancy { get; }
        IWorkerRepository Workers { get; }

        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}
