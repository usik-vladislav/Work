using System.Collections.Generic;

namespace BypassSheetLibrary.BypassSheetClasses
{
	public class BypassSheet
	{
		#region Properties: Public

		public HashSet<int> Stamps { get; }

		#endregion

		#region Constructors: Public

		public BypassSheet()
		{
			Stamps = new HashSet<int>();
		}

		#endregion
	}
}
