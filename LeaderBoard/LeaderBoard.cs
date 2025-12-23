using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using System;
using Firebase.Auth;

using System.Collections.Generic;

public class LeaderBoard : MonoBehaviour
{
    public static LeaderBoard Instance;

    [SerializeField] private GameObject boardRow;
    [SerializeField] private Transform initialpos;
    private DatabaseReference reference;

    [Header("Current Player Box")]
    public TextMeshProUGUI currentUsername;
    public TextMeshProUGUI currentUserscore;
    public TextMeshProUGUI currentUserrank;

    private int count = 0;
    private void Awake()
    {
       
    }
    void Start()
    {
         reference = FirebaseDatabase.DefaultInstance.RootReference;
         GenerateData();
         FetchCurrentUserData();
    }

     public void GenerateData()
     {
         count = 0;
        foreach (Transform child in initialpos)
        {
            Destroy(child.gameObject);
        }
        reference.Child("users").OrderByChild("score").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ Failed to fetch data: " + task.Exception);
                return;
            }
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    Debug.Log("⚠️ No data found in database.");
                    return;
                }

                // int count = 10;
                List<User> users = new List<User>();

                foreach (var child in snapshot.Children)
                {
                    string id = child.Key;
                    string json = child.GetRawJsonValue();
                    User user = JsonUtility.FromJson<User>(json);
                    if (user != null) users.Add(user);
                }
                users.Reverse();
                // foreach (Transform child in initialpos)
                //     Destroy(child.gameObject);

                // Instantiate and bind data
                foreach (var user in users)
                {

                    var rowGO = Instantiate(boardRow, initialpos);
                    var box = rowGO.GetComponent<PlayerBox>();
                    int rank= count + 1;
                    box.BindUserData(user.username, user.score, rank);
                    count++;
                }
                Debug.Log($"✅ Returned {count} entries (might be less than 10).");
                //currentBox.Instance.FetchCurrentUserData();
                
            }

        });
     }
     public void FetchCurrentUserData()
     {
        string fallbackName = "Unknown";
        int fallbackScore = 0;
        int fallbackRank = 0;

        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("⚠ No authenticated user.");
            SpawnBox(fallbackName, fallbackScore, fallbackRank);
            return;
        }

        string uid = user.UserId;

        FirebaseDatabase.DefaultInstance.GetReference("users")
            .Child(uid).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || !task.IsCompleted || !task.Result.Exists)
            {
                Debug.LogError("❌ Error reading user node.");
                SpawnBox(fallbackName, fallbackScore, fallbackRank);
                return;
            }

            User current = JsonUtility.FromJson<User>(task.Result.GetRawJsonValue());

            FirebaseDatabase.DefaultInstance.GetReference("users")
            .OrderByChild("score")
            .GetValueAsync().ContinueWithOnMainThread(rankTask =>
            {
                if (rankTask.IsFaulted || !rankTask.IsCompleted)
                {
                    Debug.LogError("❌ Error getting all users.");
                    SpawnBox(current.username, current.score, fallbackRank);
                    return;
                }

                int higher = 0;
                foreach (var snap in rankTask.Result.Children)
                {
                    User u = JsonUtility.FromJson<User>(snap.GetRawJsonValue());
                    if (u != null && u.score > current.score)
                        higher++;
                }

                int rank = higher + 1;
                SpawnBox(current.username, current.score, rank);
            });
        });
     }
    private void SpawnBox(string name, int score, int rank)
    {
        currentUsername.text = name;
        currentUserscore.text = score.ToString();
        currentUserrank.text = rank.ToString();
        PlayerPrefs.SetInt("Rank", rank);
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetString("Name", name);

        Debug.Log($"Spawned player box for {name} : Score={score}, Rank={rank}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
}


// rules
// {
//   "rules": {

//     // Global default: writing requires auth. Reading allowed for leaderboard only.
//     ".read": false,
//     ".write": "auth != null",

//     "usernames": {
//       "$uname": {
//         ".read": "auth != null",
//         ".write": "auth != null && (
//           (!data.exists() && newData.val() == auth.uid) ||
//           (data.exists() && data.val() == auth.uid && newData.val() == auth.uid)
//         )"
//       }
//     },

//     "users": {

//       // Allow public READ for leaderboard
//       ".read": true,

//       "$uid": {
//         // User can only write their own data
//         ".write": "auth != null && auth.uid == $uid"
//       }
//     }
//   }
// }
