using System;
using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.ConfigurationSpaces
{
    public interface IConfigurationSpace<TPosition>
    {
        TPosition GetRandomPosition(Random random);

        IEnumerable<TPosition> ShuffleAndSamplePositions(int maxPointsPerLine, Random random);
    }
}