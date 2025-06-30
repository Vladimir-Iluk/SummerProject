using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.AgreementDto
{
    public class AgreementCreateDto
    {
        [Required(ErrorMessage = "ID працівника є обов'язковим")]
        public Guid WorkerId { get; set; }

        [Required(ErrorMessage = "ID компанії є обов'язковим")]
        public Guid CompanieId { get; set; }

        [Required(ErrorMessage = "Позиція є обов'язковою")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Довжина позиції повинна бути від 2 до 100 символів")]
        public string Position { get; set; }

        [Range(0, 100, ErrorMessage = "Комісія повинна бути в межах від 0 до 100")]
        public decimal Commission { get; set; }
    }
}
