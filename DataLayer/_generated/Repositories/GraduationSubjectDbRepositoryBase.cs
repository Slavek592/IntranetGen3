﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;

namespace MensaGymnazium.IntranetGen3.DataLayer.Repositories;

[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
public abstract class GraduationSubjectDbRepositoryBase : DbRepository<MensaGymnazium.IntranetGen3.Model.GraduationSubject>
{
	protected GraduationSubjectDbRepositoryBase(IDbContext dbContext, IEntityKeyAccessor<MensaGymnazium.IntranetGen3.Model.GraduationSubject, int> entityKeyAccessor, IDataLoader dataLoader, ISoftDeleteManager softDeleteManager, IEntityCacheManager entityCacheManager, IRepositoryQueryProvider repositoryQueryProvider)
		: base(dbContext, entityKeyAccessor, dataLoader, softDeleteManager, entityCacheManager, repositoryQueryProvider)
	{
	}

}