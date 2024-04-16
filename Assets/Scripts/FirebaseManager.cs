using UnityEngine;
using Firebase.Database;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

public class FirebaseManager : MonoBehaviourPersistentSingleton<FirebaseManager>
{
    [SerializeField] private string databaseURL;
    [SerializeField] private string usersPath = "users";

    private string userID;
    private FirebaseUser userData;
    private DatabaseReference databaseReference;

    public static string UserID => Instance.userID;
    public static FirebaseUser UserData => Instance.userData;
    private DatabaseReference UsersReference => databaseReference.Child(usersPath);

    public static event Action OnDatabaseChanged;

    protected override void Awake()
    {
        base.Awake();
        userID = SystemInfo.deviceUniqueIdentifier;
        databaseReference = FirebaseDatabase.GetInstance(databaseURL).RootReference;
        databaseReference.ValueChanged += OnDatabaseValueChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        databaseReference.ValueChanged -= OnDatabaseValueChanged;
    }

    public static void CheckIsUserAsync(Action<bool> callback = null)
    {
        Instance.StartCoroutine(Instance.OnCheckingUser(callback));
    }

    public static void CreateAccountAsync(string userName, Action<bool> callback = null)
    {
        Instance.StartCoroutine(Instance.OnCreatingAccount(userName, callback));
    }

    public static void GetAllUsersAsync(List<FirebaseUser> users, Action callback = null)
    {
        Instance.StartCoroutine(Instance.OnGettingAllUsers(users, callback));
    }

    public static void GetUserScoreAsync(Action<int> callback = null)
    {
        Instance.StartCoroutine(Instance.OnGettingUserScore(callback));
    }

    public static void SetUserScoreAsync(int score, Action<bool> callback = null)
    {
        Instance.StartCoroutine(Instance.OnSettingUserScore(score, callback));
    }

    private IEnumerator OnCreatingAccount(string userName, Action<bool> callback)
    {
        Task<DataSnapshot> dataTask = UsersReference.GetValueAsync();
        yield return CoroutineUtils.WaitForCompletion(dataTask);
        if (!IsUserNameValid(userName, dataTask.Result))
        {
            callback?.Invoke(false);
        }
        else
        {
            Task voidTask = UsersReference.SetValueAsync(userData.ToDictionary());
            yield return CoroutineUtils.WaitForCompletion(voidTask);
            if (voidTask.IsCompletedSuccessfully)
                userData.SetName(userName);
            callback?.Invoke(voidTask.IsCompletedSuccessfully);
        }
    }

    private IEnumerator OnCheckingUser(Action<bool> callback)
    {
        Task<DataSnapshot> task = UsersReference.Child(UserID).GetValueAsync();
        yield return CoroutineUtils.WaitForCompletion(task);
        if (task.Result.Exists)
            userData = new FirebaseUser(task.Result.Value);
        callback?.Invoke(task.Result.Exists);
    }

    private IEnumerator OnGettingAllUsers(List<FirebaseUser> users, Action callback)
    {
        Task<DataSnapshot> task = UsersReference.GetValueAsync();
        yield return CoroutineUtils.WaitForCompletion(task);
        users.Clear();
        foreach (DataSnapshot user in task.Result.Children)
            users.Add(new FirebaseUser(user.Value));
        callback?.Invoke();
    }

    private IEnumerator OnGettingUserScore(Action<int> callback)
    {
        Task<DataSnapshot> task = UsersReference.Child(UserID).GetValueAsync();
        yield return CoroutineUtils.WaitForCompletion(task);
        int score = int.Parse(task.Result.Child(FirebaseUser.ScoreKey).Value.ToString());
        callback?.Invoke(score);
    }

    private IEnumerator OnSettingUserScore(int score, Action<bool> callback)
    {
        Task task = UsersReference.Child(UserID).Child(FirebaseUser.ScoreKey).SetValueAsync(score.ToString());
        yield return CoroutineUtils.WaitForCompletion(task);
        callback?.Invoke(task.IsCompletedSuccessfully);
    }

    private bool IsUserNameValid(string userName, DataSnapshot snapshot)
    {
        foreach (DataSnapshot child in snapshot.Children)
        {
            if (child.Child(FirebaseUser.NameKey).Value.ToString() == userName) return false;
        }
        return true;
    }

    private void OnDatabaseValueChanged(object sender, ValueChangedEventArgs args)
    {
        OnDatabaseChanged?.Invoke();
    }
}
