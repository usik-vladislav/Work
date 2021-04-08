using System.Threading.Tasks;
using BypassSheetLibrary.ConfigurationsClasses;
using BypassSheetLibrary.Departments.Implements;
using Xunit;

namespace BypassSheetLibrary.Tests
{
	public class ConditionDepartmentRuleTest
	{
		[Theory]
		[InlineData(1, 5, 2, 5, 3, 4, 1)]
		[InlineData(2, 5, 2, 5, 3, 4, 1)]
		[InlineData(1, 5, 0, 0, 3, 0, 0)]
		[InlineData(2, 5, 0, 0, 3, 0, 0)]
		public async Task CheckEquivalenceConstrictors(int conditionStampId, int onConditionNextDepartmentId,
			int onConditionAddedStampId, int onConditionRemovedStampId, int elseConditionNextDepartmentId,
			int elseConditionAddedStampId, int elseConditionRemovedStampId)
		{
			//Arrange
			var bypassSheetDefault = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var bypassSheetConfig = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);

			var ruleDefault = new ConditionDepartmentRule(conditionStampId, onConditionNextDepartmentId,
				elseConditionNextDepartmentId, onConditionAddedStampId, onConditionRemovedStampId,
				elseConditionAddedStampId, elseConditionRemovedStampId);
			var ruleConfig = new ConditionDepartmentRule(new ConditionDepartmentRuleConfig
			{
				ConditionStampId = conditionStampId,
				OnConditionNextDepartmentId = onConditionNextDepartmentId,
				OnConditionAddedStampId = onConditionAddedStampId,
				OnConditionRemovedStampId = onConditionRemovedStampId,
				ElseConditionNextDepartmentId = elseConditionNextDepartmentId,
				ElseConditionAddedStampId = elseConditionAddedStampId,
				ElseConditionRemovedStampId = elseConditionRemovedStampId
			});

			//Act
			var next1 = await ruleDefault.ExecuteRuleAsync(bypassSheetDefault).ConfigureAwait(false);
			var next2 = await ruleConfig.ExecuteRuleAsync(bypassSheetConfig).ConfigureAwait(false);

			//Assert
			Assert.Equal(next1, next2);
			Assert.True(bypassSheetDefault.Stamps.SetEquals(bypassSheetConfig.Stamps));
		}

		[Fact]
		public async Task ExecuteOnConditionRule()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var expectedStamps = BypassSheetTestHelper.GetInitializedBypassSheet(1, 2, 3).Stamps;

			var rule = new ConditionDepartmentRule(new ConditionDepartmentRuleConfig
			{
				ConditionStampId = 3,
				OnConditionNextDepartmentId = 4,
				OnConditionAddedStampId = 2,
				OnConditionRemovedStampId = 5,
				ElseConditionNextDepartmentId = 3,
				ElseConditionAddedStampId = 4,
				ElseConditionRemovedStampId = 1
			});

			//Act
			var nextId = await rule.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.Equal(4, nextId);
			Assert.True(expectedStamps.SetEquals(bypassSheet.Stamps));
		}

		[Fact]
		public async Task ExecuteElseConditionRule()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var expectedStamps = BypassSheetTestHelper.GetInitializedBypassSheet(5, 4, 3).Stamps;

			var rule = new ConditionDepartmentRule(new ConditionDepartmentRuleConfig
			{
				ConditionStampId = 2,
				OnConditionNextDepartmentId = 4,
				OnConditionAddedStampId = 2,
				OnConditionRemovedStampId = 5,
				ElseConditionNextDepartmentId = 3,
				ElseConditionAddedStampId = 4,
				ElseConditionRemovedStampId = 1
			});

			//Act
			var nextId = await rule.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.Equal(3, nextId);
			Assert.True(expectedStamps.SetEquals(bypassSheet.Stamps));
		}
	}
}
