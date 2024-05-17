﻿using FluentValidation;
using MensaGymnazium.IntranetGen3.Contracts.ModelMetadata;

namespace MensaGymnazium.IntranetGen3.Contracts;

public record SubjectDto : SubjectListItemDto
{
	public string Description { get; set; }

	public bool CanRegisterRepeatedly { get; set; }

	public class SubjectValidator : AbstractValidator<SubjectDto>
	{
		public SubjectValidator()
		{
			RuleFor(x => x.Name).NotEmpty().MaximumLength(SubjectMetadata.NameMaxLength).WithName("Název");
			RuleFor(x => x.Description).NotEmpty().MaximumLength(SubjectMetadata.DescriptionMaxLength).WithName("Popis");
			RuleFor(x => x.CategoryId).NotEmpty().WithName("Kategorie");
			RuleFor(x => x.ScheduleDayOfWeek).NotEmpty().WithName("Den");
			RuleFor(x => x.ScheduleSlotInDay).NotEmpty().WithName("Čas");
			RuleFor(x => x.HoursPerWeek).Must(BeWithinHoursPerWeekRange).WithName("Hodiny");
			RuleFor(x => x.MinStudentsToOpen).Must(BeMoreThanZero).WithName("Minimální počet účastníků pro otevření");
		}

		private bool BeMoreThanZero(int arg)
		{
			return arg > 0;
		}


		private bool BeWithinHoursPerWeekRange(int arg)
		{
			return arg is > 0 and < 5;
		}
	}
}
