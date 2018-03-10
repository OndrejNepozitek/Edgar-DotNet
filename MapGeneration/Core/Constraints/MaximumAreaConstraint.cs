namespace MapGeneration.Core.Constraints
{
	using System;
	using System.Linq;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.Configuration.EnergyData;
	using Interfaces.Core.Constraints;
	using Interfaces.Core.Layouts;

	public class MaximumAreaConstraint<TLayout, TNode, TConfiguration, TShapeContainer, TLayoutEnergyData> : ILayoutConstraint<TLayout, TNode, TLayoutEnergyData>
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IConfiguration<TShapeContainer>
		where TLayoutEnergyData : IEnergyData
	{
		private readonly int maximumArea;

		public MaximumAreaConstraint(int maximumArea)
		{
			this.maximumArea = maximumArea;
		}

		public bool ComputeLayoutEnergyData(TLayout layout, ref TLayoutEnergyData energyData)
		{
			var boundingRectangles = layout.GetAllConfigurations().Select(x => x.Shape.BoundingRectangle + x.Position).ToList();
			var minX = boundingRectangles.Min(x => x.A.X);
			var maxX = boundingRectangles.Max(x => x.B.X);
			var minY = boundingRectangles.Min(x => x.A.Y);
			var maxY = boundingRectangles.Max(x => x.B.Y);

			var width = maxX - minX;
			var height = maxY - minY;
			var area = width * height;

			if (area > maximumArea)
			{
				energyData.Energy += (float)(Math.Pow(Math.E, area / (float) maximumArea) - 1);
				return false;
			}

			return true;
		}

		public bool UpdateLayoutEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TLayoutEnergyData energyData)
		{
			return ComputeLayoutEnergyData(newLayout, ref energyData);
		}
	}
}