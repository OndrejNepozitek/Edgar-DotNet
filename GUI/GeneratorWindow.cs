namespace GUI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using MapDrawing;
	using MapGeneration.Interfaces.Core;
	using MapGeneration.Interfaces.Utils;
	using MapGeneration.Utils;
	using MapGeneration.Utils.MapDrawing;

	public partial class GeneratorWindow : Form
	{
		private readonly GeneratorSettings settings;

		private readonly WFLayoutDrawer<int> wfLayoutDraver = new WFLayoutDrawer<int>();
		private readonly SVGLayoutDrawer<int> svgLayoutDrawer = new SVGLayoutDrawer<int>();
		private readonly OldMapDrawer<int> oldMapDrawer = new OldMapDrawer<int>();

		private Task task;
		private CancellationTokenSource cancellationTokenSource;

		private bool isRunning = true;
		private int layoutsCount;
		private int iterationsCount;
		private readonly Stopwatch infoStopwatch = new Stopwatch();

		private IMapLayout<int> layoutToDraw;
		private List<IMapLayout<int>> generatedLayouts;
		private int slideshowIndex;
		private int slideshowTaskId;

		public GeneratorWindow(GeneratorSettings settings)
		{
			this.settings = settings;

			InitializeComponent();

			showFinalLayouts.Checked = settings.ShowFinalLayouts;
			showFinalLayoutsTime.Value = settings.ShowFinalLayoutsTime;
			showPartialValidLayouts.Checked = settings.ShowPartialValidLayouts;
			showAcceptedLayoutsTime.Value = settings.ShowPartialValidLayoutsTime;
			showPerturbedLayouts.Checked = settings.ShowPerturbedLayouts;
			showPerturbedLayoutsTime.Value = settings.ShowPerturbedLayoutsTime;
			showRoomNamesCheckbox.Checked = settings.ShowRoomNames;
			useOldPaperStyleCheckbox.Checked = settings.UseOldPaperStyle;

			slideshowPanel.Hide();
			exportPanel.Hide();
			actionsPanel.Hide();

			Run();
		}

		private void Run()
		{
			cancellationTokenSource = new CancellationTokenSource();
			var ct = cancellationTokenSource.Token;
			task = Task.Run(() =>
			{
				var layoutGenerator = settings.LayoutGenerator;

				if (layoutGenerator == null)
				{
					if (settings.MapDescription.IsWithCorridors)
					{
						var defaultGenerator = LayoutGeneratorFactory.GetSALayoutGeneratorWithCorridors(settings.MapDescription.CorridorsOffsets);
						defaultGenerator.InjectRandomGenerator(new Random(settings.RandomGeneratorSeed));

						layoutGenerator = defaultGenerator;
					}
					else
					{
						var defaultGenerator = LayoutGeneratorFactory.GetDefaultSALayoutGenerator();
						defaultGenerator.InjectRandomGenerator(new Random(settings.RandomGeneratorSeed));

						layoutGenerator = defaultGenerator;
					}
				}

				if (layoutGenerator is ICancellable cancellable)
				{
					cancellable.SetCancellationToken(ct);
				}
					
				infoStopwatch.Start();

				layoutGenerator.OnValid += layout =>
				{
					if (!showFinalLayouts.Checked)
						return;

					layoutToDraw = layout;
					mainPictureBox.BeginInvoke((Action)(() => mainPictureBox.Refresh()));
					SleepWithFastCancellation((int)showFinalLayoutsTime.Value, ct);
				};

				layoutGenerator.OnPartialValid += layout =>
				{
					if (!showPartialValidLayouts.Checked)
						return;

					layoutToDraw = layout;
					mainPictureBox.BeginInvoke((Action)(() => mainPictureBox.Refresh()));
					SleepWithFastCancellation((int)showAcceptedLayoutsTime.Value, ct);
				};

				layoutGenerator.OnPerturbed += layout =>
				{
					if (!showPerturbedLayouts.Checked)
						return;

					layoutToDraw = layout;
					mainPictureBox.BeginInvoke((Action)(() => mainPictureBox.Refresh()));
					SleepWithFastCancellation((int)showPerturbedLayoutsTime.Value, ct);
				};

				layoutGenerator.OnPerturbed += layout =>
				{
					iterationsCount++;
					if (infoStopwatch.ElapsedMilliseconds >= 200)
					{
						BeginInvoke((Action)(UpdateInfoPanel));
						infoStopwatch.Restart();
					}
				};

				layoutGenerator.OnValid += layout =>
				{
					iterationsCount = 0;
					layoutsCount++;
					BeginInvoke((Action)(UpdateInfoPanel));
					infoStopwatch.Restart();
				};

				generatedLayouts = (List<IMapLayout<int>>) layoutGenerator.GetLayouts(settings.MapDescription, settings.NumberOfLayouts);

				isRunning = false;
				BeginInvoke((Action)(UpdateInfoPanel));
				BeginInvoke((Action)(OnFinished));
			}, ct);
		}

		private void SleepWithFastCancellation(int ms, CancellationToken ct)
		{
			const int timeSpan = 100;
			var leftover = ms % timeSpan;
			var numberOfIntervals = ms / timeSpan;

			for (var i = 0; i < numberOfIntervals; i++)
			{
				if (ct.IsCancellationRequested)
					return;

				Thread.Sleep(timeSpan);
			}

			if (ct.IsCancellationRequested)
				return;

			Thread.Sleep(leftover);
		}

		private void mainPictureBox_Paint(object sender, PaintEventArgs e)
		{
			if (layoutToDraw == null)
				return;

			var showNames = showRoomNamesCheckbox.Checked;
			var useOldPaperStyle = useOldPaperStyleCheckbox.Checked;

			if (useOldPaperStyle)
			{
				var bitmap = oldMapDrawer.DrawLayout(layoutToDraw, mainPictureBox.Width, mainPictureBox.Height, showNames);
				e.Graphics.DrawImage(bitmap, new Point(0, 0));
			}
			else
			{
				wfLayoutDraver.DrawLayout(layoutToDraw, mainPictureBox, e, showNames);
			}
		}

		private void UpdateInfoPanel()
		{
			infoStatus.Text = $"Status: {(isRunning ? "running" : "completed")}";
			infoGeneratingLayout.Text = $"{layoutsCount + 1}/{settings.NumberOfLayouts}";
			infoIterations.Text = $"{iterationsCount}";

			if (!isRunning)
			{
				infoGeneratingLayout.Hide();
				infoIterations.Hide();
				infoIterationsLabel.Hide();

				// infoGeneratingLayoutLabel.Hide();
				infoGeneratingLayoutLabel.Text =
					$"Layouts generated: {layoutsCount}. Layouts requested: {settings.NumberOfLayouts}.";
			}
		}

		private void OnFinished()
		{
			automaticSlideshowCheckbox.Checked = true;

			slideshowPanel.Show();
			exportPanel.Show();
			actionsPanel.Show();
		}

		private void GeneratorWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (task != null)
			{
				cancellationTokenSource.Cancel();
				task.Wait();
			}

			slideshowTaskId++;
		}

		private void GeneratorWindow_Resize(object sender, EventArgs e)
		{
			mainPictureBox.Refresh();
		}

		private void slideshowLeftButton_Click(object sender, EventArgs e)
		{
			automaticSlideshowCheckbox.Checked = false;

			if (slideshowIndex != 0)
			{
				layoutToDraw = generatedLayouts[--slideshowIndex];
				mainPictureBox.Refresh();
			}

			UpdateSlideshowInfo();
		}

		private void slideshowRightButton_Click(object sender, EventArgs e)
		{
			automaticSlideshowCheckbox.Checked = false;

			if (slideshowIndex != generatedLayouts.Count - 1)
			{
				layoutToDraw = generatedLayouts[++slideshowIndex];
				mainPictureBox.Refresh();
			}

			UpdateSlideshowInfo();
		}

		private void UpdateSlideshowInfo()
		{
			currentlyShowLayoutLabel.Text = $"Currently shown layout: {slideshowIndex + 1}";
		}

		private void automaticSlideshowCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (automaticSlideshowCheckbox.Checked)
			{
				var id = ++slideshowTaskId;

				Task.Run(() =>
				{
					for (var i = slideshowIndex; i < generatedLayouts.Count; i++)
					{
						if (slideshowTaskId != id || !automaticSlideshowCheckbox.Checked)
							return;

						var idToShow = i;

						Invoke((Action) (() =>
						{
							slideshowIndex = idToShow;
							UpdateSlideshowInfo();
							layoutToDraw = generatedLayouts[idToShow];
							mainPictureBox.Refresh();
						}));

						Thread.Sleep(3000);
					}
				});
			}
		}

		private void exportSvgButton_Click(object sender, EventArgs e)
		{
			automaticSlideshowCheckbox.Checked = false;
			UpdateSlideshowInfo();
			saveExportDialog.DefaultExt = "svg";

			if (saveExportDialog.ShowDialog() == DialogResult.OK)
			{
				var filename = saveExportDialog.FileName;

				using (var fs = File.Open(filename, FileMode.Create))
				{
					using (var sw = new StreamWriter(fs))
					{
						var data = svgLayoutDrawer.DrawLayout(layoutToDraw, 800, showRoomNamesCheckbox.Checked);
						sw.Write(data);
					}
				}
			}
		}

		private void showRoomNamesCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			mainPictureBox.Refresh();
		}

		private void useOldPaperStyleCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			mainPictureBox.Refresh();
		}
	}
}
