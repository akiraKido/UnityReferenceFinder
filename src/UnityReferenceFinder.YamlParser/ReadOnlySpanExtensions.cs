using System;

namespace UnityReferenceFinder.YamlParser {
    internal static class ReadOnlySpanExtensions
    {
        public static bool IsMatch<T>(this ReadOnlySpan<T> input, ReadOnlySpan<T> test)
        {
            if (input.Length != test.Length) return false;
            if (input.Length == 0) return true;
            if (input[0].Equals(test[0]) == false) return false;
            
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].Equals(test[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static int Count<T>(this ReadOnlySpan<T> input, T item)
        {
            var count = 0;
            foreach (var i in input)
            {
                if (i.Equals(item))
                {
                    count += 1;
                }
            }

            return count;
        }

        public static int BackTrackCount<T>(this ReadOnlySpan<T> input, int start, T item)
        {
            var cnt = 0;
            for (int i = start; i > 0; i--)
            {
                if (input[i].Equals(item))
                {
                    break;
                }

                cnt += 1;
            }

            return cnt;
        }
    }
}