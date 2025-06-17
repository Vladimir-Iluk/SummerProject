using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Entities
{
    public class Worker
    {
        public Guid Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Qualification { get; set; }
        public string Email{ get; set; }
        public string ExpectedSalary { get; set; }
        public string OtherInfo { get; set; }
        public Guid ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; }
        public ICollection<Response> Responses { get; set; }
        public ICollection<Agreement> Agreements { get; set; }
    }
}
