using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization.Configurations;

namespace Edgar.Legacy.Utils.MetaOptimization.Mutations.MaxBranching
{
    public class MaxBranchingMutation<TConfiguration> : IMutation<TConfiguration>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
    {
        public int Priority { get; }

        public int SimulatedAnnealingMaxBranching { get; }

        public MaxBranchingMutation(int priority, int simulatedAnnealingMaxBranching)
        {
            Priority = priority;
            SimulatedAnnealingMaxBranching = simulatedAnnealingMaxBranching;
        }
        public TConfiguration Apply(TConfiguration configuration)
        {
            var copy = configuration.SmartClone();
            copy.SimulatedAnnealingMaxBranching = SimulatedAnnealingMaxBranching;

            return copy;
        }

        public override string ToString()
        {
            return $"MaxBranching {SimulatedAnnealingMaxBranching}, priority {Priority}";
        }

        #region Equals

        protected bool Equals(MaxBranchingMutation<TConfiguration> other)
        {
            return SimulatedAnnealingMaxBranching == other.SimulatedAnnealingMaxBranching;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MaxBranchingMutation<TConfiguration>) obj);
        }

        public override int GetHashCode()
        {
            return SimulatedAnnealingMaxBranching;
        }

        public static bool operator ==(MaxBranchingMutation<TConfiguration> left, MaxBranchingMutation<TConfiguration> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MaxBranchingMutation<TConfiguration> left, MaxBranchingMutation<TConfiguration> right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}