using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.ActivityTypeDto
{
    public class ActivityTypeCreateDto
    {
        [Required(ErrorMessage = "Назва типу активності є обов'язковою")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва типу активності повинна містити від 2 до 100 символів")]
        public string ActivityName { get; set; }
    }
}
