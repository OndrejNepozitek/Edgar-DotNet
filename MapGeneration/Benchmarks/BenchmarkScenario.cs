namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using Core.Interfaces;

	/// <summary>
	/// Class holding a scenario that should be run in a benchmark.
	/// </summary>
	/// <typeparam name="TLayoutGenerator"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	public class BenchmarkScenario<TLayoutGenerator, TNode>
		where TLayoutGenerator : ILayoutGenerator<TNode>
	{
		private readonly List<SetupsGroup> setupsGroups = new List<SetupsGroup>();
		private int runsCount = 1;

		/// <summary>
		/// Adds a setup to the scenario.
		/// </summary>
		/// <param name="setupsGroup"></param>
		public void AddSetupsGroup(SetupsGroup setupsGroup)
		{
			setupsGroups.Add(setupsGroup);
		}

		/// <summary>
		/// Creates a setups group and adds it to the scenario.
		/// </summary>
		/// <remarks>
		/// This is a shortcut for creating a setups group and then adding it to the scenario.
		/// Generic parameters can be infered as it is a method.
		/// </remarks>
		/// <returns></returns>
		public SetupsGroup MakeSetupsGroup()
		{
			var group = new SetupsGroup();
			setupsGroups.Add(group);

			return group;
		}

		/// <summary>
		/// How many times should all the setups be run. Defaults to 1.
		/// </summary>
		/// <param name="count"></param>
		public void SetRunsCount(int count)
		{
			runsCount = count;
		}

		private void AddRuns()
		{
			if (runsCount == 1)
				return;

			var setupsGroup = new SetupsGroup();

			for (var i = 0; i < runsCount; i++)
			{
				setupsGroup.AddSetup($"Run {i+1}", generator => {});	
			}
		}

		/// <summary>
		/// Gets all setups groups of the scenario.
		/// </summary>
		/// <returns></returns>
		public List<SetupsGroup> GetSetupsGroups()
		{
			AddRuns();

			return setupsGroups;
		}

		/// <summary>
		/// Class holding a setups group.
		/// </summary>
		public class SetupsGroup
		{
			private readonly List<Tuple<string, Action<TLayoutGenerator>>> setups = new List<Tuple<string, Action<TLayoutGenerator>>>();

			/// <summary>
			/// Adds a setup to the group.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="setup">This action will be used in the benchmark when setting up the generator.</param>
			public void AddSetup(string name, Action<TLayoutGenerator> setup)
			{
				setups.Add(Tuple.Create(name, setup));
			}

			/// <summary>
			/// Get all added setups.
			/// </summary>
			/// <returns></returns>
			public List<Tuple<string, Action<TLayoutGenerator>>> GetSetups()
			{
				return setups;
			}
		}
	}
}