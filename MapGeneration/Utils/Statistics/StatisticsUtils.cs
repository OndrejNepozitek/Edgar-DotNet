using System;

namespace MapGeneration.Utils.Statistics
{
    public static class StatisticsUtils
    {
        /// <summary>
        /// Returns the difference in percents
        /// </summary>
        /// <param name="value"></param>
        /// <param name="referenceValue"></param>
        /// <returns></returns>
        public static double DifferenceToReference(double value, double referenceValue)
        {
            return value * 100 / referenceValue - 100;
        }

        /// <summary>
        /// Returns the difference in percents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueObject"></param>
        /// <param name="referenceValueObject"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static double DifferenceToReference<T>(T valueObject, T referenceValueObject, Func<T, double> valueSelector)
        {
            return DifferenceToReference(valueSelector(valueObject), valueSelector(referenceValueObject));
        }
    }
}