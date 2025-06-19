using DAL.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.ResponseDto
{
    public class ResponseResponseDto
    {
        public Guid Id { get; set; }
        public DateTime SentAt { get; set; }
        public ResponseStatus Status { get; set; }
        public Guid WorkerId { get; set; }
        public string WorkerFullName { get; set; }
        public Guid VacancyId { get; set; }
        public string Position { get; set; }
    }
}
