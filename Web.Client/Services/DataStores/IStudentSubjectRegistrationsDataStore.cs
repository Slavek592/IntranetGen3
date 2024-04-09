﻿using Havit.Blazor.Components.Web.Services.DataStores;
using MensaGymnazium.IntranetGen3.Contracts;

namespace MensaGymnazium.IntranetGen3.Web.Client.Services.DataStores;

public interface IStudentSubjectRegistrationsDataStore : IDictionaryStaticDataStore<int, StudentSubjectRegistrationDto>
{
	/// <summary>
	/// Use this to invalidate the cache
	/// </summary>
	public void RegistrationsChanged();
}