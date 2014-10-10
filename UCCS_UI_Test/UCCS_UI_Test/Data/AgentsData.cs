using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCCS_UI_Test.Models;

namespace UCCS_UI_Test.Data
{
	public static class AgentsData
	{
		public static ObservableCollection<AgentCompactBindingSource> GetAgentCompactBindingSource()
		{
			ObservableCollection<AgentCompactBindingSource> result = new ObservableCollection<AgentCompactBindingSource>();

			AgentCompactBindingSource acm = new AgentCompactBindingSource()
			{
				Name = "Leo L -2175",
				PhoneNumber = "555 - 333 - 6666",
				Status = AgentStatus.UnavailableIn,
				StatusTime = new TimeSpan(0, 14, 32),
				IdleTime = new TimeSpan(0, 15, 54),
				CallsGroup = 10
			};
			result.Add(acm);

			acm = new AgentCompactBindingSource()
			{
				Name = "Ryen W -2184",
				//PhoneNumber = "555 - 111 - 7777",
				Status = AgentStatus.Idle,
				StatusTime = new TimeSpan(0, 0, 54),
				IdleTime = new TimeSpan(1, 25, 12),
				CallsGroup = 5
			};
			result.Add(acm);

			acm = new AgentCompactBindingSource()
			{
				Name = "Paul S -2194",
				//PhoneNumber = "555 - 111 - 7777",
				Status = AgentStatus.LoggedOut,
				StatusTime = new TimeSpan(3, 42, 54),
				IdleTime = new TimeSpan(1, 25, 12),
				CallsGroup = 2
			};
			result.Add(acm);

			acm = new AgentCompactBindingSource()
			{
				Name = "Christopher V -2188",
				PhoneNumber = "555 - 999 - 0000",
				Status = AgentStatus.Busy,
				StatusTime = new TimeSpan(0, 19, 09),
				IdleTime = new TimeSpan(3, 25, 12),
				CallsGroup = 13
			};
			result.Add(acm);

			acm = new AgentCompactBindingSource()
			{
				Name = "Brad B -2135",
				PhoneNumber = "555 - 333 - 4563",
				Status = AgentStatus.Handle,
				StatusTime = new TimeSpan(0, 05, 09),
				IdleTime = new TimeSpan(0, 05, 12),
				CallsGroup = 6
			};
			result.Add(acm);

			return result;
		}

		public static AgentCompactBindingSource Get1AgentCompactBindingSource()
		{
			return new AgentCompactBindingSource()
			{
				Name = "NNNN",
				Status = AgentStatus.Busy,
				CallsGroup = 32,
				StatusTime = new TimeSpan(0, 14, 32),
				IdleTime = new TimeSpan(0, 15, 54),
				PhoneNumber = "555 - 234 - 5678"
			};
		}

		public static List<AgentCompactModel> GetAgentsData()
		{
			List<AgentCompactModel> result = new List<AgentCompactModel>();

			AgentCompactModel acm = new AgentCompactModel()
			{
				Name = "Leo L -2175",
				PhoneNumber = "555 - 333 - 6666",
				Status = AgentStatus.UnavailableIn,
				StatusTime = new TimeSpan(0, 14, 32),
				IdleTime = new TimeSpan(0, 15, 54),
				CallsGroup = 10
			};
			result.Add(acm);

			acm = new AgentCompactModel()
			{
				Name = "Ryen W -2184",
				//PhoneNumber = "555 - 111 - 7777",
				Status = AgentStatus.Idle,
				StatusTime = new TimeSpan(0, 0, 54),
				IdleTime = new TimeSpan(1, 25, 12),
				CallsGroup = 5
			};
			result.Add(acm);

			acm = new AgentCompactModel()
			{
				Name = "Paul S -2194",
				//PhoneNumber = "555 - 111 - 7777",
				Status = AgentStatus.LoggedOut,
				StatusTime = new TimeSpan(3, 42, 54),
				IdleTime = new TimeSpan(1, 25, 12),
				CallsGroup = 2
			};
			result.Add(acm);

			acm = new AgentCompactModel()
			{
				Name = "Christopher V -2188",
				PhoneNumber = "555 - 999 - 0000",
				Status = AgentStatus.Busy,
				StatusTime = new TimeSpan(0, 19, 09),
				IdleTime = new TimeSpan(3, 25, 12),
				CallsGroup = 13
			};
			result.Add(acm);

			acm = new AgentCompactModel()
			{
				Name = "Brad B -2135",
				PhoneNumber = "555 - 333 - 4563",
				Status = AgentStatus.Handle,
				StatusTime = new TimeSpan(0, 05, 09),
				IdleTime = new TimeSpan(0, 05, 12),
				CallsGroup = 6
			};
			result.Add(acm);
			
			return result;
		}

	}
}
