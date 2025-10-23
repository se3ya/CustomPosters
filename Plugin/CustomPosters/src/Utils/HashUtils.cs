using System.Runtime.CompilerServices;

namespace CustomPosters.Utils
{
    internal static class HashUtils
    {
        private const int HashSeed = 23;
        private const int HashMul = 31;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DeterministicHash(string input)
        {
            unchecked
            {
                int hash = HashSeed;
                for (int i = 0; i < input.Length; i++)
                {
                    hash = (hash * HashMul) + input[i];
                }
                return hash;
            }
        }
    }
}
