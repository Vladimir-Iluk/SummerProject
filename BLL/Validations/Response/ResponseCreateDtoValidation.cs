using BLL.DTO.ResponseDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validations.Response
{
    public class ResponseCreateDtoValidation : AbstractValidator<ResponseCreateDto>
    {
        public ResponseCreateDtoValidation() {
            RuleFor(p => p.WorkerId)
                .NotEmpty().WithMessage("Worker ID cannot be empty");

            RuleFor(p => p.VacancyId)
                .NotEmpty().WithMessage("Vacancy ID cannot be empty");
        }

    }
}
