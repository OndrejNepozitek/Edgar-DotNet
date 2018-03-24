namespace MapGeneration.Core.LayoutEvolvers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.LayoutEvolvers;
	using Interfaces.Core.LayoutOperations;
	using Interfaces.Core.Layouts;
	using Interfaces.Utils;
	using LayoutOperations;

	public class SimulatedAnnealingEvolver<TLayout, TNode, TConfiguration> : ILayoutEvolver<TLayout, TNode>, IRandomInjectable, ICancellable
		where TLayout : ILayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
		where TConfiguration : IConfiguration<IntAlias<GridPolygon>>
	{
		protected Random Random;
		protected CancellationToken? CancellationToken;

		protected IChainBasedLayoutOperations<TLayout, TNode> LayoutOperations;

		public event Action<TLayout> OnPerturbed;
		public event Action<TLayout> OnValid;

		protected int Cycles = 50;
		protected int TrialsPerCycle = 100;

		public SimulatedAnnealingEvolver(IChainBasedLayoutOperations<TLayout, TNode> layoutOperations)
		{
			LayoutOperations = layoutOperations;
		}

		public IEnumerable<TLayout> Evolve(TLayout initialLayout, IList<TNode> chain, int count)
		{
			const double p0 = 0.2d;
			const double p1 = 0.01d;
			var t0 = -1d / Math.Log(p0);
			var t1 = -1d / Math.Log(p1);
			var ratio = Math.Pow(t1 / t0, 1d / (Cycles - 1));
			var deltaEAvg = 0d;
			var acceptedSolutions = 1;

			var t = t0;

			var layouts = new List<TLayout>();
			var originalLayout = initialLayout; 
			var currentLayout = originalLayout;

			#region Debug output

			//if (withDebugOutput)
			//{
			//	Console.WriteLine($"Initial energy: {layoutOperations.GetEnergy(currentLayout)}");
			//}

			#endregion

			var numberOfFailures = 0;

			for (var i = 0; i < Cycles; i++)
			{
				var wasAccepted = false;

				#region Random restarts

				if (enableRandomRestarts)
				{
					if (ShouldRestart(numberOfFailures))
					{
						break;
					}
				}

				#endregion

				for (var j = 0; j < TrialsPerCycle; j++)
				{
					if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
						yield break;

					var perturbedLayout = PerturbLayout(currentLayout, chain, out var energyDelta);

					OnPerturbed?.Invoke(perturbedLayout);

					// TODO: can we check the energy instead?
					if (LayoutOperations.IsLayoutValid(perturbedLayout, chain))
					{
						#region Random restarts
						if (enableRandomRestarts && randomRestartsSuccessPlace == RestartSuccessPlace.OnValid)
						{
							wasAccepted = true;

							if (randomRestartsResetCounter)
							{
								numberOfFailures = 0;
							}
						}
						#endregion

						// TODO: wouldn't it be too slow to compare againts all?
						if (IsDifferentEnough(perturbedLayout, layouts))
						{
							var shouldContinue = true;

							// TODO: should it be like this?
							if (LayoutOperations is ILayoutOperationsWithCorridors<TLayout, TNode> layoutOperationsWithCorridors)
							{
								shouldContinue = layoutOperationsWithCorridors.AddCorridors(perturbedLayout, chain);
							}

							if (shouldContinue)
							{
								layouts.Add(perturbedLayout);
								OnValid?.Invoke(perturbedLayout);

								#region Random restarts
								if (enableRandomRestarts && randomRestartsSuccessPlace == RestartSuccessPlace.OnValidAndDifferent)
								{
									wasAccepted = true;

									if (randomRestartsResetCounter)
									{
										numberOfFailures = 0;
									}
								}
								#endregion

								yield return perturbedLayout;

								#region Debug output

								//if (withDebugOutput)
								//{
								//	Console.WriteLine($"Found layout, cycle {i}, trial {j}, energy {layoutOperations.GetEnergy(perturbedLayout)}");
								//}

								#endregion

								if (layouts.Count >= count)
								{
									#region Debug output

									//if (withDebugOutput)
									//{
									//	Console.WriteLine($"Returning {layouts.Count} partial layouts");
									//}

									#endregion

									yield break;
								}
							}
						}
					}

					var deltaAbs = Math.Abs(energyDelta);
					var accept = false;

					if (energyDelta > 0)
					{
						if (i == 0 && j == 0)
						{
							deltaEAvg = deltaAbs * 15;
						}

						var p = Math.Pow(Math.E, -deltaAbs / (deltaEAvg * t));
						if (Random.NextDouble() < p)
							accept = true;
					}
					else
					{
						accept = true;
					}

					if (accept)
					{
						acceptedSolutions++;
						currentLayout = perturbedLayout;
						deltaEAvg = (deltaEAvg * (acceptedSolutions - 1) + deltaAbs) / acceptedSolutions;

						#region Random restarts
						if (enableRandomRestarts && randomRestartsSuccessPlace == RestartSuccessPlace.OnAccepted)
						{
							wasAccepted = true;

							if (randomRestartsResetCounter)
							{
								numberOfFailures = 0;
							}
						}
						#endregion
					}

				}

				if (!wasAccepted)
				{
					numberOfFailures++;
				}

				t = t * ratio;
			}

			#region Debug output

			//if (withDebugOutput)
			//{
			//	Console.WriteLine($"Returning {layouts.Count} partial layouts");
			//}

			#endregion
		}

		private TLayout PerturbLayout(TLayout layout, IList<TNode> chain, out double energyDelta)
		{
			// TODO: sometimes perturb a node that is not in the current chain?
			var newLayout = layout.SmartClone();

			var energy = LayoutOperations.GetEnergy(newLayout);

			LayoutOperations.PerturbLayout(newLayout, chain, true);

			var newEnergy = LayoutOperations.GetEnergy(newLayout);
			energyDelta = newEnergy - energy;

			return newLayout;
		}

		private bool IsDifferentEnough(TLayout layout, IList<TLayout> layouts)
		{
			return layouts.All(x => LayoutOperations.AreDifferentEnough(layout, x));
		}

		#region Random restarts

		private bool enableRandomRestarts = true;
		private RestartSuccessPlace randomRestartsSuccessPlace = RestartSuccessPlace.OnValidAndDifferent;
		private bool randomRestartsResetCounter = false;
		private float randomRestartsScale = 1;
		private List<int> randomRestartProbabilities = new List<int>() { 2, 3, 5, 7 };

		public void SetRandomRestarts(bool enable, RestartSuccessPlace successPlace = RestartSuccessPlace.OnValidAndDifferent, bool resetCounter = false, float scale = 1f)
		{
			enableRandomRestarts = enable;
			randomRestartsSuccessPlace = successPlace;
			randomRestartsResetCounter = resetCounter;

			if (scale <= 0)
				throw new ArgumentException();

			randomRestartsScale = scale;
			randomRestartProbabilities = (new List<int>() { 2, 3, 5, 7 }).Select(x => (int)(x * randomRestartsScale)).ToList();
		}

		private bool ShouldRestart(int numberOfFailures)
		{
			// ReSharper disable once ReplaceWithSingleAssignment.False
			var shouldRestart = false;

			if (numberOfFailures > 8 && Random.Next(0, randomRestartProbabilities[0]) == 0)
			{
				shouldRestart = true;
			}
			else if (numberOfFailures > 6 && Random.Next(0, randomRestartProbabilities[1]) == 0)
			{
				shouldRestart = true;
			}
			else if (numberOfFailures > 4 && Random.Next(0, randomRestartProbabilities[2]) == 0)
			{
				shouldRestart = true;
			}
			else if (numberOfFailures > 2 && Random.Next(0, randomRestartProbabilities[3]) == 0)
			{
				shouldRestart = true;
			}

			//if (shouldRestart && withDebugOutput)
			//{
			//	Console.WriteLine($"Break, we got {numberOfFailures} failures");
			//}

			return shouldRestart;
		}

		public enum RestartSuccessPlace
		{
			OnValid, OnValidAndDifferent, OnAccepted
		}

		#endregion

		public void Configure(int cycles, int trialsPerCycle)
		{
			this.Cycles = cycles;
			this.TrialsPerCycle = trialsPerCycle;
		}

		public void InjectRandomGenerator(Random random)
		{
			Random = random;
		}

		public void SetCancellationToken(CancellationToken? cancellationToken)
		{
			CancellationToken = cancellationToken;
		}
	}
}