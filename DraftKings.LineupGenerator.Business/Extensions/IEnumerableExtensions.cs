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

        public static IEnumerable<IEnumerable<T>> GetPermutationsWithRepetition<T>(this IEnumerable<T> items, int count)
        {
            if (count == 1)
            {
                return items.Select(x => new T[] { x });
            }

            return items.GetPermutationsWithRepetition(count - 1).SelectMany(t => items, (a, b) => a.Concat(new T[] { b }));
        }

        /// <summary>
        /// Aggregates two permutations into a single k-combination enumerable.
        /// </summary>
        public static IEnumerable<IEnumerable<T>> CombinePermutations<T>(this IEnumerable<IEnumerable<T>> first, IEnumerable<IEnumerable<T>> second)
        {
            var inner = first;
            var outer = second;

            var innerIterated = false;
            var outerIterated = false;

            foreach (var outerPermutation in outer)
            {
                outerIterated = true;

                foreach (var innerPermutation in inner)
                {
                    innerIterated = true;

                    yield return innerPermutation.Concat(outerPermutation);
                }

                if (!innerIterated)
                {
                    yield return outerPermutation;
                }
            }

            if (!outerIterated)
            {
                foreach (var permutation in inner)
                {
                    yield return permutation;
                }
            }
        }
    }
}
