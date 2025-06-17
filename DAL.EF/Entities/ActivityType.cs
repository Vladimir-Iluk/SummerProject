using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Entities
{
    public class ActivityType
    {
        public Guid Id { get; set; }
        public string ActivityName { get; set; }
        public ICollection<Worker> Workers  { get; set;}
        public ICollection<Companie> Companies { get; set;}
    }
}
