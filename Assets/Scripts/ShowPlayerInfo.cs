using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

/// <summary>
/// 设置显示元素的显示内容用的类
/// </summary>
public class ShowPlayerInfo : MonoBehaviour
{
    [SerializeField]
    private Text playername;
    [SerializeField]
    private Text rank;
    [SerializeField]
    private Text score;
    [SerializeField]
    private Image badge;
    [SerializeField]
    private Image medal;

    public RectTransform rectTransform;

    /// <summary>
    /// 点击时调用toast的方法
    /// </summary>
    public void Click()
    {
        Toast.Instance.ShowToast($"User: {playername.text}, Rank: {rank.text}");
    }
    
    /// <summary>
    /// 设置显示元素的内容
    /// </summary>
    /// 显示的用户信息
    /// <param name="user"></param>
    /// 显示的用户排名
    /// <param name="index"></param>
    public void ShowRankInfo(Users user,int index)
    {
        //prefab调整方法
        score.text = user.Trophy.ToString();
        playername.text = user.Nickname;
        badge.sprite =
            Resources.Load($"Images/rank/arenaBadge_{user.Trophy / 1000 + 1}", typeof(Sprite)) as Sprite;
        if (index > 2)
        {
            medal.color = new Color(255, 255, 255, 0f);
            rank.text = (index + 1).ToString();
        }
        else
        {
            medal.color = new Color(255, 255, 255, 1f);
            medal.sprite =
                Resources.Load($"Images/rank_{index + 1}", typeof(Sprite)) as Sprite;
            medal.SetNativeSize();
            rank.text = "";
        }

        transform.name = index.ToString();
    }
}
