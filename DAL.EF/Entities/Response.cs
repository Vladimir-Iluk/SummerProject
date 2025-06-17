using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Entities
{
    public class Response
    {
        public Guid Id { get; set; }
        public DataSetDateTime SentAt { get; set; }
        public ResponseStatus Status { get; set; }
        public Guid WorkerId { get; set; }
        public Guid VacancyId { get; set; }
        public Worker Worker { get; set; }
        public Vacancy Vacancy { get; set; }
    }
    public enum ResponseStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
