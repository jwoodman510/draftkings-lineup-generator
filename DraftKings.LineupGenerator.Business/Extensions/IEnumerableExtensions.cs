using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> items, int count)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                {
                    yield return new T[] { item };
                }
                else
                {
                    foreach (var result in GetPermutations(items.Skip(i + 1), count - 1))
                    {
                        yield return new T[] { item }.Concat(result);
                    }
                }

                ++i;
            }
        }

        public static IEnumerable<IEnumerable<T>> CombinePermutations<T>(this IEnumerable<IEnumerable<T>> first, IEnumerable<IEnumerable<T>> second)
        {
            foreach (var permutation in second)
            {
                foreach (var innerPermutation in first)
                {
                    yield return innerPermutation.Concat(permutation);
                }
            }
        }
    }
}
