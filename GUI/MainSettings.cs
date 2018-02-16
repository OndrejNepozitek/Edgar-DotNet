using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
	using MapGeneration.Core.Interfaces;
	using MapGeneration.Utils.ConfigParsing;

	public partial class MainSettings : Form
	{
		private readonly ConfigLoader configLoader = new ConfigLoader();
		private GeneratorWindow generatorWindow;
		private readonly Random random = new Random();

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

			var mapDescriptionFile = (string) loadedMapDescriptionsComboBox.SelectedItem;
			IMapDescription<string> mapDescription;

			try
			{
				mapDescription = configLoader.LoadMapDescriptionFromResources(mapDescriptionFile);
			}
			catch (Exception exception)
			{
				ShowSettingsError($"Map description could not be loaded. Exception: {exception.Message}");
				return;
			}

			generatorWindow = new GeneratorWindow(new GeneratorSettings()
			{
				MapDescription = mapDescription,
				RandomGeneratorSeed = useRandomSeedCheckbox.Checked ? random.Next() : (int) generatorSeedInput.Value,
				NumberOfLayouts = (int) numberOfLayoutsInput.Value,

				ShowFinalLayouts = showFinalLayouts.Checked,
				ShowFinalLayoutsTime = (int) showFinalLayoutsTime.Value,
				ShowAcceptedLayouts = showAcceptedLayouts.Checked,
				ShowAcceptedLayoutsTime = (int) showAcceptedLayoutsTime.Value,
				ShowPerturbedLayouts = showPerturbedLayouts.Checked,
				ShowPerturbedLayoutsTime = (int) showPerturbedLayoutsTime.Value,
			});
			
			generatorWindow.Show();
		}

		private void ShowSettingsError(string message)
		{
			MessageBox.Show(message, "Settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
