using System;
using System.Threading.Tasks;
using BypassSheetLibrary.BypassSheetClasses;

namespace BypassSheetLibrary.Departments.Abstractions
{
	public interface IDepartmentRule : IEquatable<IDepartmentRule>
	{
		Task<int> ExecuteRuleAsync(BypassSheet bypassSheet);
	}
}
