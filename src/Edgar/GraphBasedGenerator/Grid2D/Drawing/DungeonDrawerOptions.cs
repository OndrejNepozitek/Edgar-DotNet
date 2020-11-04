using System.Drawing;

namespace Edgar.GraphBasedGenerator.Grid2D.Drawing
{
    /// <summary>
    /// Options for the <see cref="DungeonDrawer{TRoom}"/> class.
    /// </summary>
    /// <remarks>
    /// It is possible to omit some of the <see cref="Width"/>, <see cref="Height"/> and <see cref="Scale"/> properties.
    /// 
    /// The drawer works like this: <br />
    /// If the width is set and the height is not set (and vice versa), the height is computed automatically based on the width/height ratio of the layout. <br />
    /// If the scale is not set, it is computed automatically in a way that the layout fits the image.
    /// </remarks>
    public class DungeonDrawerOptions
    {
        /// <summary>
        /// Width of the drawing (in pixels).
        /// </summary>
        public int? Width { get; set; } = null;

        /// <summary>
        /// Height of the drawing (in pixels).
        /// </summary>
        public int? Height { get; set; } = null;

        /// <summary>
        /// Scaling that will be applied to the layout.
        /// </summary>
        /// <remarks>
        /// It is automatically computed to fit the the drawing if left null.
        /// </remarks>
        /// <example>
        /// Value 1.0: Layout is not scaled. Each point on the outline of each room is represented with a single pixel. <br/>
        /// Value 10.0: Rooms are 10 times larger. 10 pixels are used to represent the line between two neighboring points. <br/>
        /// </example>
        public float? Scale { get; set; } = null;

        /// <summary>
        /// The size (in pixels) of the padding (empty space) on each side of the drawing.
        /// </summary>
        /// <remarks>
        /// Defaults to null. When null, the <see cref="PaddingPercentage"/> is used instead.
        /// Shading/hatching is not taken into account when computing the padding. That means that when padding is too small,
        /// parts of shading might be outside of the picture. At least 10% of the width of the drawing is recommended.
        /// </remarks>
        /// <example>
        /// Value 100: There will be 100px gap on each side of the drawing.
        /// </example>
        public int? PaddingAbsolute { get; set; }

        /// <summary>
        /// The size (fraction of the minimum of width and height of the drawing) of the padding on each side of the drawing.
        /// </summary>
        /// <remarks>
        /// This property is only used when <see cref="PaddingAbsolute"/> is null.
        /// Shading/hatching is not taken into account when computing the padding. That means that when padding is too small,
        /// parts of shading might be outside of the picture. At least 10% of the width of the drawing is recommended.
        /// </remarks>
        /// <example>
        /// Value 0.1f and width set to 1000px and height set to 500px: There will be 50px (10% of 500px) gap on each side of the drawing.
        /// </example>
        public float PaddingPercentage { get; set; } = 0.15f;

        /// <summary>
        /// Whether to show room names or not.
        /// </summary>
        /// <remarks>
        /// Corridor names are never shown because there is usually not enough space to write the name.
        /// </remarks>
        public bool ShowRoomNames { get; set; } = true;

        /// <summary>
        /// The size (in tiles) of the font that is used to display room names.
        /// </summary>
        /// <example>
        /// Value x: The font will have approximately the same height as x tiles in the image.
        /// </example>
        public float FontSize { get; set; } = 2;

        /// <summary>
        /// Whether to enable shading.
        /// </summary>
        public bool EnableShading { get; set; } = true;

        /// <summary>
        /// Whether to enable hatching.
        /// </summary>
        public bool EnableHatching { get; set; } = true;

        /// <summary>
        /// Whether to enable grid lines.
        /// </summary>
        public bool EnableGridLines { get; set; } = true;

        /// <summary>
        /// Background color of individual rooms.
        /// </summary>
        public Color RoomBackgroundColor { get; set; } = Color.FromArgb(248, 248, 244);

        /// <summary>
        /// Background color of the image.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.FromArgb(248, 248, 244);

        public Range<float> HatchingLength { get; set; } = new Range<float>(0.5f, 0.6f);

        public Range<float> HatchingClusterOffset { get; set; } = new Range<float>(0.15f, 0.3f);
    }
}