using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Entities
{
    public class Companie
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string EmailCompany {  get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Guid ActivityTypeId { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
        public ICollection<Agreement> Aggreements { get; set; }
    }
}
