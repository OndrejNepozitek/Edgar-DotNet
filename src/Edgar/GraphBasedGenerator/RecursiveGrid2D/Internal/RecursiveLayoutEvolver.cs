using System;
using System.Collections.Generic;
using System.Threading;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.LayoutEvolvers.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.RecursiveGrid2D.Internal
{
    public class RecursiveLayoutEvolver<TRoom> :
        ILayoutEvolver<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>, RoomNode<TRoom>>, IRandomInjectable,
        ICancellable
    {
        private readonly LevelDescriptionGrid2D<TRoom> levelDescription;
        private readonly ILevelDescription<RoomNode<TRoom>> levelDescriptionMapped;
        private readonly LevelGeometryData<RoomNode<TRoom>> geometryData;
        private readonly List<Cluster<RoomNode<TRoom>>> clusters;
        private readonly RoomTemplateInstanceGrid2D dummyRoomTemplateInstance;
        public event EventHandler<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> OnPerturbed;
        public event EventHandler<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> OnValid;

        private Random random;
        private CancellationToken? cancellationToken;

        public RecursiveLayoutEvolver(LevelDescriptionGrid2D<TRoom> levelDescription,
            ILevelDescription<RoomNode<TRoom>> levelDescriptionMapped, LevelGeometryData<RoomNode<TRoom>> geometryData,
            List<Cluster<RoomNode<TRoom>>> clusters, RoomTemplateInstanceGrid2D dummyRoomTemplateInstance)
        {
            this.levelDescription = levelDescription;
            this.levelDescriptionMapped = levelDescriptionMapped;
            this.geometryData = geometryData;
            this.clusters = clusters;
            this.dummyRoomTemplateInstance = dummyRoomTemplateInstance;
        }

        public IEnumerable<Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>>> Evolve(
            Layout<TRoom, ConfigurationGrid2D<TRoom, EnergyData>> initialLayout,
            Chain<RoomNode<TRoom>> chain,
            int count)
        {
            // TODO: add timeouts
            var initialLayoutCopy = initialLayout.SmartClone();
            var dummyLevelDescription = new LevelDescriptionGrid2D<RoomNode<TRoom>>();
            dummyLevelDescription.MinimumRoomDistance = levelDescription.MinimumRoomDistance;
            dummyLevelDescription.RoomTemplateRepeatModeDefault = levelDescription.RoomTemplateRepeatModeDefault;
            dummyLevelDescription.RoomTemplateRepeatModeOverride = levelDescription.RoomTemplateRepeatModeOverride;

            var generator = new InnerGenerator<TRoom>(levelDescriptionMapped, dummyLevelDescription, chain.Nodes,
                initialLayoutCopy, geometryData, clusters[chain.Number], dummyRoomTemplateInstance,
                new GraphBasedGeneratorConfiguration<RoomNode<TRoom>>()
                {
                    // EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(1),
                    // EarlyStopIfIterationsExceeded = 3000,
                    // EarlyStopIfIterationsExceeded = 2000,
                    EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(3.5d),
                });
            generator.InjectRandomGenerator(random);
            // generator.SetCancellationToken(cancellationToken);

            OnPerturbed?.Invoke(this, initialLayoutCopy);

            var reportedFail = false;

            for (int i = 0; i < 1; i++)
            {
                var layout = generator.GenerateLayout();

                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                {
                    yield break;
                }

                if (layout != null)
                {
                    //OnValid?.Invoke(this, layout);
                    yield return layout;
                }
                else
                {
                    if (chain.Number != 0 && !reportedFail)
                    {
                        // TODO: change
                        //OnValid?.Invoke(this, initialLayout);
                        reportedFail = true;
                    }

                    // TODO: improve
                    // Break on first not successful
                    yield break;
                }

                // TODO: remove
                OnPerturbed?.Invoke(this, initialLayoutCopy);
            }
        }

        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
        }

        public void SetCancellationToken(CancellationToken? cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }
    }
}