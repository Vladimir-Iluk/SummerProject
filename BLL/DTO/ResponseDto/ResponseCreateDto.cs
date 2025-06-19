using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.ResponseDto
{
    public class ResponseCreateDto
    {
        public Guid WorkerId { get; set; }
        public Guid VacancyId { get; set; }
    }
}
