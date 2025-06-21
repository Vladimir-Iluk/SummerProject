using BLL.DTO.VacancyDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validations.Vacancy
{
    public class VacancyCreateDtoValidation : AbstractValidator<VacancyCreateDto>
    {
        public VacancyCreateDtoValidation() {
            RuleFor(p => p.Position)
               .NotEmpty().WithMessage("Position cannot be empty")
               .MaximumLength(50).WithMessage("Position cannot be longer than 50 characters");

            RuleFor(p => p.Description)
                .MaximumLength(240).WithMessage("Description cannot be longer than 240 characters");

            RuleFor(p => p.Salary)
                .GreaterThan(0).WithMessage("Salary must be greater than 0");

            RuleFor(p => p.CompanieId)
                .NotEmpty().WithMessage("Company ID cannot be empty");
        }
    }
}
