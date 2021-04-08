using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BypassSheetLibrary.BypassSheetClasses;
using BypassSheetLibrary.ConfigurationsClasses;
using BypassSheetLibrary.Departments.Abstractions;
using BypassSheetLibrary.Departments.Implements;
using Xunit;

namespace BypassSheetLibrary.Tests
{
	public class OrganizationManagerTest
	{
		private Random _rand;
		private Random Rand => _rand ??= new Random(DateTime.UtcNow.Millisecond);

		private class TestConfiguration
		{
			public int StartId { get; }
			public int EndId { get; }
			public int DepartmentId { get; }
			public BypassSheetResponseState State { get; }

			public List<HashSet<int>> ExpectedStamps { get; }

			public TestConfiguration(int startId, int endId, int departmentId, BypassSheetResponseState state,
				string stampsString)
			{
				StartId = startId;
				EndId = endId;
				DepartmentId = departmentId;
				State = state;
				ExpectedStamps = BypassSheetTestHelper.SplitStringOnStamps(state, stampsString);
			}
		}

		private static List<IDepartmentRule> InitDefaultDepartmentsRules()
		{
			var rule1 = new DefaultDepartmentRule(new DefaultDepartmentRuleConfig
			{
				NextDepartmentId = 2,
				AddedStampId = 6,
				RemovedStampId = 3
			});

			var rule2 = new ConditionDepartmentRule(new ConditionDepartmentRuleConfig
			{
				ConditionStampId = 7,
				OnConditionNextDepartmentId = 3,
				OnConditionAddedStampId = 9,
				ElseConditionNextDepartmentId = 5,
				ElseConditionAddedStampId = 8
			});

			var rule3 = new DefaultDepartmentRule(new DefaultDepartmentRuleConfig
			{
				NextDepartmentId = 4,
				AddedStampId = 3
			});

			var rule4 = new DefaultDepartmentRule(new DefaultDepartmentRuleConfig
			{
				NextDepartmentId = 2,
				AddedStampId = 4
			});

			var rule5 = new ConditionDepartmentRule(new ConditionDepartmentRuleConfig
			{
				ConditionStampId = 8,
				OnConditionNextDepartmentId = 4,
				OnConditionAddedStampId = 7,
				ElseConditionNextDepartmentId = 1,
				ElseConditionAddedStampId = 2
			});

			return new List<IDepartmentRule> { rule1, rule2, rule3, rule4, rule5};
		}

		private static List<IDepartmentRule> InitNotCycleDepartmentsRules()
		{
			var rule1 = new ConditionDepartmentRule(new ConditionDepartmentRuleConfig
			{
				ConditionStampId = 2,
				OnConditionNextDepartmentId = 3,
				ElseConditionNextDepartmentId = 2,
				ElseConditionAddedStampId = 2
			});

			var rule2 = new DefaultDepartmentRule(new DefaultDepartmentRuleConfig
			{
				NextDepartmentId = 1
			});

			var rule3 = new DefaultDepartmentRule(new DefaultDepartmentRuleConfig
			{
				NextDepartmentId = 2
			});

			return new List<IDepartmentRule> { rule1, rule2, rule3 };
		}

		private static TestConfiguration[] GetTestConfigurations()
		{
			return new []
			{
				new TestConfiguration(1, 3, 1, BypassSheetResponseState.Single, "6"),
				new TestConfiguration(1, 3, 2, BypassSheetResponseState.Several, "6 8, 6 8 7 4 9"),
				new TestConfiguration(1, 3, 3, BypassSheetResponseState.Single, "6 8 7 4 9 3"),
				new TestConfiguration(1, 3, 4, BypassSheetResponseState.Single, "6 8 7 4"),
				new TestConfiguration(1, 3, 5, BypassSheetResponseState.Single, "6 8 7"),
				new TestConfiguration(2, 1, 2, BypassSheetResponseState.InfinityCycle, "8, 8 7 4 9, 8 7 4 9 3"),
				new TestConfiguration(2, 1, 3, BypassSheetResponseState.InfinityCycle, "8 7 4 9 3"),
				new TestConfiguration(2, 1, 4, BypassSheetResponseState.InfinityCycle, "4 7 8, 8 7 4 9 3"),
				new TestConfiguration(2, 1, 1, BypassSheetResponseState.NotVisited, "")
			};
		}

