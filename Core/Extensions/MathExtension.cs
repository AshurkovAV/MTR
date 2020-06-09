using System;
using System.Collections.Generic;

namespace Core.Extensions
{
    public static class MathExtension
    {
        /// <summary>
        /// Возвращает значение в диапазоне от min до max, если value находятся за пределами этого диапазона применяет ближайшее значение min или max.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static T Clamp<T>(this T value, T min, T max)where T : IComparable 
        {
            if (value.CompareTo(min) < 0)
                return min;

            if (value.CompareTo(max) > 0)
                return max;

            return value;
        }

        /// <summary>
        /// Возвращает значение в диапазоне от min до max, если value находятся за пределами этого диапазона применяет ближайшее значение min или max.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static decimal Clamp(this decimal value, decimal min, decimal max) {
            if (value < min)
				return min;

            if (value > max)
				return max;

            return value;
		}

        /// <summary>
        /// Возвращает значение в диапазоне от min до max, если value находятся за пределами этого диапазона применяет ближайшее значение min или max.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static decimal? Clamp(this decimal? value, decimal? min, decimal? max)
        {
            if (!value.HasValue)
                return null;

            if (min.HasValue && value < min)
                return min;

            if (max.HasValue && value > max)
                return max;

            return value;
        }
    }
}
