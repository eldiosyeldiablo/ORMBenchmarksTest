using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

	public long GetPlayerByID(int id)
	{
		Stopwatch watch = new Stopwatch();
		watch.Start();
		using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
		{
			conn.Open();
			var player = conn.Query<PlayerDTO>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE Id = @ID", new{ ID = id});
		}
		watch.Stop();
		return watch.ElapsedMilliseconds;
	}

	public long GetPlayersForTeam(int teamId)
	{
		Stopwatch watch = new Stopwatch();
		watch.Start();
		using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
		{
			conn.Open();
			var players = conn.Query<List<PlayerDTO>>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE TeamId = @ID", new { ID = teamId });
		}
		watch.Stop();
		return watch.ElapsedMilliseconds;
	}

	public long GetTeamsForSport(int sportId)
	{
		Stopwatch watch = new Stopwatch();
		watch.Start();
		using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
		{
			conn.Open();
			var players = conn.Query<PlayerDTO, TeamDTO, PlayerDTO>("SELECT p.Id, p.FirstName, p.LastName, p.DateOfBirth, p.TeamId, t.Id as TeamId, t.Name, t.SportId FROM Team t "
				+ "INNER JOIN Player p ON t.Id = p.TeamId WHERE t.SportId = @ID", (player, team) => { return player; }, splitOn: "TeamId", param: new { ID = sportId });
		}
		watch.Stop();
		return watch.ElapsedMilliseconds;
	}
}
}
