using BLL.DTO.VacancyDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validations.Vacancy
{
    public class VacancyUpdateDtoValidation : AbstractValidator<VacancyUpdateDto>
    {
        public VacancyUpdateDtoValidation() {
            RuleFor(p => p.IsOpen)
               .NotNull().WithMessage("IsOpen cannot be null");
        }
    }
}
