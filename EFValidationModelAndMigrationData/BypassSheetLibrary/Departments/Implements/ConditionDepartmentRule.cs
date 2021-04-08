using System.Threading.Tasks;
using BypassSheetLibrary.BypassSheetClasses;
using BypassSheetLibrary.ConfigurationsClasses;
using BypassSheetLibrary.Departments.Abstractions;

namespace BypassSheetLibrary.Departments.Implements
{
	public class ConditionDepartmentRule : IDepartmentRule
	{
		#region Properties: Private

		private int ConditionStampId { get; }

		private IDepartmentRule OnConditionDefaultRule { get; }

		private IDepartmentRule ElseConditionDefaultRule { get; }

		#endregion

		#region Constructors: Public

		public ConditionDepartmentRule(ConditionDepartmentRuleConfig config)
			: this(config.ConditionStampId, config.OnConditionNextDepartmentId,
				config.ElseConditionNextDepartmentId, config.OnConditionAddedStampId, config.OnConditionRemovedStampId,
				config.ElseConditionAddedStampId, config.ElseConditionRemovedStampId)
		{

		}

		public ConditionDepartmentRule(int conditionStampId, int onConditionNextDepartmentId,
			int elseConditionNextDepartmentId, int onConditionAddedStampId = 0, int onConditionRemovedStampId = 0, 
			int elseConditionAddedStampId = 0, int elseConditionRemovedStampId = 0)
		{
			ConditionStampId = conditionStampId;
			OnConditionDefaultRule = new DefaultDepartmentRule(onConditionNextDepartmentId,
				onConditionAddedStampId, onConditionRemovedStampId);
			ElseConditionDefaultRule = new DefaultDepartmentRule(elseConditionNextDepartmentId,
				elseConditionAddedStampId, elseConditionRemovedStampId);
		}

		#endregion

		public async Task<int> ExecuteRuleAsync(BypassSheet bypassSheet)
		{
			if (bypassSheet.Stamps.Contains(ConditionStampId))
			{
				return await OnConditionDefaultRule.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);
			}

			return await ElseConditionDefaultRule.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);
		}

		public bool Equals(IDepartmentRule other)
		{
			if (other == null || !(other is ConditionDepartmentRule otherRule)) return false;
			return ConditionStampId == otherRule.ConditionStampId
			       && OnConditionDefaultRule.Equals(otherRule.OnConditionDefaultRule)
			       && ElseConditionDefaultRule.Equals(otherRule.ElseConditionDefaultRule);
		}
	}
}
