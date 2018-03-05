namespace MapGeneration.Core.GeneratorPlanners
{
	using System.Collections.Generic;
	using System.Linq;

	/// <inheritdoc />
	/// <summary>
	/// Basic layout generator that always picks an unfinished node on the deepest level of the tree.
	/// </summary>
	public class BasicGeneratorPlanner<TLayout> : GeneratorPlannerBase<TLayout>
	{
		private readonly bool clearTreeAfterComplete;

		/// <summary>
		/// </summary>
		/// <param name="clearTreeAfterComplete">Whether the tree should be cleared after each complete layout that is generated.</param>
		public BasicGeneratorPlanner(bool clearTreeAfterComplete = true)
		{
			this.clearTreeAfterComplete = clearTreeAfterComplete;
		}

		/// <summary>
		/// Alaways chooses a not finished layout on the highest level.
		/// </summary>
		/// <param name="rows"></param>
		/// <returns></returns>
		protected override Node GetNextInstance(List<NodeRow> rows)
		{
			var depth = rows.Count - 1;

			while (depth >= 0)
			{
				var row = rows[depth];
				var instance = row.Instances.FirstOrDefault(x => !x.IsFinished);

				if (instance == null)
				{
					depth--;
					continue;
				}

				return instance;
			}

			return AddZeroLevelNode();
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