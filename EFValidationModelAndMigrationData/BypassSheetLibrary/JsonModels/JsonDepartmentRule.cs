using System.Runtime.Serialization;

namespace BypassSheetLibrary.JsonModels
{
	[DataContract]
	public class JsonDepartmentRule
	{
		[DataMember]
		public int ConditionStampId { get; set; }

		[DataMember]
		public int AddedStampId { get; set; }

		[DataMember]
		public int RemovedStampId { get; set; }

		[DataMember]
		public int NextDepartmentId { get; set; }

		[DataMember]
		public int ElseAddedStampId { get; set; }

		[DataMember]
		public int ElseRemovedStampId { get; set; }

		[DataMember]
		public int ElseNextDepartmentId { get; set; }
	}
}
