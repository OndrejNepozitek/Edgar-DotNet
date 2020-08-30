using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization.Configurations;

namespace Edgar.Legacy.Utils.MetaOptimization.Mutations.MaxStageTwoFailures
{
    public class MaxStageTwoFailuresMutation<TConfiguration> : IMutation<TConfiguration>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
    {

        public int Priority { get; }

        public SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; }

        public MaxStageTwoFailuresStrategy Strategy { get; }

        public double MinValue { get; }

        public double Multiplier { get; }

        public MaxStageTwoFailuresMutation(int priority, SimulatedAnnealingConfigurationProvider simulatedAnnealingConfiguration, MaxStageTwoFailuresStrategy strategy, double minValue, double multiplier)
        {
            Priority = priority;
            SimulatedAnnealingConfiguration = simulatedAnnealingConfiguration;
            Strategy = strategy;
            MinValue = minValue;
            Multiplier = multiplier;
        }
        public TConfiguration Apply(TConfiguration configuration)
        {
            var copy = configuration.SmartClone();
            copy.SimulatedAnnealingConfiguration = SimulatedAnnealingConfiguration;

            return copy;
        }

        public override string ToString()
        {
            return $"SAMaxStageTwoFailures with {Strategy} strategy, priority {Priority}, min {MinValue}, mul {Multiplier}";
        }

        #region Equals

        protected bool Equals(MaxStageTwoFailuresMutation<TConfiguration> other)
        {
            return Strategy == other.Strategy && MinValue.Equals(other.MinValue) && Multiplier.Equals(other.Multiplier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MaxStageTwoFailuresMutation<TConfiguration>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Strategy;
                hashCode = (hashCode * 397) ^ MinValue.GetHashCode();
                hashCode = (hashCode * 397) ^ Multiplier.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(MaxStageTwoFailuresMutation<TConfiguration> left, MaxStageTwoFailuresMutation<TConfiguration> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MaxStageTwoFailuresMutation<TConfiguration> left, MaxStageTwoFailuresMutation<TConfiguration> right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}