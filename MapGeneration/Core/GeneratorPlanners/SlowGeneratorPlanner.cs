namespace MapGeneration.Core.GeneratorPlanners
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// This planner should not be used. It is here just to demonstrate that 
	/// it would be realy slow to not use lazy evaluation.
	/// </summary>
	[Obsolete("This generator tries hard to show that non-lazy planning does not make any sense. Use any other planner.")]
	public class SlowGeneratorPlanner<TLayout> : GeneratorPlannerBase<TLayout>
	{
		private int currentRow;
		private Node lastNode;

		private readonly bool clearTreeAfterComplete;

		/// <summary>
		/// </summary>
		/// <param name="clearTreeAfterComplete">Whether the tree should be cleared after each complete layout that is generated.</param>
		public SlowGeneratorPlanner(bool clearTreeAfterComplete = true)
		{
			this.clearTreeAfterComplete = clearTreeAfterComplete;
		}

		/// <summary>
		/// Simulates how would non-lazy evaluation work.
		/// </summary>
		/// <param name="rows"></param>
		/// <returns></returns>
		protected override Node GetNextInstance(List<NodeRow> rows)
		{
			if (lastNode != null && lastNode.IsFinished)
			{
				currentRow = rows.Count - 1;
			}

			var instance = rows[currentRow].Instances.FirstOrDefault(x => !x.IsFinished);

			if (instance != null)
			{
				lastNode = instance;
				return instance;
			}
				
			var rowIndex = rows.Count - 1;

			while (rowIndex >= 0)
			{
				instance = rows[rowIndex].Instances.FirstOrDefault(x => !x.IsFinished);

				if (instance != null)
				{
					currentRow = rowIndex;
					lastNode = instance;
					return instance;
				}

				rowIndex--;
			}

			instance = AddZeroLevelNode();
			lastNode = instance;
			return instance;
		}

		protected override void BeforeGeneration()
		{
			base.BeforeGeneration();

			currentRow = 0;
			lastNode = null;
		}

		protected override void AfterValid()
		{
			base.AfterValid();

			if (clearTreeAfterComplete)
			{
				ResetRows();
			}
		}
	}
}