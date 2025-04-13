namespace GameStateService.Utils
{
    public static class RandomProvider
    {
        private static readonly Random _global = new();
        [ThreadStatic] private static Random? _local;

        public static Random GetThreadRandom()
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }

            return _local;
        }

        public static bool FiftyFiftyChance() => GetThreadRandom().Next(0, 2) == 1;
        public static int Next() => GetThreadRandom().Next();
        public static int Next(int max) => GetThreadRandom().Next(max);
        public static int Next(int min, int max) => GetThreadRandom().Next(min, max);

        public static double NextDouble() => GetThreadRandom().NextDouble();
    }
}
