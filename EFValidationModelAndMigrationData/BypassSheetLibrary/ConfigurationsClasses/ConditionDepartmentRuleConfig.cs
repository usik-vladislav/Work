namespace BypassSheetLibrary.ConfigurationsClasses
{
	public class ConditionDepartmentRuleConfig
	{
		public int ConditionStampId { get; set; }
		public int OnConditionAddedStampId { get; set; }
		public int OnConditionRemovedStampId { get; set; }
		public int OnConditionNextDepartmentId { get; set; }
		public int ElseConditionAddedStampId { get; set; }
		public int ElseConditionRemovedStampId { get; set; }
		public int ElseConditionNextDepartmentId { get; set; }
	}
}
