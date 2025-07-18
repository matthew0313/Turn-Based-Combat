using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumeratorUtility
{
    public static T Select<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        foreach (var i in list) if (predicate(i)) return i;
        return default;
    }
    public static IEnumerable<T> SelectAll<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        foreach(var i in list) if (predicate(i)) yield return i;
    }
    public static T SelectRandom<T>(this IEnumerable<T> list)
    {
        if (list == null) return default;
        List<T> tmp = list.ToList();
        if (tmp.Count <= 0) return default;
        else return tmp[UnityEngine.Random.Range(0, tmp.Count)];
    }
    public static T SelectRandom<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        if (list == null) return default;
        List<T> tmp = list.SelectAll(predicate).ToList();
        if (tmp.Count <= 0) return default;
        else return tmp[UnityEngine.Random.Range(0, tmp.Count)];
    }
    public static List<T> SelectRandomMultiple<T>(this IEnumerable<T> list, Func<T, bool> predicate, int count)
    {
        if (list == null) return null;
        List<T> tmp = list.SelectAll(predicate).ToList();
        if (tmp.Count < count) return null;
        List<T> results = new();
        for(int i = 0; i < count; i++)
        {
            int rand = UnityEngine.Random.Range(0, tmp.Count);
            results.Add(tmp[rand]);
            tmp.RemoveAt(rand);
        }
        return results;
    }
}
