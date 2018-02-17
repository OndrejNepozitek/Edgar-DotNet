namespace GUI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using MapDrawing;
	using MapGeneration.Core;
	using MapGeneration.Core.Interfaces;

	public partial class GeneratorWindow : Form
	{
		private readonly GeneratorSettings settings;
		private readonly WFLayoutDrawer<int> layoutDrawer = new WFLayoutDrawer<int>();

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
			showAcceptedLayouts.Checked = settings.ShowAcceptedLayouts;
			showAcceptedLayoutsTime.Value = settings.ShowAcceptedLayoutsTime;
			showPerturbedLayouts.Checked = settings.ShowPerturbedLayouts;
			showPerturbedLayoutsTime.Value = settings.ShowPerturbedLayoutsTime;

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
				var layoutGenerator = new SALayoutGenerator<int>();
				layoutGenerator.EnableLazyProcessing(true);
				layoutGenerator.SetCancellationToken(ct);
				layoutGenerator.InjectRandomGenerator(new Random(settings.RandomGeneratorSeed));
				infoStopwatch.Start();

				layoutGenerator.OnFinal += layout =>
				{
					if (!showFinalLayouts.Checked)
						return;

					layoutToDraw = layout;
					mainPictureBox.BeginInvoke((Action) (() => mainPictureBox.Refresh()));
					SleepWithFastCancellation((int) showFinalLayoutsTime.Value, ct);
				};

				layoutGenerator.OnValidAndDifferent += layout =>
				{
					if (!showAcceptedLayouts.Checked)
						return;

					layoutToDraw = layout;
					mainPictureBox.BeginInvoke((Action)(() => mainPictureBox.Refresh()));
					SleepWithFastCancellation((int) showAcceptedLayoutsTime.Value, ct);
				};

				layoutGenerator.OnPerturbed += layout =>
				{
					if (!showPerturbedLayouts.Checked)
						return;

					layoutToDraw = layout;
					mainPictureBox.BeginInvoke((Action)(() => mainPictureBox.Refresh()));
					SleepWithFastCancellation((int) showPerturbedLayoutsTime.Value, ct);
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

				layoutGenerator.OnFinal += layout =>
				{
					iterationsCount = 0;
					layoutsCount++;
					BeginInvoke((Action)(UpdateInfoPanel));
					infoStopwatch.Restart();
				};

				generatedLayouts = (List<IMapLayout<int>>) layoutGenerator.GetLayouts(settings.MapDescription, settings.NumberOfLayouts);

				isRunning = false;
				Invoke((Action)(UpdateInfoPanel));
				Invoke((Action)(OnFinished));

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

			layoutDrawer.DrawLayout(layoutToDraw, mainPictureBox, e);
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
		}

		private void GeneratorWindow_Resize(object sender, EventArgs e)
		{
			mainPictureBox.Refresh();
		}

		private void slideshowLeftButton_Click(object sender, EventArgs e)
		{
			if (slideshowIndex == 0)
				return;

			layoutToDraw = generatedLayouts[--slideshowIndex];
			mainPictureBox.Refresh();
			UpdateSlideshowInfo();
			automaticSlideshowCheckbox.Checked = false;
		}

		private void slideshowRightButton_Click(object sender, EventArgs e)
		{
			if (slideshowIndex == generatedLayouts.Count - 1)
				return;

			layoutToDraw = generatedLayouts[++slideshowIndex];
			mainPictureBox.Refresh();
			UpdateSlideshowInfo();
			automaticSlideshowCheckbox.Checked = false;
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
						if (!automaticSlideshowCheckbox.Checked || slideshowTaskId != id)
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
	}
}
