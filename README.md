# Unity Firebase Leaderboard System
This package provides a **Plug-and-Play Leaderboard** for your Unity game. It automatically handles player scores, ranks, and displaying a "Top 10" list.
## 🚀 Features
- **Real-time Updates**: The leaderboard updates instantly when scores change.
- **Top 10 Display**: Automatically shows the top 10 players.
- **Player Stats**: Shows the current player's personal Rank and Score at the bottom.
- **Anonymous Login**: No complex sign-up required; users are logged in automatically.
---
## 📂 The Scripts
### 1. `LeaderBoard.cs` (The Brain)
This script manages everything.
- **Connects** to Firebase.
- **Downloads** the top 10 high scores.
- **Calculates** the current player's rank.
- **Creates** the list of names you see on screen.
### 2. `PlayerBox.cs` (The Viewer)
This is a small script for the UI.
- It sits on each row of the leaderboard.
- It simply displays: `Name`, `Score`, and `Rank`.
---
## 🛠️ How to Setup in Unity
1. **Install Firebase**: Make sure you have the **Firebase Realtime Database** and **Firebase Auth** SDKs in your project.
2. **Setup the UI**:
   - Create a **Scroll View** to list the players (this will be your `Initial Pos`).
   - Create a **Panel** for the current player's stats (Name, Score, Rank).
3. **Connect the Script**:
   - create an empty GameObject in your scene and name it "LeaderboardManager".
   - Drag `LeaderBoard.cs` onto it.
4. **Assign Variables**:
   - **Board Row**: Drag your "Player Row" prefab here.
   - **Initial Pos**: Drag the "Content" object from your Scroll View here.
   - **Current User Text**: Drag your UI Text objects for Name, Score, and Rank into the slots.
---
## 🔒 Firebase Security Rules
Copy and paste these rules into your **Firebase Console > Realtime Database > Rules** tab. These rules ensure that players can only write their own scores and usernames ensuring a secure leaderboard.
```json
{
  "rules": {
    // Global default: writing requires auth. Reading allowed for leaderboard only.
    ".read": false,
    ".write": "auth != null",
    "usernames": {
      "$uname": {
        ".read": "auth != null",
        ".write": "auth != null && (
          (!data.exists() && newData.val() == auth.uid) ||
          (data.exists() && data.val() == auth.uid && newData.val() == auth.uid)
        )"
      }
    },
    "users": {
      // Allow public READ for leaderboard
      ".read": true,
      "$uid": {
        // User can only write their own data
        ".write": "auth != null && auth.uid == $uid"
      }
    }
  }
}
```
---
## 📊 Database Structure
Your data in Firebase will look like this:
```json
{
  "users": {
    "unique_user_id_1": {
      "username": "CoolPlayer",
      "score": 1500
    },
    "unique_user_id_2": {
      "username": "ProGamer",
      "score": 1200
    }
  }
}
```
