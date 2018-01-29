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
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapDrawing;
	using MapGeneration.Core;
	using MapGeneration.Core.ConfigurationSpaces;
	using MapGeneration.Core.Doors;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.Interfaces;

	public partial class Form1 : Form
	{
		private IMapLayout<int> layout;
		private readonly LayoutDrawer layoutDrawer = new LayoutDrawer();

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

			var scale = new IntVector2(3, 3);
			var squareRoom = new RoomDescription(GridPolygon.GetSquare(6).Scale(scale), new OverlapMode(1, 0));
			var squareRoom2 = new RoomDescription(GridPolygon.GetSquare(3).Scale(scale), new OverlapMode(1, 0));
			var rectangleRoom = new RoomDescription(GridPolygon.GetRectangle(3, 5).Scale(scale), new OverlapMode(1, 0));
			var room1 = new RoomDescription(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 4)
					.AddPoint(2, 4)
					.AddPoint(2, 2)
					.AddPoint(6, 2)
					.AddPoint(6, 0)
					.Build().Scale(scale)
				, new OverlapMode(1, 0));
			var room2 = new RoomDescription(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 4)
					.AddPoint(2, 4)
					.AddPoint(2, 2)
					.AddPoint(4, 2)
					.AddPoint(4, 0)
					.Build().Scale(scale)
				, new OverlapMode(1, 0));
			var room3 = new RoomDescription(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 2)
					.AddPoint(2, 2)
					.AddPoint(2, 4)
					.AddPoint(4, 4)
					.AddPoint(4, 2)
					.AddPoint(6, 2)
					.AddPoint(6, 0)
					.Build().Scale(scale)
				, new OverlapMode(1, 0));

			var mapDescription = new MapDescription<int>();
			Enumerable.Range(0, 17).ToList().ForEach(x => mapDescription.AddRoom(x));

			mapDescription.AddPassage(0, 2);
			mapDescription.AddPassage(0, 3);
			mapDescription.AddPassage(1, 2);
			mapDescription.AddPassage(1, 9);
			mapDescription.AddPassage(2, 5);
			mapDescription.AddPassage(3, 6);
			mapDescription.AddPassage(4, 7);
			mapDescription.AddPassage(4, 8);
			mapDescription.AddPassage(5, 6);
			mapDescription.AddPassage(5, 10);
			mapDescription.AddPassage(6, 11);
			mapDescription.AddPassage(7, 12);
			mapDescription.AddPassage(8, 13);
			mapDescription.AddPassage(9, 10);
			mapDescription.AddPassage(11, 12);
			mapDescription.AddPassage(11, 14);
			mapDescription.AddPassage(12, 15);
			mapDescription.AddPassage(12, 16);
			mapDescription.AddPassage(13, 16);
			mapDescription.AddPassage(14, 15);

			// mapDescription.AddRoomShapes(squareRoom);
			mapDescription.AddRoomShapes(squareRoom2, true, 4);
			mapDescription.AddRoomShapes(rectangleRoom, true, 2);
			mapDescription.AddRoomShapes(room1);
			mapDescription.AddRoomShapes(room2);
			mapDescription.AddRoomShapes(room3);
			// mapDescription.AddRoomShapes(lShapeRoom);

			var configurationSpaces = configurationSpacesGenerator.Generate(mapDescription);
			var layoutGenerator = new SALayoutGenerator<int>(configurationSpaces);
			layoutGenerator.InjectRandomGenerator(new Random(0));

			/*layoutGenerator.OnPerturbed += (l) =>
			{
				layout = l;
				canvas.Invoke((Action)(() => canvas.Refresh()));
				Thread.Sleep(150);
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
