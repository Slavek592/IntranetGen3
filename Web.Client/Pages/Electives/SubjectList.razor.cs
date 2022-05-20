﻿using Havit.Collections;
using MensaGymnazium.IntranetGen3.Contracts;
using MensaGymnazium.IntranetGen3.Primitives;
using MensaGymnazium.IntranetGen3.Web.Client.Services;
using MensaGymnazium.IntranetGen3.Web.Client.Services.DataStores;
using Microsoft.AspNetCore.Components;

namespace MensaGymnazium.IntranetGen3.Web.Client.Pages.Electives;

public partial class SubjectList
{
	[Inject] protected IHxMessengerService Messenger { get; set; }
	[Inject] protected Func<ISubjectFacade> SubjectFacade { get; set; }
	[Inject] protected Func<ISubjectRegistrationsManagerFacade> SubjectRegistrationsManagerFacade { get; set; }
	[Inject] protected NavigationManager NavigationManager { get; set; }
	[Inject] protected ISubjectCategoriesDataStore SubjectCategoriesDataStore { get; set; }
	[Inject] protected ISubjectTypesDataStore SubjectTypesDataStore { get; set; }
	[Inject] protected ITeachersDataStore TeachersDataStore { get; set; }
	[Inject] protected IGradesDataStore GradesDataStore { get; set; }
	[Inject] protected IClientAuthService ClientAuthService { get; set; }

	private SubjectListQueryFilter subjectListFilter = new SubjectListQueryFilter()
	{
		SigningRuleId = LastSigningRuleId
	};
	private HxGrid<SubjectListItemDto> subjectsGrid;
	private SubjectListItemDto subjectSelected;
	private SubjectEdit subjectEditComponent;
	private List<SigningRuleWithRegistrationsDto> studentRegistrations;

	private static int? LastSigningRuleId { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await SubjectCategoriesDataStore.EnsureDataAsync();
		await SubjectTypesDataStore.EnsureDataAsync();
		await TeachersDataStore.EnsureDataAsync();
		await GradesDataStore.EnsureDataAsync();

		if ((await ClientAuthService.GetCurrentClaimsPrincipal()).IsInRole(nameof(Role.Student)))
		{
			studentRegistrations = await SubjectRegistrationsManagerFacade().GetCurrentUserSigningRulesWithRegistrationsAsync(Dto.FromValue((int?)null));
		}
	}

	protected bool IsStudentRegistered(int subjectId, StudentRegistrationType registrationType)
	{
		if (studentRegistrations == null)
		{
			return false;
		}
		return studentRegistrations
			.Where(ssr => (this.subjectListFilter.SigningRuleId is null) || (ssr.Id == this.subjectListFilter.SigningRuleId))
			.SelectMany(sr => sr.Registrations)
			.Any(r => (r.SubjectId == subjectId) && (r.RegistrationType == registrationType));
	}

	protected string GetRowCssClass(SubjectListItemDto item)
	{
		if (IsStudentRegistered(item.Id, StudentRegistrationType.Main))
		{
			return "reg-main";
		}
		else if (IsStudentRegistered(item.Id, StudentRegistrationType.Secondary))
		{
			return "reg-secondary";
		}

		return null;
	}

	private async Task<GridDataProviderResult<SubjectListItemDto>> LoadSubjects(GridDataProviderRequest<SubjectListItemDto> request)
	{
		var subjectListRequest = new DataFragmentRequest<SubjectListQueryFilter>()
		{
			Filter = subjectListFilter,
			StartIndex = request.StartIndex,
			Count = request.Count,
			Sorting = request.Sorting?.Select(s => new SortItem(s.SortString, s.SortDirection)).ToArray()
		};

		var subjectListResult = await SubjectFacade().GetSubjectListAsync(subjectListRequest, request.CancellationToken);

		return new()
		{
			Data = subjectListResult.Data ?? new(),
			TotalCount = subjectListResult.TotalCount
		};
	}

	private async Task HandleSigningRuleFilterChanged(int? newSigningRuleFilterValue)
	{
		subjectListFilter.SigningRuleId = newSigningRuleFilterValue;
		LastSigningRuleId = newSigningRuleFilterValue;
		await subjectsGrid.RefreshDataAsync();
	}

	private Task HandleSelectedDataItemChanged(SubjectListItemDto selection)
	{
		subjectSelected = selection;
		NavigationManager.NavigateTo(Routes.Electives.GetSubjectDetail(selection.Id));
		return Task.CompletedTask;
	}

	private async Task HandleDeleteItemClicked(SubjectListItemDto subject)
	{
		await SubjectFacade().DeleteSubjectAsync(Dto.FromValue(subject.Id));
		Messenger.AddInformation(subject.Name, "Předmět smazán.");
		await subjectsGrid.RefreshDataAsync();
	}

	private async Task HandleFilterModelChanged(SubjectListQueryFilter newFilterModel)
	{
		subjectListFilter = newFilterModel;
		await subjectsGrid.RefreshDataAsync();
	}

	private async Task HandleNewItemClicked()
	{
		await subjectEditComponent.ShowAsync();
	}

	private async Task HandleSubjectEditSaved()
	{
		await subjectsGrid.RefreshDataAsync();
	}

	private string GetSubjectTypes(List<int> subjectTypesIds)
	{
		return String.Join(", ", subjectTypesIds.Select(id => SubjectTypesDataStore.GetByKey(id)?.Name))
			.Trim(',', ' ');
	}

	private string GetGrades(List<int> gradeIds)
	{
		return String.Join(", ", gradeIds.OrderBy(id => -id).Select(id => GradesDataStore.GetByKey(id)?.Name))
			.Trim(',', ' ');
	}

	private string GetTeachers(List<int> teacherIds)
	{
		return String.Join(", ", teacherIds.Select(id => TeachersDataStore.GetByKeyOrDefault(id)?.Name))
			.Trim(',', ' ');
	}
}
