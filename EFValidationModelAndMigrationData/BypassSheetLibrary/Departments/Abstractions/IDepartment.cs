using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BypassSheetLibrary.BypassSheetClasses;
using BypassSheetLibrary.Departments.Implements;

namespace BypassSheetLibrary.Departments.Abstractions
{
	public interface IDepartment : IEquatable<IDepartment>
	{
		int Id { get; }
		bool IsCycle { get; }
		IList<StampsHistoryElement> StampsHistory { get; }

		Task<int> ExecuteDepartmentRuleAsync(BypassSheet bypassSheet);
	}
}
