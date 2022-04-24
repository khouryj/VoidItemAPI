using System;
using System.Collections.Generic;
using System.Text;

namespace VoidItemAPI
{
    public static class Util
    {

        /// <summary>
        /// Splits an enumerable into two based on the bool returned by the given func.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="first">The enumerable to be split</param>
        /// <param name="func">The function to split based upon</param>
        /// <param name="Result2">An enumerable of type T given func returns false.</param>
        /// <returns>An enumerable of type T given func returns true</returns>
        public static IEnumerable<T> Split<T>(this IEnumerable<T> first, Func<T,bool> func, out IEnumerable<T> Result2)
        {
            List<T> result = new List<T>();
            List<T> result2 = new List<T>();

            foreach(T item in first)
            {
                if (func.Invoke(item))
                {
                    result.Add(item);
                }
                else
                {
                    result2.Add(item);
                }
            }
            Result2 = result2;
            return result;
        }
    }
}
