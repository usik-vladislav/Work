using System.Collections.Generic;
using BypassSheetLibrary.Departments.Abstractions;

namespace BypassSheetLibrary.ConfigurationsClasses
{
	public class OrganizationConfiguration
	{
		#region Public: Properties

		public IEnumerable<IDepartment> Departments { get; }
		public int StartDepartmentId { get; }
		public int EndDepartmentId { get; }

		#endregion

		#region Constructors: Public

		public OrganizationConfiguration(int startDepartmentId, int endDepartmentId,
			IEnumerable<IDepartment> departments)
		{
			StartDepartmentId = startDepartmentId;
			EndDepartmentId = endDepartmentId;
			Departments = departments;
		}

		#endregion
	}
}
