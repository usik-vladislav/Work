using System.Collections.Generic;
using System.Linq;
using BypassSheetLibrary.BypassSheetClasses;

namespace BypassSheetLibrary.Tests
{
	public static class BypassSheetTestHelper
	{
		public static BypassSheet GetInitializedBypassSheet(params int[] stamps)
		{
			var bypassSheet = new BypassSheet();
			foreach (var stamp in stamps)
			{
				bypassSheet.Stamps.Add(stamp);
			}
			return bypassSheet;
		}

		public static List<HashSet<int>> SplitStringOnStamps(BypassSheetResponseState state, string stampsString)
		{
			var stamps = stampsString == "" ? new List<HashSet<int>>() : stampsString
				.Split(',')
				.Select(st => st
					.Trim()
					.Split(' ')
					.Select(st2 => int.Parse(st2.Trim()))
					.ToHashSet())
				.ToList();

			if (state != BypassSheetResponseState.NotVisited && stamps.Count == 0)
			{
				stamps.Add(new HashSet<int>());
			}

			return stamps;
		}
	}
}
