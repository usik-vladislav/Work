using System.Threading.Tasks;
using BypassSheetLibrary.BypassSheetClasses;
using BypassSheetLibrary.ConfigurationsClasses;
using BypassSheetLibrary.Departments.Abstractions;

namespace BypassSheetLibrary.Departments.Implements
{
	public class DefaultDepartmentRule : IDepartmentRule
	{
		#region Properties: Private

		private int AddedStampId { get; }

		private int RemovedStampId { get; }

		private int NextDepartmentId { get; }

		#endregion

		#region Constructors: Public

		public DefaultDepartmentRule(DefaultDepartmentRuleConfig config)
			: this(config.NextDepartmentId, config.AddedStampId, config.RemovedStampId)
		{
		}

		public DefaultDepartmentRule(int nextDepartmentId, int addedStampId = 0, int removedStampId = 0)
		{
			AddedStampId = addedStampId;
			RemovedStampId = removedStampId;
			NextDepartmentId = nextDepartmentId;
		}

		#endregion

		#region Methods: Private

		private int ExecuteInternalRule(BypassSheet bypassSheet)
		{
			if (AddedStampId != 0)
			{
				bypassSheet.Stamps.Add(AddedStampId);
			}

			if (RemovedStampId != 0)
			{
				bypassSheet.Stamps.RemoveWhere(stamp => stamp.Equals(RemovedStampId));
			}

			return NextDepartmentId;
		}

		#endregion

		#region Methods: Public

		public async Task<int> ExecuteRuleAsync(BypassSheet bypassSheet)
		{
			return await Task.Run(() => ExecuteInternalRule(bypassSheet)).ConfigureAwait(false);
		}

		#endregion

		public bool Equals(IDepartmentRule other)
		{
			if (other == null || !(other is DefaultDepartmentRule otherRule)) return false;
			return NextDepartmentId == otherRule.NextDepartmentId && AddedStampId == otherRule.AddedStampId
			                                                      && RemovedStampId == otherRule.RemovedStampId;
		}
	}
}
