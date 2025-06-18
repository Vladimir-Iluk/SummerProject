using DAL.EF.DbCreating;
using DAL.EF.Entities;
using DAL.EF.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Repositories
{
    public class CompanieRepository : GenericRepository<Companie>, ICompanieRepository
    {
        public CompanieRepository(SummerDbContext context) : base(context) { }
    }
}
