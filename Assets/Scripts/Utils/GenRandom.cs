using UnityEngine;
using Random = System.Random;

namespace Utils
{
    public static class GenRandom
    {
        private static Random _random;

        static GenRandom()
        {
            Initialize(null);
        }

        public static void Initialize(int? seed = null)
        {
            if (seed == null)
            {
                var random = new Random();
                seed = random.Next(100);
                // seed = 4;
            }

            Debug.Log($"Random SEED = {seed}");
            _random = new Random(seed.Value);
        }

        public static int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public static int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public static bool NextBool()
        {
            return _random.Next(2) == 0;
        }
    }
}