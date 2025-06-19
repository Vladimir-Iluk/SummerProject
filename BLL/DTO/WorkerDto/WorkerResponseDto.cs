using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.WorkerDto
{
    public class WorkerResponseDto
    {
        public Guid Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Qualification { get; set; }
        public string Email { get; set; }
        public string ExpectedSalary { get; set; }
        public string OtherInfo { get; set; }
        public Guid ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }
    }
}
