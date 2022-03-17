using System;
using System.Collections.Generic;
using Edgar.Legacy.Utils.Statistics;
using NUnit.Framework;

namespace MapGeneration.Tests.Utils.Statistics
{
    [TestFixture]
    public class EntropyCalculatorTests
    {
        private EntropyCalculator entropyCalculator;

        [SetUp]
        public void SetUp()
        {
            entropyCalculator = new EntropyCalculator();
        }

        [Test]
        public void ComputeEntropy_EmptyDistribution_Throws()
        {
            var distribution = new Dictionary<int, double>();

            Assert.Throws<ArgumentException>(() => entropyCalculator.ComputeEntropy(distribution));
        }

        [Test]
        public void ComputeEntropy_SingleElement_ReturnsZero()
        {
            var distribution = new Dictionary<int, double>()
            {
                {0, 1d}
            };

            var entropy = entropyCalculator.ComputeEntropy(distribution);
            var normalizedEntropy = entropyCalculator.ComputeEntropy(distribution, true);

            Assert.That(entropy, Is.EqualTo(0));
            Assert.That(normalizedEntropy, Is.EqualTo(0));
        }

        [Test]
        public void ComputeEntropy_ZeroProbabilities()
        {
            var distribution = new Dictionary<int, double>()
            {
                {0, 1 / 2d},
                {1, 1 / 2d},
                {2, 0},
                {3, 0}
            };

            var entropy = entropyCalculator.ComputeEntropy(distribution);
            var normalizedEntropy = entropyCalculator.ComputeEntropy(distribution, true);

            Assert.That(entropy, Is.EqualTo(1));
            Assert.That(normalizedEntropy, Is.EqualTo(1 / 2d));
        }

        [Test]
        public void ComputeEntropy_EquiprobableEvents()
        {
            var distribution = new Dictionary<int, double>()
            {
                {0, 1 / 4d},
                {1, 1 / 4d},
                {2, 1 / 4d},
                {3, 1 / 4d}
            };

            var entropy = entropyCalculator.ComputeEntropy(distribution);
            var normalizedEntropy = entropyCalculator.ComputeEntropy(distribution, true);

            Assert.That(entropy, Is.EqualTo(2));
            Assert.That(normalizedEntropy, Is.EqualTo(1));
        }
    }
}