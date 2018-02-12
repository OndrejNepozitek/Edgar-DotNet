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
			layoutGenerator.EnableDifferenceFromAvg(true, 0.3f);
			layoutGenerator.EnableLazyProcessing(true);

			//layoutGenerator.OnPerturbed += (l) =>
			//{
			//	layout = l;
			//	canvas.Invoke((Action)(() => canvas.Refresh()));
			//	Thread.Sleep(50);
			//};

			//layoutGenerator.OnValid += (l) =>
			//{
			//	layout = l;
			//	canvas.Invoke((Action)(() => canvas.Refresh()));
			//	Thread.Sleep(500);
			//};

			/*layoutGenerator.OnValidAndDifferent += (l) =>
			{
				layout = l;
				canvas.Invoke((Action)(() => canvas.Refresh()));
				Thread.Sleep(200);
			};*/

			Task.Run(() =>
			{
				for (int i = 0; i < 10; i++)
				{
					var mapDescription = MapDescriptionsDatabase.Reference_41Vertices_WithoutRoomShapes;
					MapDescriptionsDatabase.AddClassicRoomShapes(mapDescription, new IntVector2(1, 1));

					/*mapDescription.AddRoomShapes(7, new List<RoomDescription>()
					{
						new RoomDescription(
							GridPolygon.GetSquare(5),
							new SpecificPositionsMode(new List<OrthogonalLine>()
							{
								new OrthogonalLine(new IntVector2(2, 0), new IntVector2(3, 0)),
								new OrthogonalLine(new IntVector2(0, 2), new IntVector2(0, 3)),
								new OrthogonalLine(new IntVector2(2, 5), new IntVector2(3, 5)),
								new OrthogonalLine(new IntVector2(5, 2), new IntVector2(5, 3)),
							})
						)
					});*/

					mapDescription.AddRoomShapes(40, new List<RoomDescription>()
					{
						new RoomDescription(
							new GridPolygonBuilder()
								.AddPoint(2, 0)
								.AddPoint(2, 1)
								.AddPoint(1, 1)
								.AddPoint(1, 2)
								.AddPoint(0, 2)
								.AddPoint(0, 7)
								.AddPoint(1, 7)
								.AddPoint(1, 8)
								.AddPoint(2, 8)
								.AddPoint(2, 9)
								.AddPoint(7, 9)
								.AddPoint(7, 8)
								.AddPoint(8, 8)
								.AddPoint(8, 7)
								.AddPoint(9, 7)
								.AddPoint(9, 2)
								.AddPoint(8, 2)
								.AddPoint(8, 1)
								.AddPoint(7, 1)
								.AddPoint(7, 0)
								.Build()
							,
							new OverlapMode(1, 2)
						)
					});

					var layouts = layoutGenerator.GetLayouts(mapDescription, 1);

					foreach (var layout in layouts)
					{
						this.layout = layout;
						canvas.Invoke((Action)(() => canvas.Refresh()));
						Thread.Sleep(3000);
					}
				}
			});
		}
	}
}
