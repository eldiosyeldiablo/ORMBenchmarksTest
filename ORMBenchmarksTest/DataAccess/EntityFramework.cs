using ORMBenchmarksTest.DTOs;
using ORMBenchmarksTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ORMBenchmarksTest.TestData;

namespace ORMBenchmarksTest.DataAccess
{
	public class EntityFramework : ITestSignature
	{
		public Framework FrameWorkType { get; } = Framework.EntityFramework;

		public bool GetPlayerByID(int id)
		{
			Player player = null;
			using (SportContext context = new SportContext())
			{
				player = context.Players.Find(id);
			}

			return true;
		}

		public bool GetPlayersForTeam(int teamId)
		{
			using (SportContext context = new SportContext())
			{
				var players = context.Players.AsNoTracking().Where(x => x.TeamId == teamId).ToList();
			}

			return true;
		}

		public bool GetTeamsForSport(int sportId)
		{
			using (SportContext context = new SportContext())
			{
				var players = context.Teams.AsNoTracking().Include(x=>x.Players).Where(x => x.SportId == sportId).ToList();
			}

			return true;
		}
	}
}
