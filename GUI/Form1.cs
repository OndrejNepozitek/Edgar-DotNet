namespace GUI
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapDrawing;
	using MapGeneration.Core;
	using MapGeneration.Core.ConfigurationSpaces;
	using MapGeneration.Core.Doors;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.GraphDecomposition;
	using MapGeneration.Core.Interfaces;
	using MapGeneration.Utils;

	public partial class Form1 : Form
	{
		private IMapLayout<int> layout;
		private readonly WFLayoutDrawer<int> layoutDrawer = new WFLayoutDrawer<int>();

		public Form1()
		{
			InitializeComponent();

		}

		private void canvas_Paint(object sender, PaintEventArgs e)
		{
			if (layout == null)
			{
				return;
			}

			layoutDrawer.DrawLayout(layout, canvas, e);
		}

		private void button1_Click(object sender, System.EventArgs e)
		{

			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler,
				new OrthogonalLineIntersection(), new GridPolygonUtils());

			/*var squareRoom = new RoomDescription(GridPolygon.GetSquare(6), new OverlapMode(1, 0));
			var squareRoom2 = new RoomDescription(GridPolygon.GetSquare(3), new OverlapMode(1, 0));
			var rectangleRoom = new RoomDescription(GridPolygon.GetRectangle(3, 5), new OverlapMode(1, 0));

			var mapDescription = new MapDescription<int>();

			mapDescription.AddRoom(0);
			mapDescription.AddRoom(1);
			mapDescription.AddRoom(2);
			mapDescription.AddRoom(3);
			mapDescription.AddRoom(4);

			mapDescription.AddPassage(0, 1);
			mapDescription.AddPassage(1, 2);
			mapDescription.AddPassage(2, 3);
			mapDescription.AddPassage(3, 0);
			mapDescription.AddPassage(3, 4);

			mapDescription.AddRoomShapes(squareRoom);
			mapDescription.AddRoomShapes(squareRoom2);
			mapDescription.AddRoomShapes(rectangleRoom);*/


			var layoutGenerator = new SALayoutGenerator<int>();
			layoutGenerator.InjectRandomGenerator(new Random(0));
			layoutGenerator.EnableDebugOutput(true);
			layoutGenerator.SetChainDecomposition(new LongerChainsDecomposition<int>(new GraphDecomposer<int>()));
			layoutGenerator.SetChainDecomposition(new BreadthFirstLongerChainsDecomposition<int>());

			/*layoutGenerator.OnPerturbed += (l) =>
			{
				layout = l;
				canvas.Invoke((Action)(() => canvas.Refresh()));
				Thread.Sleep(25);
			};*/

			/*layoutGenerator.OnValid += (l) =>
			{
				layout = l;
				canvas.Invoke((Action)(() => canvas.Refresh()));
				Thread.Sleep(100);
			};*/

			/*layoutGenerator.OnValidAndDifferent += (l) =>
			{
				layout = l;
				canvas.Invoke((Action)(() => canvas.Refresh()));
				Thread.Sleep(200);
			};*/

			Task.Run(() =>
			{
				for (int i = 0; i < 1; i++)
				{
					var mapDescription = MapDescriptionsDatabase.Reference_9Vertices_WithoutRoomShapes;
					MapDescriptionsDatabase.AddClassicRoomShapes(mapDescription);

					var layouts = layoutGenerator.GetLayouts(mapDescription);

					foreach (var layout in layouts)
					{
						this.layout = layout;
						canvas.Invoke((Action)(() => canvas.Refresh()));
						Thread.Sleep(2000);
					}
				}
			});
		}
	}
}
