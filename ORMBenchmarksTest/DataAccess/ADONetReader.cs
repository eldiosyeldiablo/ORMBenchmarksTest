using ORMBenchmarksTest.DTOs;
using ORMBenchmarksTest.TestData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ORMBenchmarksTest.DataAccess
{
	public class ADONetReader : ITestSignature
	{
		public Framework FrameWorkType { get; } = Framework.ADONetDr;

		public bool GetPlayerByID(int id)
		{
			using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE Id = @ID", conn))
				{
					command.Parameters.Add(new SqlParameter("@ID", id));
					var reader = command.ExecuteReader();
					var item = AutoMapper.Mapper.DynamicMap<List<PlayerDTO>>(reader);

				}
			}

			return true;
		}

		public bool GetPlayersForTeam(int teamId)
		{
			using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE TeamId = @ID", conn))
				{
					command.Parameters.Add(new SqlParameter("@ID", teamId));
					var reader = command.ExecuteReader();
					var items = AutoMapper.Mapper.DynamicMap<List<PlayerDTO>>(reader);
				}
			}

			return true;
		}

		public bool GetTeamsForSport(int sportId)
		{
			using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT p.Id, p.FirstName, p.LastName, p.DateOfBirth, p.TeamId, t.Id as TeamId, t.Name, t.SportId FROM Player p "
																  + "INNER JOIN Team t ON p.TeamId = t.Id WHERE t.SportId = @ID", conn))
				{
					command.Parameters.Add(new SqlParameter("@ID", sportId));
					var reader = command.ExecuteReader();
					var items = AutoMapper.Mapper.DynamicMap<List<PlayerDTO>>(reader);
				}
			}

			return true;
		}
	}
}
