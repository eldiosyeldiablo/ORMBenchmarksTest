using System;
using System.Collections.Generic;
using Dapper;
using DapperExtensions;
using System.Data.SqlClient;
using ORMBenchmarksTest.DTOs;
using ORMBenchmarksTest.TestData;

namespace ORMBenchmarksTest.DataAccess
{
	public class Dapper : ITestSignature
	{
		public Framework FrameWorkType { get; } = Framework.Dapper;

		public bool GetPlayerByID(int id)
		{
			using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
			{
				conn.Open();
				var player = conn.Query<PlayerDTO>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE Id = @ID", new { ID = id });
			}

			return true;
		}

		public bool GetPlayers(int count)
		{
			using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
			{
				conn.Open();
				var player = conn.Query<PlayerDTO>($"SELECT TOP {count} Id, FirstName, LastName, DateOfBirth, TeamId FROM Player");
			}

			return true;
		}

		public bool GetPlayersForTeam(int teamId)
		{
			using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
			{
				conn.Open();
				var players = conn.Query<List<PlayerDTO>>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE TeamId = @ID", new { ID = teamId })
					.AsList();
			}

			return true;
		}

		public bool GetTeamsForSport(int sportId)
		{
			using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
			{
				conn.Open();
				var players = conn.Query<PlayerDTO, TeamDTO, PlayerDTO>("SELECT p.Id, p.FirstName, p.LastName, p.DateOfBirth, p.TeamId, t.Id as TeamId, t.Name, t.SportId FROM Team t "
					+ "INNER JOIN Player p ON t.Id = p.TeamId WHERE t.SportId = @ID", (player, team) => { return player; }, splitOn: "TeamId", param: new { ID = sportId })
					.AsList();
			}

			return true;
		}
	}
}