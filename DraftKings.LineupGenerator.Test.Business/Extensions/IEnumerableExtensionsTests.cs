using DraftKings.LineupGenerator.Business.Extensions;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace DraftKings.LineupGenerator.Test.Business.Extensions
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void GetsPermutations()
        {
            var values = new[] { 1, 2, 3 };

            IEnumerable<IEnumerable<int>> expected = new int[][]
            {
                new int[] { 1, 2 },
                new int[] { 1, 3 },
                new int[] { 2, 3 }
            };

            var actual = values.GetPermutations(2);

            actual.Should().BeEquivalentTo(expected, opts => opts.WithoutStrictOrdering());
        }

        [Fact]
        public void CombinesPermutations()
        {
            IEnumerable<IEnumerable<int>> first = new int[][]
            {
                new int[] { 1, 2 },
                new int[] { 1, 3 },
                new int[] { 2, 3 }
            };

            IEnumerable<IEnumerable<int>> second = new int[][]
            {
                new int[] { 4, 5 },
                new int[] { 4, 6 },
                new int[] { 5, 6 }
            };

            IEnumerable<IEnumerable<int>> expected = new int[][]
            {
                new int[] { 1, 2, 4, 5 },
                new int[] { 1, 2, 4, 6 },
                new int[] { 1, 2, 5, 6 },

                new int[] { 1, 3, 4, 5 },
                new int[] { 1, 3, 4, 6 },
                new int[] { 1, 3, 5, 6 },

                new int[] { 2, 3, 4, 5 },
                new int[] { 2, 3, 4, 6 },
                new int[] { 2, 3, 5, 6 }
            };

            var actual = first.CombinePermutations(second);

            actual.Should().BeEquivalentTo(expected, opts => opts.WithoutStrictOrdering());
        }
    }
}
