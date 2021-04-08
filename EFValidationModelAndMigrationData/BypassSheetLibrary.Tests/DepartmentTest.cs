using System.Collections.Generic;
using System.Threading.Tasks;
using BypassSheetLibrary.Departments.Implements;
using Xunit;

namespace BypassSheetLibrary.Tests
{
	public class DepartmentTest
	{
		[Fact]
		public async Task CheckInfinityCycle()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var history = new StampsHistoryElement
			{
				NextDepartmentId = 2,
				Stamps = new HashSet<int> { 3, 2, 1, 5 }
			};

			var ruleAdd = new DefaultDepartmentRule(2, 2);
			var department = new Department(2, ruleAdd);
			department.StampsHistory.Add(history);

			//Assert IsCycle - false
			Assert.False(department.IsCycle);


			//Act execute check cycle step
			await department.ExecuteDepartmentRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert IsCycle - true
			Assert.True(department.IsCycle);


			//Arrange old history
			var oldHistoryElementCount = department.StampsHistory.Count;

			//Act repeat rule after finding cycle

			await department.ExecuteDepartmentRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert history don't changed
			Assert.Equal(oldHistoryElementCount, department.StampsHistory.Count);
		}
	}
}
