using System.Runtime.Serialization;

namespace BypassSheetLibrary.JsonModels
{
	[DataContract]
	public class JsonConfiguration
	{
		[DataMember]
		public int StartId { get; set; }

		[DataMember]
		public int EndId { get; set; }

		[DataMember]
		public JsonDepartmentRule[] DepartmentRules { get; set; }
	}
}
