using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.AgreementDto
{
    public   class AgreementUpdateDto
    {
        public Guid WorkerId { get; set; }
        public Guid CompanieId { get; set; }
        public string Position { get; set; }
        public decimal Commission { get; set; }
    }
}
