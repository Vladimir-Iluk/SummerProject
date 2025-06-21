using BLL.DTO.AgreementDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validations.Agreement
{
    public class AgreementCreateDtoValidation : AbstractValidator<AgreementCreateDto>
    {
        public AgreementCreateDtoValidation() {
            RuleFor(p => p.WorkerId).NotEmpty().WithMessage("Worker id is required!");
            RuleFor(p => p.CompanieId).NotEmpty().WithMessage("Companie id is required!");
            RuleFor(p => p.Position).NotEmpty().WithMessage("Position cannot be empty")
                .MaximumLength(50).WithMessage("Position lenght cannot be bigger than 50 symblos");
            RuleFor(p => p.Commission)
            .GreaterThan(0).WithMessage("Commission must be greater than 0")
            .LessThan(100).WithMessage("Commission cannot be greater than 100");
        }

    }
}
