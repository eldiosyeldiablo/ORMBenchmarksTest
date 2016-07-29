using ORMBenchmarksTest.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMBenchmarksTest.DataAccess
{
	public interface ITestSignature
	{
		Framework FrameWorkType { get; }

		bool GetPlayerByID(int id);
		bool GetPlayers(int count);
		bool GetPlayersForTeam(int teamID);
		bool GetTeamsForSport(int sportID);
	}
}
