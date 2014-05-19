using System.Collections.Generic;

namespace Plato.Helpers
{
    /// <summary>
    ///  Helper class that provides binary search algorithm
    ///  This code should be synchronized with QSharp.Scheme.Classical.Sequential
    /// </summary>
    public static class BinarySearch
    {
        #region Delegates

        public delegate int ComparePredicate<in T>(T testee);

        #endregion

        #region Methods

        /// <summary>
        ///  Binary searches the specified list for the specified item within the specifeid range and using the specified comparer
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">The list to search</param>
        /// <param name="start">The inclusive start point</param>
        /// <param name="end">The exclusive end point</param>
        /// <param name="compareTo">
        ///  The criteria that determines if an item is greater than, less than or equals the target that it embodies
        ///  The comparison is in line with the ordering of the list
        /// </param>
        /// <param name="index">The index of the item found or where it should be inserted</param>
        /// <returns>True if the item was found</returns>
        public static bool Search<T>(this IList<T> list, int start, int end, ComparePredicate<T> compareTo, out int index)
        {
            int b = start, e = end;

            for (; ; )
            {
                if (b == e)
                {
                    index = b;
                    return false;
                }

                index = (b + e) / 2;
                var t = list[index];

                var cmp = compareTo(t);
                if (cmp < 0)
                {
                    // targetKey < list[index]
                    e = index;
                }
                else if (cmp > 0)
                {
                    // list[index] < targetKey
                    b = index + 1;
                }
                else
                {
                    // match
                    return true;
                }
            }
        }

        /// <summary>
        ///  Searches for the specified item in the list within the specified range
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">The list to search through</param>
        /// <param name="start">The inclusive starting point</param>
        /// <param name="end">The exclusive end point</param>
        /// <param name="compareTo">A comparer that compares an item with the search target it involves</param>
        /// <returns>The index to the item that matches the input</returns>
        public static int Search<T>(this IList<T> list, int start, int end, ComparePredicate<T> compareTo)
        {
            int at;
            var found = Search(list, start, end, compareTo, out at);
            return found ? at : -(at + 1);
        }

        /// <summary>
        ///  Searches for the specified item in the whole list
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">The list to search through</param>
        /// <param name="compareTo">A comparer that compares an item with the search target it involves</param>
        /// <returns>The index to the item that matches the input</returns>
        public static int Search<T>(this IList<T> list, ComparePredicate<T> compareTo)
        {
            return Search(list, 0, list.Count, compareTo);
        }

        #endregion
    }
}
