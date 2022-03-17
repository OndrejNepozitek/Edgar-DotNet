using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Utils.ConfigParsing;

namespace Edgar.GUI.Legacy
{
    /// <summary>
    /// Windows that shows main settings of the layout generator.
    /// </summary>
    public partial class MainSettings : Form
    {
        private GeneratorWindow generatorWindow;
        private readonly Random random = new Random();
        private readonly ConfigLoader configLoader = new ConfigLoader();
        private MapDescription<int> mapDescription;

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
            if (mapDescription == null)
            {
                ShowSettingsError("Map description not chosen");
                return;
            }

            generatorWindow = new GeneratorWindow(new GeneratorSettings()
            {
                RandomGeneratorSeed = useRandomSeedCheckbox.Checked ? random.Next() : (int) generatorSeedInput.Value,
                NumberOfLayouts = (int) numberOfLayoutsInput.Value,

                MapDescription = mapDescription,

                ShowFinalLayouts = showFinalLayouts.Checked,
                ShowFinalLayoutsTime = (int) showFinalLayoutsTime.Value,
                ShowPartialValidLayouts = showPartialValidLayouts.Checked,
                ShowPartialValidLayoutsTime = (int) showPartialValidLayoutsTime.Value,
                ShowPerturbedLayouts = showPerturbedLayouts.Checked,
                ShowPerturbedLayoutsTime = (int) showPerturbedLayoutsTime.Value,
                ExportShownLayouts = exportShownLayoutsCheckbox.Checked,

                UseOldPaperStyle = useOldPaperStyleCheckbox.Checked,
                ShowRoomNames = showRoomNamesCheckbox.Checked,

                FixedFontSize = fixedFontSizeCheckbox.Checked,
                FixedFontSizeValue = (int) fixedFontSizeValue.Value,
                FidexSquareExport = fixedSquareExportCheckbox.Checked,
                FixedSquareExportValue = (int) fixedSquareExportValue.Value,

                FixedPositionsAndScale = fixedPositionsAndScaleCheckbox.Checked,
                FixedPositionsAndScaleValue = fixedPositionsAndScaleValue.Value,
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
                ShowSettingsError(
                    $"Map description could not be loaded. Exception: {exception.Message}. Inner exception: {exception.InnerException}");
                return;
            }

            usingUploaded = false;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            descriptionNotChosen.Hide();
            var graph = mapDescription.GetGraph();

            usedDescription.Text = usingUploaded
                ? $"Using uploaded map description file."
                : $"Using map description file from Resources.";
            usedDescriptionRoomsCount.Text = $"Number of rooms: {graph.VerticesCount}";
            usedDescriptionPassagesCount.Text = $"Number of passages: {graph.Edges.Count()}";

            usedDescriptionInfoPanel.Show();
        }
    }
}