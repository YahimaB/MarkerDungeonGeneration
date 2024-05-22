using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        public static bool Intersects(this BoundsInt a, BoundsInt b, bool intersectTouching = false)
        {
            if (intersectTouching)
            {
                return (a.xMin <= b.xMax) && (a.xMax >= b.xMin) &&
                       (a.yMin <= b.yMax) && (a.yMax >= b.yMin) &&
                       (a.zMin <= b.zMax) && (a.zMax >= b.zMin);
            }

            return (a.xMin < b.xMax) && (a.xMax > b.xMin) &&
                   (a.yMin < b.yMax) && (a.yMax > b.yMin) &&
                   (a.zMin < b.zMax) && (a.zMax > b.zMin);
        }

        public static int GetBoundsVolume(this BoundsInt bounds)
        {
            var size = bounds.size;
            return size.x * size.y * size.z;
        }

        //Root mean square percent error
        public static float RMSPE(this List<float> list, float target, float range)
        {
            return list.RMSE(target) / range;
        }

        //Root mean square error
        public static float RMSE(this List<float> list, float target)
        {
            var squareErrorSum = 0f;

            foreach (var value in list)
            {
                var diff = value - target;
                squareErrorSum += diff * diff;
            }

            return Mathf.Sqrt(squareErrorSum / list.Count);
        }

        public static float WAPE(this List<float> list, float target)
        {
            var sum = 0f;
            var absErrorSum = 0f;

            foreach (var value in list)
            {
                sum += value;
                var absError = Mathf.Abs(value - target);
                absErrorSum += absError;
            }

            return absErrorSum / sum;
        }
        
        public static float WAPE(this List<float> list, List<float> targets)
        {
            var sum = 0f;
            var absErrorSum = 0f;

            for (var i = 0; i < list.Count; i++)
            {
                var value = list[i];
                sum += value;
                var absError = Mathf.Abs(value - targets[i]);
                absErrorSum += absError;
            }

            return absErrorSum / sum;
        }
        
        //Mean absolute percent error
        public static float MAPE(this List<float> list, List<float> targets)
        {
            // var sum = 0f;
            var absErrorSum = 0f;

            for (var i = 0; i < list.Count; i++)
            {
                var value = list[i];
                // sum += value;
                var absError = Mathf.Abs(value - targets[i]);
                absErrorSum += absError / targets[i];
            }

            return absErrorSum / list.Count;
        }

        public static float MASE(this List<float> list, float target)
        {
            var absErrorSum = 0f;
            var absDiffSum = 0f;
            var count = list.Count;

            for (var i = 1; i < count; i++)
            {
                var absError = Mathf.Abs(list[i] - target);
                absErrorSum += absError;

                var absDiff = Mathf.Abs(list[i] - list[i - 1]);
                absDiffSum += absDiff;
            }

            var t = (float)count / (count - 1);

            return absErrorSum / (t * absDiffSum);
        }
    }
}