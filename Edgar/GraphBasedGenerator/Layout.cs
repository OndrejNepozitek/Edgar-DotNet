using System;
using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Core.Configurations;
using MapGeneration.Core.Layouts.Interfaces;

namespace Edgar.GraphBasedGenerator
{
    public class Layout<TRoom, TConfiguration> : ILayout<RoomNode<TRoom>, TConfiguration>
    {
        public IGraph<RoomNode<TRoom>> Graph => throw new NotSupportedException();

        public Layout()
        {

        }

        public bool GetConfiguration(RoomNode<TRoom> node, out TConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public void SetConfiguration(RoomNode<TRoom> node, TConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveConfiguration(RoomNode<TRoom> node)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TConfiguration> GetAllConfigurations()
        {
            throw new System.NotImplementedException();
        }
    }
}