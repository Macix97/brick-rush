using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FirebaseUser : IComparable
{
    [SerializeField] private string name;
    [SerializeField] private int score;

    public readonly string Name => name;
    public readonly int Score => score;

    public const string NameKey = nameof(name);
    public const string ScoreKey = nameof(score);

    public FirebaseUser(string name)
    {
        this.name = name;
        score = 0;
    }

    public FirebaseUser(object keyValuePairs)
    {
        Dictionary<string, object> dictionary = keyValuePairs as Dictionary<string, object>;
        name = dictionary[NameKey].ToString();
        score = int.Parse(dictionary[ScoreKey].ToString());
    }

    public readonly Dictionary<string, object> GetDictionary(Dictionary<string, object> dictionary)
    {
        dictionary.Clear();
        dictionary.Add(NameKey, Name);
        dictionary.Add(ScoreKey, Score);
        return dictionary;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public int CompareTo(object instance)
    {
        FirebaseUser user = (FirebaseUser)instance;
        return score.CompareTo(user.score);
    }
}
