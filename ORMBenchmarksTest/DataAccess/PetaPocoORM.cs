using System;
using System.Linq;
using System.Collections.Generic;
using PetaPoco;
using System.Data.SqlClient;
using ORMBenchmarksTest.DTOs;
using ORMBenchmarksTest.TestData;

namespace ORMBenchmarksTest.DataAccess
{
	public class PetaPocoORM : ITestSignature
	{
		public Framework FrameWorkType { get; } = Framework.PetaPoco;

		private IDatabase GetConnection()
		{
			var db = PetaPoco.DatabaseConfiguration.Build()
				.UsingConnectionString(Constants.ConnectionString)
				.UsingProvider<SqlServerDatabaseProvider>()
				.Create();
			return db;
		}

		public bool GetPlayers(int count)
		{
			var db = GetConnection();
			var player = db.Fetch<PlayerDTO>($"SELECT TOP {count} Id, FirstName, LastName, DateOfBirth, TeamId FROM Player");

			return true;
		}

		public bool GetPlayerByID(int id)
		{
			var db = GetConnection();
			var player = db.Single<PlayerDTO>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE Id = @ID", new { ID = id });

			return true;
		}

		public bool GetPlayersForTeam(int teamId)
		{
			var db = GetConnection();
			var players = db.Fetch<List<PlayerDTO>>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE TeamId = @ID", new { ID = teamId });

			return true;
		}

		public bool GetTeamsForSport(int sportId)
		{
			var db = GetConnection();
			var players = db.Fetch<PlayerDTO, TeamDTO, PlayerDTO>((player, team) => { return player; },
				"SELECT p.Id, p.FirstName, p.LastName, p.DateOfBirth, p.TeamId, t.Id as TeamId, t.Name, t.SportId FROM Team t "
					+ "INNER JOIN Player p ON t.Id = p.TeamId WHERE t.SportId = @ID", new { ID = sportId });

			return true;
		}
	}
}