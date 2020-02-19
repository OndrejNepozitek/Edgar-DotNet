using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing
{
    public class SimulatedAnnealingConfigurationProvider : ISmartCloneable<SimulatedAnnealingConfigurationProvider>
    {
        private readonly List<SimulatedAnnealingConfiguration> configurationsForChains;

        public SimulatedAnnealingConfigurationProvider(List<SimulatedAnnealingConfiguration> configurationsForChains)
        {
            this.configurationsForChains = configurationsForChains;
        }

        public SimulatedAnnealingConfiguration GetConfiguration(int chainNumber)
        {
            if (chainNumber < 0)
            {
                throw new ArgumentException($"{nameof(chainNumber)} must not be negative", nameof(chainNumber));
            }

            if (chainNumber >= configurationsForChains.Count)
            {
                throw new ArgumentException($"{nameof(chainNumber)} is larger than the number chains", nameof(chainNumber));
            }

            return configurationsForChains[chainNumber];
        }

        public List<SimulatedAnnealingConfiguration> GetAllConfigurations()
        {
            return configurationsForChains.ToList();
        }

        public SimulatedAnnealingConfigurationProvider SmartClone()
        {
            return new SimulatedAnnealingConfigurationProvider(configurationsForChains.Select(x => x.SmartClone()).ToList());
        }

        #region Equals

        protected bool Equals(SimulatedAnnealingConfigurationProvider other)
        {
            return configurationsForChains.SequenceEqual(other.configurationsForChains);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SimulatedAnnealingConfigurationProvider)obj);
        }

        public override int GetHashCode()
        {
            return (configurationsForChains != null ? configurationsForChains.GetHashCode() : 0);
        }

        public static bool operator ==(SimulatedAnnealingConfigurationProvider left, SimulatedAnnealingConfigurationProvider right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SimulatedAnnealingConfigurationProvider left, SimulatedAnnealingConfigurationProvider right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}