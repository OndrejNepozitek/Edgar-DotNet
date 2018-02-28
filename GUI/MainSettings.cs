namespace GUI
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Windows.Forms;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Interfaces.Core;
	using MapGeneration.Interfaces.Core.MapDescription;
	using MapGeneration.Utils.ConfigParsing;

	public partial class MainSettings : Form
	{
		private readonly ConfigLoader configLoader = new ConfigLoader();
		private GeneratorWindow generatorWindow;
		private readonly Random random = new Random();

		private IMapDescription<int> mapDescription;
		private bool usingUploaded = false;

		public MainSettings()
		{
			InitializeComponent();
			InitializeForm();
		}

		private void InitializeForm()
		{
			var mapDescriptionFiles = configLoader.GetSavedMapDescriptionsNames();
			mapDescriptionFiles.ForEach(x => loadedMapDescriptionsComboBox.Items.Add(x));
		}

		private void generateButton_Click(object sender, EventArgs e)
		{
			if (loadedMapDescriptionsComboBox == null)
			{
				ShowSettingsError("Map description not chosen");
			}

			((MapDescription<int>) mapDescription).SetWithCorridors(true); // TODO: remove

			{
				var corridor1 = new RoomDescription(
					GridPolygon.GetRectangle(1, 1)
					, new OverlapMode(1, 0));

				((MapDescription<int>)mapDescription).AddCorridorShapes(corridor1);
			}

			{
				var corridor1 = new RoomDescription(
					GridPolygon.GetRectangle(2, 1)
					, new OverlapMode(1, 0));

				((MapDescription<int>)mapDescription).AddCorridorShapes(corridor1);
			}

			{
				var corridor1 = new RoomDescription(
					GridPolygon.GetRectangle(3, 1)
					, new OverlapMode(1, 0));

				((MapDescription<int>)mapDescription).AddCorridorShapes(corridor1);
			}

			{
				var corridor1 = new RoomDescription(
					GridPolygon.GetRectangle(4, 1)
					, new OverlapMode(1, 0));

				((MapDescription<int>)mapDescription).AddCorridorShapes(corridor1);
			}


			generatorWindow = new GeneratorWindow(new GeneratorSettings()
			{
				MapDescription = mapDescription,
				RandomGeneratorSeed = useRandomSeedCheckbox.Checked ? random.Next() : (int) generatorSeedInput.Value,
				NumberOfLayouts = (int) numberOfLayoutsInput.Value,

				ShowFinalLayouts = showFinalLayouts.Checked,
				ShowFinalLayoutsTime = (int) showFinalLayoutsTime.Value,
				ShowPartialValidLayouts = showPartialValidLayouts.Checked,
				ShowPartialValidLayoutsTime = (int) showPartialValidLayoutsTime.Value,
				ShowPerturbedLayouts = showPerturbedLayouts.Checked,
				ShowPerturbedLayoutsTime = (int) showPerturbedLayoutsTime.Value,
			});
			
			generatorWindow.Show();
		}

		private void ShowSettingsError(string message)
		{
			MessageBox.Show(message, "Settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void uploadButton_Click(object sender, EventArgs e)
		{
			if (mapDescriptionFileDialog.ShowDialog() == DialogResult.OK)
			{
				var filename = mapDescriptionFileDialog.FileName;

				using (var sr = new StreamReader(filename))
				{
					try
					{
						mapDescription = configLoader.LoadMapDescription(sr);
					}
					catch (Exception exception)
					{
						ShowSettingsError($"Map description could not be loaded. Exception: {exception.Message}");
						return;
					}

					usingUploaded = true;
					UpdateInfo();
				}
			}
		}

		private void loadedMapDescriptionsComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var mapDescriptionFile = (string) loadedMapDescriptionsComboBox.SelectedItem;

			try
			{
				mapDescription = configLoader.LoadMapDescriptionFromResources(mapDescriptionFile);
			}
			catch (Exception exception)
			{
				ShowSettingsError($"Map description could not be loaded. Exception: {exception.Message}");
				return;
			}

			usingUploaded = false;
			UpdateInfo();
		}

		private void UpdateInfo()
		{
			descriptionNotChosen.Hide();
			var graph = mapDescription.GetGraph();

			usedDescription.Text = usingUploaded ? $"Using uploaded map description file." : $"Using map description file from Resources.";
			usedDescriptionRoomsCount.Text = $"Number of rooms: {graph.VerticesCount}";
			usedDescriptionPassagesCount.Text = $"Number of passages: {graph.Edges.Count()}";

			usedDescriptionInfoPanel.Show();
		}
	}
}
