using BLL.DTO.ResponseDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validations.Response
{
    public class ResponseUpdateDtoValidation : AbstractValidator<ResponseUpdateDto>
    {
        public ResponseUpdateDtoValidation() {
            RuleFor(p => p.Status)
                .IsInEnum().WithMessage("Invalid response status");
        }

    }
}
