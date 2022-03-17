using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing
{
    /// <summary>
    /// Simulated annealing configuration provider. Provides configurations for individual chains.
    /// </summary>
    public class SimulatedAnnealingConfigurationProvider : ISmartCloneable<SimulatedAnnealingConfigurationProvider>
    {
        private readonly List<SimulatedAnnealingConfiguration> configurationsForChains;
        private readonly SimulatedAnnealingConfiguration fixedConfiguration;
        private readonly bool useFixedConfiguration;

        public SimulatedAnnealingConfigurationProvider(List<SimulatedAnnealingConfiguration> configurationsForChains)
        {
            this.configurationsForChains = configurationsForChains ??
                                           throw new ArgumentNullException(nameof(configurationsForChains));
            useFixedConfiguration = false;
        }

        public SimulatedAnnealingConfigurationProvider(SimulatedAnnealingConfiguration fixedConfiguration)
        {
            this.fixedConfiguration = fixedConfiguration ?? throw new ArgumentNullException(nameof(fixedConfiguration));
            useFixedConfiguration = true;
        }

        public SimulatedAnnealingConfiguration GetConfiguration(int chainNumber)
        {
            if (chainNumber < 0)
            {
                throw new ArgumentException($"{nameof(chainNumber)} must not be negative", nameof(chainNumber));
            }

            if (useFixedConfiguration)
            {
                return fixedConfiguration;
            }

            if (chainNumber >= configurationsForChains.Count)
            {
                throw new ArgumentException($"{nameof(chainNumber)} is larger than the number chains",
                    nameof(chainNumber));
            }

            return configurationsForChains[chainNumber];
        }

        public List<SimulatedAnnealingConfiguration> GetAllConfigurations()
        {
            // TODO: is this the best possible way?
            return configurationsForChains != null
                ? configurationsForChains.ToList()
                : new List<SimulatedAnnealingConfiguration>() {fixedConfiguration};
        }

        public SimulatedAnnealingConfigurationProvider SmartClone()
        {
            if (useFixedConfiguration)
            {
                return new SimulatedAnnealingConfigurationProvider(fixedConfiguration);
            }

            return new SimulatedAnnealingConfigurationProvider(configurationsForChains.Select(x => x.SmartClone())
                .ToList());
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
            return Equals((SimulatedAnnealingConfigurationProvider) obj);
        }

        public override int GetHashCode()
        {
            return (configurationsForChains != null ? configurationsForChains.GetHashCode() : 0);
        }

        public static bool operator ==(SimulatedAnnealingConfigurationProvider left,
            SimulatedAnnealingConfigurationProvider right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SimulatedAnnealingConfigurationProvider left,
            SimulatedAnnealingConfigurationProvider right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}