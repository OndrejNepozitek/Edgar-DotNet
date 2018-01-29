namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using Core.Interfaces;

	public class BenchmarkScenarios<TLayoutGenerator, TNode>
		where TLayoutGenerator : ILayoutGenerator<TNode>
	{
		private readonly List<List<Tuple<string, Action<TLayoutGenerator>>>> scenarios = new List<List<Tuple<string, Action<TLayoutGenerator>>>>();

		public void AddScenario(List<Tuple<string, Action<TLayoutGenerator>>> scenario) 
		{
			scenarios.Add(scenario);
		}

		public List<List<Tuple<string, Action<TLayoutGenerator>>>> GetSetups()
		{
			return scenarios;
		}
	}
}