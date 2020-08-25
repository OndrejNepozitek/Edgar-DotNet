using System.Drawing;
using System.Linq;
using Edgar.GraphBasedGenerator.Grid2D;

namespace Edgar.Examples.MapDrawing
{
    public class GeneratorSummaryDrawer<TRoom>
    {
        private Bitmap bitmap;
        private Graphics graphics;
        private GraphBasedGeneratorGrid2D<TRoom> generator;
        private readonly DungeonDrawer<TRoom> dungeonDrawer = new DungeonDrawer<TRoom>();
        private LevelDescriptionGrid2D<TRoom> levelDescription;
        private int totalWidth;

        public Bitmap Draw(LevelDescriptionGrid2D<TRoom> levelDescription, int width, GraphBasedGeneratorGrid2D<TRoom> generator)
        {
            bitmap = new Bitmap(width, width);
            graphics = Graphics.FromImage(bitmap);
            this.generator = generator;
            this.levelDescription = levelDescription;
            this.totalWidth = width;

            var headingHeight = 0.1f;

            DrawBackground();
            DrawHeading(GetSubRect(GetMainRect(), 0f, 0f, 1f, headingHeight));
            DrawLayout1(GetSubRect(GetMainRect(), 0f, headingHeight + 0.025f, 1f, 1 - headingHeight - 0.025f));

            graphics.Dispose();
            return bitmap;
        }

        private void DrawHeading(RectangleF rect)
        {
            var mainHeight = 0.35f;
            var color = Color.FromArgb(50, 50, 50);

            var rectMain = GetSubRect(rect, 0f, 0.2f, 1f, mainHeight);
            var rectSub = GetSubRect(rect, 0f, 0.2f + mainHeight + 0.1f, 1f, 0.2f);

            using (var font = new Font("Baskerville Old Face", rectMain.Height, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };

                graphics.DrawString("Edgar", font, new SolidBrush(color), rectMain, sf);
            }

            using (var font = new Font("Baskerville Old Face", rectSub.Height, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };

                graphics.DrawString("Graph-based procedural level generator for .NET and Unity", font, new SolidBrush(color), rectSub, sf);
            }
        }

        private void DrawLayout1(RectangleF rect)
        {
            DrawRoomTemplates(GetSubRect(rect, 0f, 0f, 0.5f, 0.5f));
            DrawOutput(GetSubRect(rect,0f, 0.5f, 1/3f, 0.5f));
            DrawOutput(GetSubRect(rect,1/3f, 0.5f, 1/3f, 0.5f));
            DrawOutput(GetSubRect(rect,2/3f, 0.5f, 1/3f, 0.5f));
        }

        private void DrawBackground()
        {
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(248, 248, 244)), GetMainRect());
        }

        private void DrawRoomTemplates(RectangleF rect)
        {
            var roomTemplates = levelDescription
                .GetGraph().Vertices
                .Select(levelDescription.GetRoomDescription)
                .Where(x => x.IsCorridor == false)
                .SelectMany(x => x.RoomTemplates)
                .Distinct()
                .ToList();
            var roomTemplatesDrawer = new RoomTemplateDrawer<TRoom>();
            var roomTemplatesBitmap = roomTemplatesDrawer.DrawRoomTemplates(roomTemplates, (int) rect.Width, (int) rect.Height, true,1.5f, 0.2f);
            graphics.DrawImage(roomTemplatesBitmap, rect);
        }

        private void DrawOutput(RectangleF rect)
        {
            var level = generator.GenerateLayout();
            var drawing = dungeonDrawer.DrawLayout(level, (int) rect.Width, (int) rect.Height, true, 1.5f);
            graphics.DrawImage(drawing, rect);
        }

        private RectangleF GetMainRect()
        {
            return new RectangleF(0, 0, totalWidth, totalWidth);
        }

        private RectangleF GetSubRect(RectangleF rect, float xFrom, float yFrom, float width, float height)
        {
            return new RectangleF(rect.Left + xFrom * rect.Width, rect.Top + yFrom * rect.Height, rect.Width * width, rect.Height * height);
        }
    }
}