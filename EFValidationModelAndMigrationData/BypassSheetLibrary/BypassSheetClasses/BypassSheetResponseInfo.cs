using System.Collections.Generic;

namespace BypassSheetLibrary.BypassSheetClasses
{
	#region Class: BypassResponseInfo

	public class BypassSheetResponseInfo
	{
		#region Properties: Public

		public IEnumerable<IEnumerable<int>> Stamps { get; }
		public BypassSheetResponseState State { get; }

		#endregion

		#region Constructors: Public

		public BypassSheetResponseInfo(IEnumerable<IEnumerable<int>> stamps, BypassSheetResponseState state)
		{
			Stamps = stamps;
			State = state;
		}

		#endregion
	}

	#endregion

	#region Enum: BypassSheetResponseState

	public enum BypassSheetResponseState
	{
		Single,
		Several,
		InfinityCycle,
		NotVisited
	}

	#endregion
}