		private async Task TestRequestsInConfiguration(int startId, int endId, List<IDepartmentRule> rules,
			List<TestConfiguration> expectedInfoList, int requestsCount)
		{
			var organizationManager = new OrganizationManager();
			await organizationManager.SetConfiguration(startId, endId, rules.Count, rules).ConfigureAwait(false);

			var tasks = new List<Task>();

			for (var i = 0; i < requestsCount; i++)
			{
				var expectedInfo = expectedInfoList[Rand.Next(expectedInfoList.Count)];
				tasks.Add(Task.Run(() => TestRequest(organizationManager, expectedInfo.DepartmentId, 
					expectedInfo.State, expectedInfo.ExpectedStamps)));
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		private static async Task TestRequestInNewConfiguration(int startId, int endId, int departmentId,
			BypassSheetResponseState state, List<HashSet<int>> expectedStamps, List<IDepartmentRule> rules)
		{
			var organizationManager = new OrganizationManager();
			await organizationManager.SetConfiguration(startId, endId, rules.Count, rules).ConfigureAwait(false);
			
			TestRequest(organizationManager, departmentId, state, expectedStamps);
		}

		private static void TestRequest(OrganizationManager organizationManager, int departmentId, BypassSheetResponseState state,
			List<HashSet<int>> expectedStamps)
		{
			var response = organizationManager.RequestBypassSheetInfoByDepartment(departmentId);

			Assert.Equal(state, response.State);

			for (var i = 0; i < expectedStamps.Count; i++)
			{
				Assert.True(expectedStamps[i].SetEquals(response.Stamps.ElementAt(i)));
			}
			Assert.Equal(expectedStamps.Count, response.Stamps.Count());
		}

		[Fact]
		public async Task CheckEquivalenceSettingConfiguration()
		{
			//Arrange
			var startId = 1;
			var endId = 3;
			var rules = InitNotCycleDepartmentsRules();
			var jsonString = @"{
				'StartId': '1',
				'EndId': '3',
				'DepartmentRules': [
					{
						'ConditionStampId': 2,
						'NextDepartmentId': 3,
						'ElseNextDepartmentId': 2,
						'ElseAddedStampId': 2
					},
					{
						'NextDepartmentId': 1
					},
					{
						'NextDepartmentId': 2
					}
				]
			}";

			var organizationManagerDefault = new OrganizationManager();
			var organizationManagerJson = new OrganizationManager();

			//Act
			await organizationManagerDefault.SetConfiguration(startId, endId, rules.Count, rules).ConfigureAwait(false);
			await organizationManagerJson.SetConfiguration(jsonString);

			//Assert
			Assert.Equal(organizationManagerDefault.Configuration.StartDepartmentId,
				organizationManagerJson.Configuration.StartDepartmentId);
			Assert.Equal(organizationManagerDefault.Configuration.EndDepartmentId,
				organizationManagerJson.Configuration.EndDepartmentId);

			var departmentsDefault = organizationManagerDefault.Configuration.Departments.ToList();
			var departmentsJson = organizationManagerJson.Configuration.Departments.ToList();
			for (var i = 0; i < departmentsDefault.Count; i++)
			{
				Assert.True(departmentsDefault[i].Equals(departmentsJson[i]));
			}

			Assert.Equal(departmentsDefault.Count, departmentsJson.Count);
		}

		[Theory]
		[InlineData(1, 3, 1, BypassSheetResponseState.Single, "6")]
		[InlineData(1, 3, 2, BypassSheetResponseState.Several, "6 8, 6 8 7 4 9")]
		[InlineData(1, 3, 3, BypassSheetResponseState.Single, "6 8 7 4 9 3")]
		[InlineData(1, 3, 4, BypassSheetResponseState.Single, "6 8 7 4")]
		[InlineData(1, 3, 5, BypassSheetResponseState.Single, "6 8 7")]
		[InlineData(2, 1, 2, BypassSheetResponseState.InfinityCycle, "8, 8 7 4 9, 8 7 4 9 3")]
		[InlineData(2, 1, 3, BypassSheetResponseState.InfinityCycle, "8 7 4 9 3")]
		[InlineData(2, 1, 4, BypassSheetResponseState.InfinityCycle, "4 7 8, 8 7 4 9 3")]
		[InlineData(2, 1, 1, BypassSheetResponseState.NotVisited, "")]
		public async Task RequestDefaultBypassSheetInfo(int startId, int endId, int departmentId,
			BypassSheetResponseState state, string stampsString)
		{
			var rules = InitDefaultDepartmentsRules();
			var expectedStamps = BypassSheetTestHelper.SplitStringOnStamps(state, stampsString);
			await TestRequestInNewConfiguration(startId, endId, departmentId, state, expectedStamps, rules).ConfigureAwait(false);
		}

		[Theory]
		[InlineData(1, 3, 1, BypassSheetResponseState.Single, "2")]
		[InlineData(2, 2, 1, BypassSheetResponseState.NotVisited, "")]
		[InlineData(2, 2, 2, BypassSheetResponseState.Single, "")]
		public async Task RequestNotCycleBypassSheetInfo(int startId, int endId, int departmentId,
			BypassSheetResponseState state, string stampsString)
		{
			var rules = InitNotCycleDepartmentsRules();
			var expectedStamps = BypassSheetTestHelper.SplitStringOnStamps(state, stampsString);
			await TestRequestInNewConfiguration(startId, endId, departmentId, state, expectedStamps, rules).ConfigureAwait(false);
		}

		[Theory]
		[InlineData(100)]
		[InlineData(1000)]
		[InlineData(10000)]
		[InlineData(100000)]
		[InlineData(1000000)]
		public async Task TestConcurrentRequestInOneConfiguration(int requestCount)
		{
			var rules = InitDefaultDepartmentsRules();
			var configurations = GetTestConfigurations();
			var randomConfiguration = configurations[Rand.Next(configurations.Length)];
			var startId = randomConfiguration.StartId;
			var endId = randomConfiguration.EndId;
			var testedConfigurations = configurations
				.Where(c => c.StartId == startId && c.EndId == endId).ToList();

			await TestRequestsInConfiguration(startId, endId, rules, testedConfigurations, requestCount).ConfigureAwait(false);
		}

		[Theory]
		[InlineData(100, 100)]
		[InlineData(100, 1000)]
		[InlineData(100, 10000)]
		[InlineData(100, 100000)]
		[InlineData(1000, 100)]
		[InlineData(1000, 1000)]
		[InlineData(1000, 10000)]
		[InlineData(10000, 100)]
		[InlineData(10000, 1000)]
		[InlineData(100000, 100)]
		public async Task TestConcurrentRequestInDifferentConfiguration(int configurationCount, int requestCount)
		{
			var tasks = new List<Task>();

			for (var i = 0; i < configurationCount; i++)
			{
				tasks.Add(TestConcurrentRequestInOneConfiguration(requestCount));
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}
	}
}
