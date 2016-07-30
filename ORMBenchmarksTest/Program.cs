using ORMBenchmarksTest.Configuration;
using ORMBenchmarksTest.DataAccess;
using ORMBenchmarksTest.DTOs;
using ORMBenchmarksTest.TestData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMBenchmarksTest
{
	class Program
	{
		public static int CountOfPlayersTopX = 1000;
		public static int TimeRoundPrecision = 5;
		public static int NumPlayers { get; set; }
		public static int NumTeams { get; set; }
		public static int NumSports { get; set; }
		public static int NumRuns { get; set; }
		static void Main(string[] args)
		{
			char input;
			AutoMapperConfiguration.Configure();
			do
			{
				ShowMenu();

				input = Console.ReadLine().ToUpper().First();
				switch (input)
				{
					case 'Q':
						break;

					case 'T':
						PrepareAndRunTests();
						break;
				}

			}
			while (input != 'Q');
		}

		private static void PrepareAndRunTests()
		{
			Console.WriteLine($"Resetting existing db records to start clean");
			Database.Reset();

			List<TestResult> testResults = new List<TestResult>();

			Console.WriteLine("# of Test Runs:");
			NumRuns = int.Parse(Console.ReadLine());

			//Gather Details for Test
			Console.WriteLine("# of Sports per Run: ");
			NumSports = int.Parse(Console.ReadLine());

			Console.WriteLine("# of Teams per Sport: ");
			NumTeams = int.Parse(Console.ReadLine());

			Console.WriteLine("# of Players per Team: ");
			NumPlayers = int.Parse(Console.ReadLine());

			Console.WriteLine($"Generating {NumSports} Sports");
			List<SportDTO> sports = TestData.Generator.GenerateSports(NumSports);
			List<TeamDTO> teams = new List<TeamDTO>();
			List<PlayerDTO> players = new List<PlayerDTO>();
			foreach (var sport in sports)
			{
				Console.WriteLine($"Generating {NumTeams} Teams for Sport.Id {sport.Id}");

				var newTeams = TestData.Generator.GenerateTeams(sport.Id, NumTeams);
				teams.AddRange(newTeams);
				foreach (var team in newTeams)
				{
					Console.WriteLine($"Generating {NumPlayers} Players for Team.Id {team.Id}");
					var newPlayers = TestData.Generator.GeneratePlayers(team.Id, NumPlayers);
					players.AddRange(newPlayers);
				}
			}

			Database.Load(sports, teams, players);

			Console.WriteLine("Starting tests");

			List<ITestSignature> testsORMs = new List<ITestSignature>()
			{
				new EntityFramework(),
				new ADONET(),
				new ADONetReader(),
				new DataAccess.Dapper(),
				new PetaPocoORM()
			};

			Console.WriteLine("Shufflining the order of ORMs to run");
			Fisher_Yates_CardDeck_Shuffle(testsORMs);
			for (int i = 0; i < NumRuns; i++)
			{
				Console.WriteLine($"\nRunning test simulation number {i}");
				foreach (var tester in testsORMs)
				{
					Console.WriteLine($"Testing {tester.FrameWorkType}");
					var testResult = RunTests(i, tester.FrameWorkType, tester);
					testResults.AddRange(testResult);
				}
			}
			ProcessResults(testResults);
		}

		/// <summary>
		/// Fisher_Yates_CardDeck_Shuffle
		/// </summary>
		/// <param name="aList">a list.</param>
		/// <returns></returns>
		/// With the Fisher-Yates shuffle, first implemented on computers by Durstenfeld in 1964,
		/// we randomly sort elements. This is an accurate, effective shuffling method for all array types.
		public static List<ITestSignature> Fisher_Yates_CardDeck_Shuffle(List<ITestSignature> aList)
		{

			System.Random _random = new System.Random();

			ITestSignature myGO;

			int n = aList.Count;
			for (int i = 0; i < n; i++)
			{
				// NextDouble returns a random number between 0 and 1.
				// ... It is equivalent to Math.random() in Java.
				int r = i + (int)(_random.NextDouble() * (n - i));
				myGO = aList[r];
				aList[r] = aList[i];
				aList[i] = myGO;
			}

			return aList;
		}
		public static List<TestResult> RunTests(int runID, Framework framework, ITestSignature testSignature)
		{
			Stopwatch watch = new Stopwatch();
			List<TestResult> results = new List<TestResult>();

			TestResult result = new TestResult() { Run = runID, Framework = framework };

			List<long> playerByIDResults = new List<long>();
			for (int i = 1; i <= NumPlayers; i++)
			{
				watch.Reset();
				watch.Start();
				var getPlayers = testSignature.GetPlayerByID(i);
				watch.Stop();
				var time = watch.ElapsedMilliseconds;
				playerByIDResults.Add(time);
			}
			result.PlayerByIDMilliseconds = Math.Round(playerByIDResults.Average(), TimeRoundPrecision);

			List<long> playersForTeamResults = new List<long>();
			for (int i = 1; i <= NumTeams; i++)
			{
				watch.Reset();
				watch.Start();
				var getPlayers = testSignature.GetPlayersForTeam(i);
				watch.Stop();
				var time = watch.ElapsedMilliseconds;
				playersForTeamResults.Add(time);
			}
			result.PlayersForTeamMilliseconds = Math.Round(playersForTeamResults.Average(), TimeRoundPrecision);

			List<long> teamsForSportResults = new List<long>();
			for (int i = 1; i <= NumSports; i++)
			{
				watch.Reset();
				watch.Start();
				var getPlayers = testSignature.GetTeamsForSport(i);
				watch.Stop();
				var time = watch.ElapsedMilliseconds;
				teamsForSportResults.Add(time);
			}
			result.TeamsForSportMilliseconds = Math.Round(teamsForSportResults.Average(), TimeRoundPrecision);

			List<long> playersSelectTopXResults = new List<long>();
			{
				watch.Reset();
				watch.Start();
				var getPlayersTopX = testSignature.GetPlayers(CountOfPlayersTopX);
				watch.Stop();
				var time = watch.ElapsedMilliseconds;
				playersSelectTopXResults.Add(time);
			}
			result.PlayersSelectTopXMilliseconds = Math.Round(playersSelectTopXResults.Average(), TimeRoundPrecision);

			results.Add(result);

			return results;
		}

		public static void ProcessResults(List<TestResult> results)
		{
			var groupedResults = results.GroupBy(x => x.Framework).OrderBy(x=>x.Key);
			foreach(var group in groupedResults)
			{
				Console.WriteLine($"\n{group.Key} Results in Milliseconds");
				Console.WriteLine($"Run #\tPlayer by ID\t\tPlayers per Team\t\tTeams per Sport\t\tPlayers Top {CountOfPlayersTopX}");
				var orderedResults = group.OrderBy(x=>x.Run);
				foreach(var orderResult in orderedResults)
				{
					Console.WriteLine($"{orderResult.Run}\t\t{orderResult.PlayerByIDMilliseconds}\t\t\t{orderResult.PlayersForTeamMilliseconds}\t\t\t{orderResult.TeamsForSportMilliseconds}\t\t\t{orderResult.PlayersSelectTopXMilliseconds}");
				}

				var avgPlayerById = Math.Round(orderedResults.Average(x => x.PlayerByIDMilliseconds), TimeRoundPrecision);
				var avgPlayersForTeam = Math.Round(orderedResults.Average(x => x.PlayersForTeamMilliseconds), TimeRoundPrecision);
				var avgPlayersSelectTopX = Math.Round(orderedResults.Average(x => x.PlayersSelectTopXMilliseconds), TimeRoundPrecision);
				var avgPlayersForSport = Math.Round(orderedResults.Average(x => x.TeamsForSportMilliseconds), TimeRoundPrecision);

				Console.WriteLine($"Average\t\t{avgPlayerById}\t\t\t{avgPlayersForTeam}\t\t\t{avgPlayersForSport}\t\t\t{avgPlayersSelectTopX}");
			}
		}

		public static void ShowMenu()
		{
			Console.WriteLine("Please enter one of the following options:");
			Console.WriteLine("Q - Quit");
			Console.WriteLine("T - Run Test");
			Console.WriteLine("Option:");
		}
	}
}