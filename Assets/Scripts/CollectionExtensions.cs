using System.Collections.Generic;

public static class CollectionExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            T item = list[i];
            int index = UnityEngine.Random.Range(i, count);
            list[i] = list[index];
            list[index] = item;
        }
    }

    public static T First<T>(this IList<T> list) => list[0];

    public static void Push<T>(this List<T> list, T element) => list.Insert(0, element);

    public static T Random<T>(this IList<T> list) => list[UnityEngine.Random.Range(0, list.Count)];

    public static T Random<T>(this List<T> list, bool remove = false)
    {
        int index = UnityEngine.Random.Range(0, list.Count);
        T item = list[index];
        if (remove) list.RemoveAt(index);
        return item;
    }

    public static void SetAsFirst<T>(this IList<T> list, T element) => list.SetAsFirst(list.IndexOf(element));

    public static void SetAsFirst<T>(this IList<T> list, int index)
    {
        T item = list[index];
        for (int i = index; i > 0; i--)
            list[i] = list[i - 1];
        list[0] = item;
    }
}
