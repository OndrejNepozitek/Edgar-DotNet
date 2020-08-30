using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing
{
    /// <summary>
    /// Configuration of simulated annealing
    /// </summary>
    public class SimulatedAnnealingConfiguration : ISmartCloneable<SimulatedAnnealingConfiguration>
    {
        public int Cycles { get; set; } = 50;

        public int TrialsPerCycle { get; set; } = 100;

        public int MaxIterationsWithoutSuccess { get; set; } = 100;

        public int MaxStageTwoFailures { get; set; } = 10000;

        public bool HandleTreesGreedily { get; set; } = true;

        public SimulatedAnnealingConfiguration()
        {

        }

        public SimulatedAnnealingConfiguration(int cycles, int trialsPerCycle, int maxIterationsWithoutSuccess, int maxStageTwoFailures)
        {
            Cycles = cycles;
            TrialsPerCycle = trialsPerCycle;
            MaxIterationsWithoutSuccess = maxIterationsWithoutSuccess;
            MaxStageTwoFailures = maxStageTwoFailures;
        }

        public static SimulatedAnnealingConfiguration GetDefaultConfiguration()
        {
            return new SimulatedAnnealingConfiguration(50, 100, 100, 10000)
            {
                HandleTreesGreedily = true,
            };
        }

        public SimulatedAnnealingConfiguration SmartClone()
        {
            return new SimulatedAnnealingConfiguration(Cycles, TrialsPerCycle, MaxIterationsWithoutSuccess, MaxStageTwoFailures);
        }

        #region Equals

        protected bool Equals(SimulatedAnnealingConfiguration other)
        {
            return Cycles == other.Cycles && TrialsPerCycle == other.TrialsPerCycle && MaxIterationsWithoutSuccess == other.MaxIterationsWithoutSuccess && MaxStageTwoFailures == other.MaxStageTwoFailures;
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