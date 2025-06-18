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
    public class ActivityTypeRepository : GenericRepository<ActivityType>, IActivityTypeRepository
    {
        public ActivityTypeRepository(SummerDbContext context) : base(context)
        {
        }
    }
}
