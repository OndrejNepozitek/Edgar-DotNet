namespace MapGeneration.Core.SimulatedAnnealing.GeneratorPlanner
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// This planner should not be used. It is here just to demonstrate that 
	/// it would be slower to not use lazy evaluation.
	/// </summary>
	[Obsolete("This generator tries hard to show that non-lazy planning does not make any sense. Use any other planner.")]
	public class BasicGeneratorPlanner<TLayout> : GeneratorPlannerBase<TLayout>
	{
		private int currentRow;
		private Instance lastInstance;

		private readonly bool resetAfterValid = true;

		public BasicGeneratorPlanner()
		{
			/* empty */
		}

		public BasicGeneratorPlanner(bool resetAfterValid)
		{
			this.resetAfterValid = resetAfterValid;
		}

		/// <summary>
		/// Simulates how would non-lazy evaluation work.
		/// </summary>
		/// <param name="rows"></param>
		/// <returns></returns>
		protected override Instance GetNextInstance(List<InstanceRow> rows)
		{
			if (lastInstance != null && lastInstance.IsFinished)
			{
				currentRow = rows.Count - 1;
			}

			var instance = rows[currentRow].Instances.FirstOrDefault(x => !x.IsFinished);

			if (instance != null)
			{
				lastInstance = instance;
				return instance;
			}
				
			var rowIndex = rows.Count - 1;

			while (rowIndex >= 0)
			{
				instance = rows[rowIndex].Instances.FirstOrDefault(x => !x.IsFinished);

				if (instance != null)
				{
					currentRow = rowIndex;
					lastInstance = instance;
					return instance;
				}

				rowIndex--;
			}

			instance = AddZeroLevelInstance();
			lastInstance = instance;
			return instance;
		}

		protected override void BeforeGeneration()
		{
			base.BeforeGeneration();

			currentRow = 0;
			lastInstance = null;
		}

		protected override void AfterValid()
		{
			base.AfterValid();

			if (resetAfterValid)
			{
				ResetRows();
			}
		}
	}
}