using BLL.DTO.ActivityTypeDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validations.ActivityType
{
    public class ActivityTypeUpdateDtoValitation : AbstractValidator<ActivityTypeUpdateDto>
    {
        public ActivityTypeUpdateDtoValitation()
        {
            RuleFor(p => p.ActivityName).NotEmpty().WithMessage("Activity name cannot be empty")
                .MaximumLength(50).WithMessage("Activity name cannot be bigger than 50 symbols");

        }
    }
}
