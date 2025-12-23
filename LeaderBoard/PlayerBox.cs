using UnityEngine;
using TMPro;

public class PlayerBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI rank;
    public void BindUserData(string username,int userScore, int userRank)
    {
        name.text = username;
        score.text = userScore.ToString();
        rank.text = userRank.ToString();
    }
}
