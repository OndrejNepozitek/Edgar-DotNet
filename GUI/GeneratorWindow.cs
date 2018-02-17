namespace GUI
{
	using System;
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
						BeginInvoke((Action)(updateInfoPanel));
						infoStopwatch.Restart();
					}
				};

				layoutGenerator.OnFinal += layout =>
				{
					iterationsCount = 0;
					layoutsCount++;
					BeginInvoke((Action)(updateInfoPanel));
					infoStopwatch.Restart();
				};

				var layouts = layoutGenerator.GetLayouts(settings.MapDescription, settings.NumberOfLayouts);

				isRunning = false;
				BeginInvoke((Action)(updateInfoPanel));

				foreach (var layout in layouts)
				{
					if (ct.IsCancellationRequested)
						break;

					layoutToDraw = layout;
					mainPictureBox.Invoke((Action) (() => mainPictureBox.Refresh()));
					SleepWithFastCancellation(3000, ct);
				}
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

		private void updateInfoPanel()
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
	}
}
