using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BypassSheetLibrary.BypassSheetClasses;
using BypassSheetLibrary.Departments.Abstractions;

namespace BypassSheetLibrary.Departments.Implements
{
	public class Department : IDepartment
	{
		#region Properties: Private

		private IDepartmentRule Rule { get; }

		#endregion

		#region Properties: Public

		public int Id { get; }
		public bool IsCycle { get; private set; }
		public IList<StampsHistoryElement> StampsHistory { get; }

		#endregion

		#region Constructors: Public

		public Department(int id, IDepartmentRule rule)
		{
			Id = id;
			StampsHistory = new List<StampsHistoryElement>();
			Rule = rule;
		}

		#endregion

		#region Methods: Private

		private void FindCycle(BypassSheet bypassSheet, int nextId)
		{
			IsCycle = StampsHistory.Any(e => 
				e.Stamps.SetEquals(bypassSheet.Stamps) && e.NextDepartmentId == nextId);

			if (!IsCycle)
			{
				StampsHistory.Add(new StampsHistoryElement
				{
					Stamps = new HashSet<int>(bypassSheet.Stamps),
					NextDepartmentId = nextId
				});
			}
		}

		#endregion

		#region Methods: Public

		public async Task<int> ExecuteDepartmentRuleAsync(BypassSheet bypassSheet)
		{
			var nextId = await Rule.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			if (!IsCycle)
			{
				await Task.Run(() => FindCycle(bypassSheet, nextId)).ConfigureAwait(false);
			}

			return nextId;
		}

		#endregion

		public bool Equals(IDepartment other)
		{
			if (other == null || !(other is Department otherDepartment)) return false;
			return Id == other.Id && Rule.Equals(otherDepartment.Rule);
		}
	}
}
