using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BypassSheetLibrary.ConfigurationsClasses;
using BypassSheetLibrary.Departments.Abstractions;
using BypassSheetLibrary.Departments.Implements;
using BypassSheetLibrary.JsonModels;
using Newtonsoft.Json;

namespace BypassSheetLibrary.BypassSheetClasses
{
	public class OrganizationManager
	{
		#region Class: StampsComparer

		private class StampsComparer : IEqualityComparer<HashSet<int>>
		{
			public bool Equals(HashSet<int> x, HashSet<int> y)
			{
				if (x == null && y == null) return true;
				if (x == null || y == null) return false;
				return x.SetEquals(y);
			}

			public int GetHashCode(HashSet<int> obj)
			{
				return obj.Sum().GetHashCode();
			}
		}

		#endregion

		#region Properties: Public

		public OrganizationConfiguration Configuration { get; private set; }

		#endregion

		#region Constructors: Public

		public OrganizationManager()
		{
			Configuration = null;
		}

		#endregion

		#region Methods: Private

		private async Task FillBypassSheet(OrganizationConfiguration configuration)
		{
			var bypassSheet = new BypassSheet();
			var oldId = 0;
			for (var departmentId = configuration.StartDepartmentId; oldId != configuration.EndDepartmentId;)
			{
				var department = configuration.Departments.FirstOrDefault(d => d.Id == departmentId);
				if (department == null)
				{
					throw new InvalidDataException("Entered department id not included in available identifiers!");
				}

				if (department.IsCycle) break;

				oldId = departmentId;
				departmentId = await department.ExecuteDepartmentRuleAsync(bypassSheet);
			}

			Configuration = configuration;
		}

		private BypassSheetResponseInfo GetBypassInfoByDepartment(IDepartment department)
		{
			var stamps = department.StampsHistory
				.Select(h => h.Stamps)
				.Distinct(new StampsComparer())
				.ToList();
			var state = stamps.Count == 0 ? BypassSheetResponseState.NotVisited 
				: Configuration.Departments.Any(d => d.IsCycle) ? BypassSheetResponseState.InfinityCycle
				: stamps.Count > 1 ? BypassSheetResponseState.Several
				: BypassSheetResponseState.Single;
			return new BypassSheetResponseInfo(stamps, state);
		}

		private IDepartmentRule ConvertJsonDepartmentRule(JsonDepartmentRule jsonDepartmentRule)
		{
			if (jsonDepartmentRule.ConditionStampId == 0)
			{
				return new DefaultDepartmentRule( 
					jsonDepartmentRule.NextDepartmentId,
					jsonDepartmentRule.AddedStampId,
					jsonDepartmentRule.RemovedStampId);
			}
			return new ConditionDepartmentRule(
				jsonDepartmentRule.ConditionStampId,
				jsonDepartmentRule.NextDepartmentId,
				jsonDepartmentRule.ElseNextDepartmentId,
				jsonDepartmentRule.AddedStampId,
				jsonDepartmentRule.RemovedStampId,
				jsonDepartmentRule.ElseAddedStampId,
				jsonDepartmentRule.ElseRemovedStampId);
		}

		#endregion

		#region Methods: Public

		public async Task SetConfiguration(string jsonConfiguration)
		{
			var configuration = JsonConvert.DeserializeObject<JsonConfiguration>(jsonConfiguration);
			if (configuration == null)
			{
				throw new ArgumentException($"Wrong format of json, parameterName - \"{nameof(jsonConfiguration)}\"");
			}
			var rules = configuration.DepartmentRules.Select(ConvertJsonDepartmentRule).ToList();
			await SetConfiguration(configuration.StartId, configuration.EndId, rules.Count, rules).ConfigureAwait(false);
		}

		public async Task SetConfiguration(int startDepartment, int endDepartment, int departmentsCount,
			IEnumerable<IDepartmentRule> orderedRulesList)
		{
			var departmentRules = orderedRulesList.ToList();
			if (departmentRules.Count != departmentsCount)
			{
				throw new ArgumentException("Count of rules not equal count of departments!");
			}

			if (startDepartment > departmentsCount || endDepartment > departmentsCount)
			{
				throw new ArgumentException("Entered department id not included in available identifiers!");
			}

			var departments = departmentRules
				.Select((rule, index) => new Department(index + 1, rule))
				.ToList();

			var configuration = new OrganizationConfiguration(startDepartment, endDepartment, departments);
			await FillBypassSheet(configuration).ConfigureAwait(false);
		}

		public BypassSheetResponseInfo RequestBypassSheetInfoByDepartment(int departmentId)
		{
			if (Configuration == null)
			{
				throw new Exception("Configuration is not initialized!");
			}

			var department = Configuration.Departments.FirstOrDefault(d => d.Id == departmentId);
			if (department == null)
			{
				throw new InvalidDataException("Entered department id not included in available identifiers!");
			}

			return GetBypassInfoByDepartment(department);
		}

		#endregion
	}
}
