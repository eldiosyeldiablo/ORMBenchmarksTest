using ORMBenchmarksTest.DTOs;
using ORMBenchmarksTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMBenchmarksTest.TestData
{
	public static class Database
	{
		public static void Reset()
		{
			using(SportContext context = new SportContext())
			{
				context.Database.ExecuteSqlCommand("DELETE FROM Player");
				context.Database.ExecuteSqlCommand("DELETE FROM Team");
				context.Database.ExecuteSqlCommand("DELETE FROM Sport");
			}
		}

		public static void Load(List<SportDTO> sports, List<TeamDTO> teams, List<PlayerDTO> players)
		{
			AddSports(sports);
			AddTeams(teams);
			AddPlayers(players);
		}

		private static void AddPlayers(List<PlayerDTO> players)
		{
			Console.WriteLine($"Persisting {players.Count} players");
			//using (TransactionScope scope = new TransactionScope())
			{
				SportContext context = null;
				try
				{
					context = new SportContext();
					context.Configuration.AutoDetectChangesEnabled = false;

					int count = 0;
					foreach (var player in players)
					{
						count++;
						var p = new Player()
						{
							FirstName = player.FirstName,
							LastName = player.LastName,
							DateOfBirth = player.DateOfBirth,
							TeamId = player.TeamId,
							Id = player.Id
						};

						//flush to db after 1000 records
						context = AddToContext(context, p, count, 1000, true);
					}

					context.SaveChanges();
				}
				finally
				{
					if (context != null)
						context.Dispose();
				}

				//scope.Complete();
			}
		}

		private static SportContext AddToContext(SportContext context, Player entity, int count, int commitCount, bool recreateContext)
		{
			context.Set<Player>().Add(entity);

			if (count % commitCount == 0)
			{
				Console.WriteLine($"Flushing out {commitCount} players of {count} to database to avoid very large saves");
				context.SaveChanges();
				if (recreateContext)
				{
					context.Dispose();
					context = new SportContext();
					context.Configuration.AutoDetectChangesEnabled = false;
					context.Configuration.ValidateOnSaveEnabled = false;
				}
			}

			return context;
		}

		private static void AddTeams(List<TeamDTO> teams)
		{
			Console.WriteLine($"Persisting {teams.Count} teams");
			using (SportContext context = new SportContext())
			{
				foreach (var team in teams)
				{
					context.Teams.Add(new Team()
					{
						Name = team.Name,
						Id = team.Id,
						SportId = team.SportId,
						FoundingDate = team.FoundingDate
					});
				}

				context.SaveChanges();
			}
		}

		private static void AddSports(List<SportDTO> sports)
		{
			Console.WriteLine($"Persisting {sports.Count} sports");
			using (SportContext context = new SportContext())
			{
				foreach (var sport in sports)
				{
					context.Sports.Add(new Sport()
					{
						Id = sport.Id,
						Name = sport.Name
					});
				}

				context.SaveChanges();
			}
		}
	}
}
