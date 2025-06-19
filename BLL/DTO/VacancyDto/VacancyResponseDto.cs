using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.VacancyDto
{
    public class VacancyResponseDto
    {
        public Guid Id { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public decimal Salary { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsOpen { get; set; }
        public Guid CompanieId { get; set; }
        public string CompanyName { get; set; }
    }
}
