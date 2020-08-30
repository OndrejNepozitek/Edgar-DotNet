using System;
using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Common.ConfigurationSpaces
{
    public interface IConfigurationSpace<TPosition>
    {
        TPosition GetRandomPosition(Random random);

        IEnumerable<TPosition> ShuffleAndSamplePositions(int maxPointsPerLine, Random random);
    }
}