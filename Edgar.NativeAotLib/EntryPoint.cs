using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Utils;

namespace Edgar.NativeAotLib {
    public static class EntryPoint
    {
        [UnmanagedCallersOnly(EntryPoint = "GenerateLevel", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GenerateLevel(IntPtr levelDescriptionJsonPtr, int seed)
        {
            try
            {
                var levelDescriptionJson = Marshal.PtrToStringAnsi(levelDescriptionJsonPtr) ?? string.Empty;
                var levelDescription = SystemTextJsonUtils.DeserializeFromJson(levelDescriptionJson)!;

                var random = new Random(seed);
                var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
                generator.InjectRandomGenerator(random);

                var layout = generator.GenerateLayout();
                var layoutJson = SystemTextJsonUtils.SerializeToJson(layout);

                var result = Marshal.StringToHGlobalAnsi(layoutJson);

                return result;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "GenerateLevelDummy")]
        public static IntPtr GenerateLevelDummy(IntPtr levelDescriptionJsonPtr, int seed)
        {
            var levelDescriptionJson = Marshal.PtrToStringAnsi(levelDescriptionJsonPtr) ?? string.Empty;
            var result = Marshal.StringToHGlobalAnsi(levelDescriptionJson);

            return result;
        }

        [UnmanagedCallersOnly(EntryPoint = "RunNativeEdgar")]
        public static int RunNativeEdgar(int count)
        {
            var graph = GraphsDatabase.GetExample4();
            var roomTemplates = new List<RoomTemplateGrid2D>()
            {
                new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(10), new SimpleDoorModeGrid2D(1, 1)),
            };

            var levelDescription = new LevelDescriptionGrid2D<int>();
            foreach (var graphVertex in graph.Vertices)
            {
                levelDescription.AddRoom(graphVertex, new RoomDescriptionGrid2D(false, roomTemplates));
            }

            foreach (var graphEdge in graph.Edges)
            {
                levelDescription.AddConnection(graphEdge.From, graphEdge.To);
            }

            var random = new Random(0);
            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            generator.InjectRandomGenerator(random);
            var successCount = 0;

            for (int i = 0; i < count; i++)
            {
                var level = generator.GenerateLayout();
                if (level != null)
                {
                    successCount++;
                }
            }

            return successCount;
        }
    }

}

