using System.Collections.Generic;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Common
{
    public class Layout<TRoom, TConfiguration> : ILayout<RoomNode<TRoom>, TConfiguration>, ISmartCloneable<Layout<TRoom, TConfiguration>>
        where TConfiguration: ISmartCloneable<TConfiguration>
    {
        private readonly TConfiguration[] vertices;
        private readonly bool[] hasValue;

        public IGraph<RoomNode<TRoom>> Graph { get; }

        /// <summary>
        /// Construct a layout with a given graph and no configurations.
        /// </summary>
        /// <param name="graph"></param>
        public Layout(IGraph<RoomNode<TRoom>> graph)
        {
            Graph = graph;
            vertices = new TConfiguration[Graph.VerticesCount];
            hasValue = new bool[Graph.VerticesCount];
        }

        /// <inheritdoc />
        public bool GetConfiguration(RoomNode<TRoom> node, out TConfiguration configuration)
        {
            if (hasValue[node.Id])
            {
                configuration = vertices[node.Id];
                return true;
            }

            configuration = default(TConfiguration);
            return false;
        }

        /// <inheritdoc />
        public void SetConfiguration(RoomNode<TRoom> node, TConfiguration configuration)
        {
            vertices[node.Id] = configuration;
            hasValue[node.Id] = true;
        }

        /// <inheritdoc />
        public void RemoveConfiguration(RoomNode<TRoom> node)
        {
            vertices[node.Id] = default(TConfiguration);
            hasValue[node.Id] = false;
        }

        /// <inheritdoc />
        public IEnumerable<TConfiguration> GetAllConfigurations()
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                if (hasValue[i])
                {
                    yield return vertices[i];
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Smart clones all configurations.
        /// Graph is not smart cloned.
        /// </summary>
        /// <returns></returns>
        public Layout<TRoom, TConfiguration> SmartClone()
        {
            var layout = new Layout<TRoom, TConfiguration>(Graph);

            for (var i = 0; i < vertices.Length; i++)
            {
                var configuration = vertices[i];

                if (hasValue[i])
                {
                    layout.vertices[i] = configuration.SmartClone();
                    layout.hasValue[i] = true;
                }
            }

            return layout;
        }
    }
}