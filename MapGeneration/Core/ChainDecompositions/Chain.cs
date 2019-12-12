using System.Collections.Generic;
using System.Linq;
using MapGeneration.Interfaces.Core.ChainDecompositions;

namespace MapGeneration.Core.ChainDecompositions
{
    public class Chain<TNode> : IChain<TNode>
    {
        public List<TNode> Nodes { get; }

        public int Number { get; }

        public Chain(List<TNode> nodes, int number)
        {
            Nodes = nodes;
            Number = number;
        }

        #region Equals

        protected bool Equals(Chain<TNode> other)
        {
            return Nodes.SequenceEqual(other.Nodes) && Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Chain<TNode>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Nodes != null ? Nodes.GetHashCode() : 0) * 397) ^ Number;
            }
        }

        public static bool operator ==(Chain<TNode> left, Chain<TNode> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Chain<TNode> left, Chain<TNode> right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}