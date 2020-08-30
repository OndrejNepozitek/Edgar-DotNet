using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MapDrawing;
using Edgar.Legacy.Utils.Serialization;
using GUI.MapDrawing;

namespace Edgar.GUI.New
{
    /// <summary>
	/// Window that shows progress of the layout generator. 
	/// </summary>
	public partial class GeneratorWindow : Form
	{
		private readonly GeneratorSettings settings;

		private readonly WFLayoutDrawer<int> wfLayoutDrawer = new WFLayoutDrawer<int>();
		private readonly SVGLayoutDrawer<int> svgLayoutDrawer = new SVGLayoutDrawer<int>();
		private readonly OldMapDrawer<int> oldMapDrawer = new OldMapDrawer<int>();
        private readonly JsonSerializer<int> jsonSerializer = new JsonSerializer<int>();

		private Task task;
		private CancellationTokenSource cancellationTokenSource;

		private bool isRunning = true;
		private int layoutsCount;
		private int iterationsCount;
		private readonly Stopwatch infoStopwatch = new Stopwatch();

		private LayoutGrid2D<int> layoutToDraw;
		private LayoutGrid2D<int> firstChainToDraw;
		private List<LayoutGrid2D<int>> generatedLayouts;
		private int slideshowIndex;
		private int slideshowTaskId;

		private const int DefaultWidth = 600;
		private const int DefaultHeight = 600;

		private string dumpFolder;
		private int dumpCount;
		private GeneratorEvent lastEvent;

		private float fixedScaleRatio = 1f;
		private float fixedScale;
		private Vector2Int fixedOffset;

		public GeneratorWindow(GeneratorSettings settings)
		{
			this.settings = settings;

			InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

			showFinalLayouts.Checked = settings.ShowFinalLayouts;
			showFinalLayoutsTime.Value = settings.ShowFinalLayoutsTime;
			showPartialValidLayouts.Checked = settings.ShowPartialValidLayouts;
			showAcceptedLayoutsTime.Value = settings.ShowPartialValidLayoutsTime;
			showPerturbedLayouts.Checked = settings.ShowPerturbedLayouts;
			showPerturbedLayoutsTime.Value = settings.ShowPerturbedLayoutsTime;
			showRoomNamesCheckbox.Checked = settings.ShowRoomNames;
			useOldPaperStyleCheckbox.Checked = settings.UseOldPaperStyle;
			exportShownLayoutsCheckbox.Checked = settings.ExportShownLayouts;

			fixedFontSizeCheckbox.Checked = settings.FixedFontSize;
			fixedFontSizeValue.Value = settings.FixedFontSizeValue;
			fixedSquareExportCheckbox.Checked = settings.FidexSquareExport;
			fixedSquareExportValue.Value = settings.FixedSquareExportValue;
			fixedPositionsAndScaleCheckbox.Checked = settings.FixedPositionsAndScale;
			fixedPositionsAndScaleValue.Value = settings.FixedPositionsAndScaleValue;

			slideshowPanel.Hide();
			exportPanel.Hide();
			actionsPanel.Hide();

			Run();
		}

