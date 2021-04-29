using System;
using System.Collections.Generic;

namespace CodeBase
{
    public static class DictionaryExtension
    {
        public static void Push<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static void Push<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value, Func<TValue, TValue, TValue> contains)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = contains(dict[key], value);
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static TValue Pop<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            dict.TryGetValue(key, out TValue value);
            return value;
        }

        public static TValue Pop<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue alt)
        {
            if (dict.TryGetValue(key, out TValue value))
            {
                return value;
            }
            return alt;
        }
    }
}
