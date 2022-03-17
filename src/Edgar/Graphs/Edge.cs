using System;
using System.Collections.Generic;

namespace Edgar.Graphs
{
    /// <summary>
    /// Class representing an edge of a graph.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Edge<T> : IEdge<T>, IEquatable<Edge<T>>
    {
        /// <summary>
        /// First vertex of the edge.
        /// </summary>
        public T From { get; }

        /// <summary>
        /// Second vertex of the edge.
        /// </summary>
        public T To { get; }

        public Edge(T from, T to)
        {
            From = from;
            To = to;
        }

        public bool Equals(Edge<T> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(From, other.From) &&
                   EqualityComparer<T>.Default.Equals(To, other.To);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Edge<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(From) * 397) ^
                       EqualityComparer<T>.Default.GetHashCode(To);
            }
        }
    }
}