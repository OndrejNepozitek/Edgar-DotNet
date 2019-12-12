using MapGeneration.Interfaces.Utils;

namespace MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing
{
    public class SimulatedAnnealingConfiguration : ISmartCloneable<SimulatedAnnealingConfiguration>
    {
        public int Cycles { get; }

        public int TrialsPerCycle { get; }

        public int MaxIterationsWithoutSuccess { get; }

        public SimulatedAnnealingConfiguration(int cycles, int trialsPerCycle, int maxIterationsWithoutSuccess)
        {
            Cycles = cycles;
            TrialsPerCycle = trialsPerCycle;
            MaxIterationsWithoutSuccess = maxIterationsWithoutSuccess;
        }

        public static SimulatedAnnealingConfiguration GetDefaultConfiguration()
        {
            return new SimulatedAnnealingConfiguration(50, 100, 10000);
        }

        public SimulatedAnnealingConfiguration SmartClone()
        {
            return new SimulatedAnnealingConfiguration(Cycles, TrialsPerCycle, MaxIterationsWithoutSuccess);
        }

        #region Equals

        protected bool Equals(SimulatedAnnealingConfiguration other)
        {
            return Cycles == other.Cycles && TrialsPerCycle == other.TrialsPerCycle && MaxIterationsWithoutSuccess == other.MaxIterationsWithoutSuccess;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SimulatedAnnealingConfiguration)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Cycles;
                hashCode = (hashCode * 397) ^ TrialsPerCycle;
                hashCode = (hashCode * 397) ^ MaxIterationsWithoutSuccess;
                return hashCode;
            }
        }

        public static bool operator ==(SimulatedAnnealingConfiguration left, SimulatedAnnealingConfiguration right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SimulatedAnnealingConfiguration left, SimulatedAnnealingConfiguration right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}