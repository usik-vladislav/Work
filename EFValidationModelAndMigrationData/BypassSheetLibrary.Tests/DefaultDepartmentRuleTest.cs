using System.Threading.Tasks;
using BypassSheetLibrary.ConfigurationsClasses;
using BypassSheetLibrary.Departments.Implements;
using Xunit;

namespace BypassSheetLibrary.Tests
{
	public class DefaultDepartmentRuleTest
	{
		[Fact]
		public async Task ExecuteRuleNextDepartmentId()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var ruleAdd = new DefaultDepartmentRule(2);

			//Act
			var nextId = await ruleAdd.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.Equal(2, nextId);
		}

		[Fact]
		public async Task ExecuteRuleNothing()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var expectedStamps = BypassSheetTestHelper.GetInitializedBypassSheet(1, 3, 5).Stamps;

			var ruleAdd = new DefaultDepartmentRule(2);

			//Act
			await ruleAdd.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.True(expectedStamps.SetEquals(bypassSheet.Stamps));
		}

		[Fact]
		public async Task ExecuteRuleAdd()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var expectedStamps = BypassSheetTestHelper.GetInitializedBypassSheet(1, 2, 3, 5).Stamps;

			var ruleAdd = new DefaultDepartmentRule(2, 2);

			//Act
			await ruleAdd.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.True(expectedStamps.SetEquals(bypassSheet.Stamps));
		}

		[Fact]
		public async Task ExecuteRuleExistAdd()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 2, 1, 5);
			var expectedStamps = BypassSheetTestHelper.GetInitializedBypassSheet(1, 2, 3, 5).Stamps;

			var ruleAdd = new DefaultDepartmentRule(2, 2);

			//Act
			await ruleAdd.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.True(expectedStamps.SetEquals(bypassSheet.Stamps));
		}

		[Fact]
		public async Task ExecuteRuleRemove()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var expectedStamps = BypassSheetTestHelper.GetInitializedBypassSheet(1, 5).Stamps;

			var ruleAdd = new DefaultDepartmentRule(2, 0, 3);

			//Act
			await ruleAdd.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.True(expectedStamps.SetEquals(bypassSheet.Stamps));
		}

		[Fact]
		public async Task ExecuteRuleNotExistRemove()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(1, 5);
			var expectedStamps = BypassSheetTestHelper.GetInitializedBypassSheet(1, 5).Stamps;

			var ruleAdd = new DefaultDepartmentRule(2, removedStampId: 3);

			//Act
			await ruleAdd.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.True(expectedStamps.SetEquals(bypassSheet.Stamps));
		}

		[Fact]
		public async Task ExecuteRuleAddAndRemove()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var expectedStamps = BypassSheetTestHelper.GetInitializedBypassSheet(1, 2, 5).Stamps;

			var ruleAdd = new DefaultDepartmentRule(2, 2, 3);

			//Act
			await ruleAdd.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.True(expectedStamps.SetEquals(bypassSheet.Stamps));
		}

		[Fact]
		public async Task ExecuteRuleConflictAddAndRemove()
		{
			//Arrange
			var bypassSheet = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var expectedStamps = BypassSheetTestHelper.GetInitializedBypassSheet(1, 3, 5).Stamps;

			var ruleAdd = new DefaultDepartmentRule(2, 4, 4);

			//Act
			await ruleAdd.ExecuteRuleAsync(bypassSheet).ConfigureAwait(false);

			//Assert
			Assert.True(expectedStamps.SetEquals(bypassSheet.Stamps));
		}

		[Fact]
		public async Task CheckEquivalenceConstructors ()
		{
			//Arrange
			var bypassSheetDefault = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);
			var bypassSheetConfig = BypassSheetTestHelper.GetInitializedBypassSheet(3, 1, 5);

			var ruleAddDefault = new DefaultDepartmentRule(2, 2, 5);
			var ruleAddConfig = new DefaultDepartmentRule(new DefaultDepartmentRuleConfig
			{
				NextDepartmentId = 2,
				AddedStampId = 2,
				RemovedStampId = 5
			});

			//Act
			var next1 = await ruleAddDefault.ExecuteRuleAsync(bypassSheetDefault).ConfigureAwait(false);
			var next2 = await ruleAddConfig.ExecuteRuleAsync(bypassSheetConfig).ConfigureAwait(false);

			//Assert
			Assert.Equal(next1, next2);
			Assert.True(bypassSheetDefault.Stamps.SetEquals(bypassSheetConfig.Stamps));
		}
	}
}
