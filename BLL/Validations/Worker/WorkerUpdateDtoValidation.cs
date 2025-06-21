using BLL.DTO.WorkerDto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validations.Worker
{
    public class WorkerUpdateDtoValidation : AbstractValidator<WorkerUpdateDto>
    {
        public WorkerUpdateDtoValidation() {

            RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty")
                .MaximumLength(50).WithMessage("Last name cannot be longer than 50 characters");

            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("First name cannot be empty")
                .MaximumLength(50).WithMessage("First name cannot be longer than 50 characters");

            RuleFor(p => p.MiddleName)
                .MaximumLength(50).WithMessage("Middle name cannot be longer than 50 characters");

            RuleFor(p => p.Qualification)
                .MaximumLength(50).WithMessage("Qualification cannot be longer than 50 characters");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("Email cannot be empty")
                .MaximumLength(50).WithMessage("Email cannot be longer than 50 characters");

            RuleFor(p => p.ExpectedSalary)
                .MaximumLength(50).WithMessage("Expected salary cannot be longer than 50 characters");

            RuleFor(p => p.OtherInfo)
                .MaximumLength(240).WithMessage("Other info cannot be longer than 240 characters");

            RuleFor(p => p.ActivityTypeId)
                .NotEmpty().WithMessage("Activity type ID cannot be empty");
        }
    }
}
