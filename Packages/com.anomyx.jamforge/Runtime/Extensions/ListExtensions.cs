using System;
using System.Collections;
using System.Collections.Generic;

namespace JamForge
{
    public static class ListExtensions
    {
        public static T AddTo<T>(this T data, List<T> list)
        {
            list.Add(data);
            return data;
        }

        public static List<T> AddMultiple<T>(this List<T> list, params T[] data)
        {
            list.AddRange(data);
            return list;
        }

        public static void Shuffle(this IList list, int? seed = null)
        {
            var random = seed.HasValue ? new Random(seed.Value) : new Random();
            for (var i = 0; i < list.Count; i++)
            {
                var j = random.Next(i, list.Count);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
        
        public static void Shuffle<T>(this IList<T> list, int? seed = null)
        {
            var random = seed.HasValue ? new Random(seed.Value) : new Random();
            for (var i = 0; i < list.Count; i++)
            {
                var j = random.Next(i, list.Count);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
    }
}
