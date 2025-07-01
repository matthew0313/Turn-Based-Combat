using System;
using System.Collections.Generic;
using UnityEngine;

public static class RandomUtility
{
    public static T SelectRandom<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        if (list == null) return default;
        List<T> tmp = new();
        foreach (var i in list) if (predicate.Invoke(i)) tmp.Add(i);
        if (tmp.Count <= 0) return default;
        else return tmp[UnityEngine.Random.Range(0, tmp.Count)];
    }
    public static List<T> SelectRandom<T>(this IEnumerable<T> list, Func<T, bool> predicate, int count)
    {
        if (list == null) return null;
        List<T> tmp = new();
        foreach (var i in list) if (predicate.Invoke(i)) tmp.Add(i);
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
