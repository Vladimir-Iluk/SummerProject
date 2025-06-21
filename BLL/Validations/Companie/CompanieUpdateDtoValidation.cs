using BLL.DTO.CompanieDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validations.Companie
{
    public class CompanieUpdateDtoValidation : AbstractValidator<CompanieUpdateDto>
    {
        public CompanieUpdateDtoValidation() {
            RuleFor(p => p.CompanyName).NotEmpty().WithMessage("Company name cannot be emptye")
                .MaximumLength(50).WithMessage("Company name cannot be bigger than 50 symbols");
            RuleFor(p => p.EmailCompany).NotEmpty().WithMessage("Comapny email cannot be empty")
                .MaximumLength(50).WithMessage("Maximum lenght 50 symbols");
            RuleFor(p => p.Address).NotEmpty().WithMessage("Address cannot be empty")
                .MaximumLength(100).WithMessage("Adrress lenght maximum 100");
            RuleFor(p => p.Phone).NotEmpty().WithMessage("Phone cannot be empty")
                .MaximumLength(50).WithMessage("Maximum lenght phone 50");
            RuleFor(p => p.ActivityTypeId).NotEmpty().WithMessage("Activity type ID cannot be empty");

        }
    }
}
