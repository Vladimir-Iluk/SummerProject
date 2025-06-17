using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Entities
{
    public class Agreement
    {
        public Guid Id { get; set; }
        public string Position { get; set; }
        public decimal Commission { get; set; }
        public DateTime AgreementDate { get; set; }
        public Guid WorkerId { get; set; }
        public Guid CompanieId { get; set; }
        public Worker Worker { get; set; }
        public Companie Companie { get; set; }
    }
}