		/// <summary>
		/// Run the generator.
		/// </summary>
		private void Run()
		{
			cancellationTokenSource = new CancellationTokenSource();
			var ct = cancellationTokenSource.Token;
			task = Task.Run(() =>
			{
				try
				{
					dumpFolder = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
					dumpCount = 0;
					var layoutGenerator = settings.LayoutGenerator;

					if (layoutGenerator == null)
					{
						var defaultGenerator = new GraphBasedGeneratorGrid2D<int>(settings.LevelDescription);
						defaultGenerator.InjectRandomGenerator(new Random(settings.RandomGeneratorSeed));
                        layoutGenerator = defaultGenerator;
                    }

					// Set cancellation token
					if (layoutGenerator is ICancellable cancellable)
					{
						cancellable.SetCancellationToken(ct);
					}

					infoStopwatch.Start();

					// Register handler that shows generated layouts OnValid
					layoutGenerator.OnValid += layout =>
					{
						if (!showFinalLayouts.Checked)
							return;

						lastEvent = GeneratorEvent.OnValid;
						layoutToDraw = layout;
						mainPictureBox.BeginInvoke((Action) (() => mainPictureBox.Refresh()));
						SleepWithFastCancellation((int) showFinalLayoutsTime.Value, ct);
					};

					// Register handler that shows generated layouts OnPartialValid
					layoutGenerator.OnPartialValid += layout =>
					{
						if (!showPartialValidLayouts.Checked)
							return;

						lastEvent = GeneratorEvent.OnPartialValid;
						layoutToDraw = layout;
						mainPictureBox.BeginInvoke((Action) (() => mainPictureBox.Refresh()));
						SleepWithFastCancellation((int) showAcceptedLayoutsTime.Value, ct);
					};

					// Register handler that shows generated layouts OnPerturbed
					layoutGenerator.OnPerturbed += layout =>
					{
						if (!showPerturbedLayouts.Checked)
							return;

						lastEvent = GeneratorEvent.OnPerturbed;
						layoutToDraw = layout;
						mainPictureBox.BeginInvoke((Action) (() => mainPictureBox.Refresh()));
						SleepWithFastCancellation((int) showPerturbedLayoutsTime.Value, ct);
					};

					// Register handler that counts iteration count
					layoutGenerator.OnPerturbed += layout =>
					{
						lastEvent = GeneratorEvent.OnPerturbed;
						iterationsCount++;
						if (infoStopwatch.ElapsedMilliseconds >= 200)
						{
							BeginInvoke((Action) (UpdateInfoPanel));
							infoStopwatch.Restart();
						}
					};

					// Register handler that resets iteration count
					layoutGenerator.OnValid += layout =>
					{
						lastEvent = GeneratorEvent.OnValid;
						iterationsCount = 0;
						layoutsCount++;
						BeginInvoke((Action) (UpdateInfoPanel));
						infoStopwatch.Restart();
					};

					generatedLayouts = new List<LayoutGrid2D<int>>() { };

                    for (int i = 0; i < settings.NumberOfLayouts; i++)
                    {
                        generatedLayouts.Add(layoutGenerator.GenerateLayout());
                    }

					isRunning = false;
					BeginInvoke((Action) (UpdateInfoPanel));
					BeginInvoke((Action) (OnFinished));
				}
				catch (Exception e)
				{
					ShowExceptionAndClose(e);
				}
			}, ct);
		}

		private void ShowExceptionAndClose(Exception e)
		{
			BeginInvoke((Action)(() =>
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
			}));
		}

		/// <summary>
		/// Smarter thread sleep that checks cancellation token and ends prematurelly if needed.
		/// </summary>
		/// <param name="ms"></param>
		/// <param name="ct"></param>
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

			if (exportShownLayoutsCheckbox.Checked)
				DumpSvg();

			UpdateLayoutType();

			var showNames = showRoomNamesCheckbox.Checked;
			var useOldPaperStyle = useOldPaperStyleCheckbox.Checked;
			var fixedFontSize = fixedFontSizeCheckbox.Checked ? (int?)fixedFontSizeValue.Value : null;

			if (firstChainToDraw == null)
			{
				firstChainToDraw = layoutToDraw;
				RecomputeFixedScaleAndOffset();
			}
			else
			{
				var previousRoomsCount = firstChainToDraw.Rooms.Count();
				var currentRoomsCount = layoutToDraw.Rooms.Count();

				if (previousRoomsCount == currentRoomsCount)
				{
					firstChainToDraw = layoutToDraw;
					RecomputeFixedScaleAndOffset();
				}
			}

