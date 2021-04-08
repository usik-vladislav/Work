using System.Collections.Generic;

namespace BypassSheetLibrary.Departments.Implements
{
	public class StampsHistoryElement
	{
		public int NextDepartmentId { get; set; }
		public HashSet<int> Stamps { get; set; }
	}
}
