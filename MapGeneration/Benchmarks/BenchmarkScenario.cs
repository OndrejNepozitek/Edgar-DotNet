namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using Core.Interfaces;

	public class BenchmarkScenario<TLayoutGenerator, TNode>
		where TLayoutGenerator : ILayoutGenerator<TNode>
	{
		private readonly List<SetupsGroup> setupsGroups = new List<SetupsGroup>();
		private int runsCount = 1;

		public void AddSetupsGroup(SetupsGroup setupsGroup)
		{
			setupsGroups.Add(setupsGroup);
		}

		public SetupsGroup MakeSetupsGroup()
		{
			var group = new SetupsGroup();
			setupsGroups.Add(group);

			return group;
		}

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

		public List<SetupsGroup> GetSetupsGroups()
		{
			AddRuns();

			return setupsGroups;
		}

		public class SetupsGroup
		{
			private readonly List<Tuple<string, Action<TLayoutGenerator>>> setups = new List<Tuple<string, Action<TLayoutGenerator>>>();

			public void AddSetup(string name, Action<TLayoutGenerator> setup)
			{
				setups.Add(Tuple.Create(name, setup));
			}

			public List<Tuple<string, Action<TLayoutGenerator>>> GetSetups()
			{
				return setups;
			}
		}
	}
}