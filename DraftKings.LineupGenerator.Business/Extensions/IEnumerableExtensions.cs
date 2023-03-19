using System;
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

        /// <summary>
        /// Aggregates two permutations into a single k-combination enumerable.
        /// </summary>
        /// <param name="filterPredicate">An optional filter applied to determine whether permutation items should be combined.</param>
        public static IEnumerable<IEnumerable<T>> CombinePermutations<T>(
            this IEnumerable<IEnumerable<T>> first,
            IEnumerable<IEnumerable<T>> second,
            Func<IEnumerable<T>, IEnumerable<T>, bool> filterPredicate = null)
        {
            var inner = first;
            var outter = second;

            var innerIterated = false;
            var outerIterated = false;

            foreach (var outerPermutation in outter)
            {
                outerIterated = true;

                foreach (var innerPermutation in inner)
                {
                    innerIterated = true;

                    if (filterPredicate == null || filterPredicate(innerPermutation, outerPermutation))
                    {
                        yield return innerPermutation.Concat(outerPermutation);
                    }
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
