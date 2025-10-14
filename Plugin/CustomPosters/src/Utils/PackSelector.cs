using System.Collections.Generic;
using System.Linq;

namespace CustomPosters.Utils
{
    internal static class PackSelector
    {
        public static string SelectPackByChance(List<string> enabledPacks)
        {
            if (enabledPacks == null || enabledPacks.Count == 0)
            {
                return null!;
            }

            if (enabledPacks.Count == 1)
            {
                return enabledPacks[0];
            }

            var packChances = enabledPacks.Select(p => Plugin.ModConfig.GetPackChance(p)).ToList();
            
            if (packChances.All(c => c == 0))
            {
                return enabledPacks[Plugin.Service.Rand.Next(enabledPacks.Count)];
            }

            return SelectByWeightedChance(enabledPacks, packChances);
        }

        public static T SelectContentByChance<T>(List<T> items, System.Func<T, int> getChance, System.Func<T, int> getPriority)
        {
            if (items == null || items.Count == 0)
            {
                return default!;
            }

            if (items.Count == 1)
            {
                return items[0];
            }

            var chances = items.Select(getChance).ToList();
            
            if (chances.All(c => c == 0))
            {
                return items.OrderBy(getPriority).First();
            }

            return SelectByWeightedChance(items, chances);
        }

        private static T SelectByWeightedChance<T>(List<T> items, List<int> chances)
        {
            var totalChance = chances.Sum();
            if (totalChance <= 0)
            {
                return items[0];
            }

            var randomValue = Plugin.Service.Rand.NextDouble() * totalChance;
            double cumulative = 0;

            for (int i = 0; i < items.Count; i++)
            {
                cumulative += chances[i];
                if (randomValue <= cumulative)
                {
                    return items[i];
                }
            }

            return items[0];
        }
    }
}