			if (useOldPaperStyle)
			{
				//var bitmap = oldMapDrawer.DrawLayout(layoutToDraw, mainPictureBox.Width, mainPictureBox.Height, showNames, fixedFontSize);
				//e.Graphics.DrawImage(bitmap, new Point(0, 0));
				throw new NotSupportedException();
			}
			else
			{
				//Bitmap bitmap;

				//if (fixedPositionsAndScaleCheckbox.Checked)
				//{
				//	bitmap = wfLayoutDrawer.DrawLayout(layoutToDraw, mainPictureBox.Width, mainPictureBox.Height, showNames, fixedFontSize, fixedScale, fixedOffset);
				//}
				//else
				//{
				//	bitmap = wfLayoutDrawer.DrawLayout(layoutToDraw, mainPictureBox.Width, mainPictureBox.Height, showNames, fixedFontSize);
				//}

				var layoutDrawer = new DungeonDrawer<int>();
				var bitmap = layoutDrawer.DrawLayout(layoutToDraw, new DungeonDrawerOptions()
                {
					Width = mainPictureBox.Width,
					Height = mainPictureBox.Height,
					ShowRoomNames = showNames,
					PaddingPercentage = 0.05f,
					EnableHatching = true,
                });

                // e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				e.Graphics.DrawImage(bitmap, new Point(0, 0));
            }
		}

		private void RecomputeFixedScaleAndOffset()
		{
			var layout = firstChainToDraw;

			if (layout == null)
				return;

			var polygons = layout.Rooms.Select(x => x.Outline + x.Position).ToList();
			var points = polygons.SelectMany(x => x.GetPoints()).ToList();

			var minx = points.Min(x => x.X);
			var miny = points.Min(x => x.Y);
			var maxx = points.Max(x => x.X);
			var maxy = points.Max(x => x.Y);

			fixedScale = (float)fixedPositionsAndScaleValue.Value / 100 * wfLayoutDrawer.GetScale(minx, miny, maxx, maxy, mainPictureBox.Width, mainPictureBox.Height);
			fixedOffset = wfLayoutDrawer.GetOffset(minx, miny, maxx, maxy, mainPictureBox.Width, mainPictureBox.Height, fixedScale);
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

		private void UpdateLayoutType()
		{
			if ((showPartialValidLayouts.Checked || showPerturbedLayouts.Checked) && showFinalLayouts.Checked)
			{
				switch (lastEvent)
				{
					case GeneratorEvent.OnPerturbed:
						infoLayoutType.Text = "";
						break;

					case GeneratorEvent.OnPartialValid:
						infoLayoutType.Text = "";
						break;

					case GeneratorEvent.OnValid:
						infoLayoutType.Text = "Final layout";
						break;
				}
			}
			else
			{
				infoLayoutType.Text = "";
			}
		}

		private void OnFinished()
		{
			automaticSlideshowCheckbox.Checked = true;

			slideshowPanel.Show();
			exportPanel.Show();
			actionsPanel.Show();
			infoLayoutType.Hide();
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
			RecomputeFixedScaleAndOffset();
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

			var width = fixedSquareExportCheckbox.Checked ? (int)fixedSquareExportValue.Value : DefaultWidth;
			var fixedFontSize = fixedFontSizeCheckbox.Checked ? (int?)fixedFontSizeValue.Value : null;

			if (saveExportDialog.ShowDialog() == DialogResult.OK)
			{
				var filename = saveExportDialog.FileName;

				using (var fs = File.Open(filename, FileMode.Create))
				{
					using (var sw = new StreamWriter(fs))
					{
						throw new NotSupportedException();
						//var data = svgLayoutDrawer.DrawLayout(layoutToDraw, width, showRoomNamesCheckbox.Checked, fixedFontSize, fixedSquareExportCheckbox.Checked);
						//sw.Write(data);
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

		private void exportJsonButton_Click(object sender, EventArgs e)
		{
			automaticSlideshowCheckbox.Checked = false;
			UpdateSlideshowInfo();
			saveExportDialog.DefaultExt = "json";

			if (saveExportDialog.ShowDialog() == DialogResult.OK)
			{
				var filename = saveExportDialog.FileName;

				using (var fs = File.Open(filename, FileMode.Create))
				{
					using (var sw = new StreamWriter(fs))
					{
                        throw new NotSupportedException();
                        // jsonSerializer.Serialize(layoutToDraw, sw);
					}
				}
			}
		}

		private void exportAllJsonButton_Click(object sender, EventArgs e)
		{
			saveExportDialog.DefaultExt = "json";

			if (saveExportDialog.ShowDialog() == DialogResult.OK)
			{
				var filename = saveExportDialog.FileName;

				using (var fs = File.Open(filename, FileMode.Create))
				{
					using (var sw = new StreamWriter(fs))
					{
                        throw new NotSupportedException();
                        // jsonSerializer.Serialize(layoutToDraw, sw);
					}
				}
			}
		}

		private void DumpSvg()
		{
			var width = fixedSquareExportCheckbox.Checked ? (int)fixedSquareExportValue.Value : DefaultWidth;
			var fixedFontSize = fixedFontSizeCheckbox.Checked ? (int?)fixedFontSizeValue.Value : null;
			var folder = $"Output/{dumpFolder}";
			var filename = $"{folder}/{dumpCount++}_{lastEvent.ToString()}.svg";
			Directory.CreateDirectory(folder);

			using (var fs = File.Open(filename, FileMode.Create))
			{
				using (var sw = new StreamWriter(fs))
				{
                    throw new NotSupportedException();
					//var data = svgLayoutDrawer.DrawLayout(layoutToDraw, width, showRoomNamesCheckbox.Checked, fixedFontSize, fixedSquareExportCheckbox.Checked);
					//sw.Write(data);
				}
			}
		}

		private void exportAllJpgButton_Click(object sender, EventArgs e)
		{
			var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
			var folder = $"Output/{time}";
			var showNames = showRoomNamesCheckbox.Checked;
			var useOldPaperStyle = useOldPaperStyleCheckbox.Checked;

			var width = fixedSquareExportCheckbox.Checked ? (int) fixedSquareExportValue.Value : DefaultWidth;
			var height = fixedSquareExportCheckbox.Checked ? (int) fixedSquareExportValue.Value : DefaultHeight;
			var fixedFontSize = fixedFontSizeCheckbox.Checked ? (int?) fixedFontSizeValue.Value : null;

			Directory.CreateDirectory(folder);

			for (var i = 0; i < generatedLayouts.Count; i++)
			{
				Bitmap bitmap;

                throw new NotSupportedException();

				//if (useOldPaperStyle)
				//{
				//	bitmap = oldMapDrawer.DrawLayout(generatedLayouts[i], width, height, showNames, fixedFontSize);
				//}
				//else
				//{
				//	bitmap = wfLayoutDrawer.DrawLayout(generatedLayouts[i], width, height, showNames, fixedFontSize);
				//}

				//bitmap.Save($"{folder}/{i}.jpg");
			}

			MessageBox.Show($"Images were saved to {folder}", "Images saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void fixedFontSizeCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			mainPictureBox.Refresh();
		}

		private void fixedSquareExportCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			mainPictureBox.Refresh();
		}

		private void fixedFontSizeValue_ValueChanged(object sender, EventArgs e)
		{
			if (fixedFontSizeCheckbox.Checked)
			{
				mainPictureBox.Refresh();
			}
		}

		private void fixedSquareExportValue_ValueChanged(object sender, EventArgs e)
		{
			if (fixedSquareExportCheckbox.Checked)
			{
				mainPictureBox.Refresh();
			}
		}

		private enum GeneratorEvent
		{
			OnPerturbed, OnPartialValid, OnValid
		}

		private void fixedPositionsAndScaleValue_ValueChanged(object sender, EventArgs e)
		{
			RecomputeFixedScaleAndOffset();
		}
	}
}
