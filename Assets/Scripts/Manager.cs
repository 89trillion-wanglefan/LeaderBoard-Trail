using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主要控制类
/// </summary>
public class Manager : MonoBehaviour
{
    public delegate void ShowEventHandler(Users user,int index);

    public static event ShowEventHandler UpdateINfo;
        
    public GameObject UI;
    public GameObject Button;
    public ShowPlayerInfo PlayerCase;
    public Text Countdown;
    private List<Users> UserList = new List<Users>();
    public ScrollRect scrollRect;
    [Header("固定的Item数量")] public int fixedCount;
    [Header("Item的预制体")] public ShowPlayerInfo itemPrefab;
    public long userID = 3716954261;
    private RectTransform content; //滑动框的Content
    [SerializeField] private GridLayoutGroup layout; //布局组件

    private List<ShowPlayerInfo> dataList = new List<ShowPlayerInfo>(); //实例列表
    private List<ShowPlayerInfo> dataListOrigin = new List<ShowPlayerInfo>(); //保存原始顺序
    private int totalCount; //总的数据数量
    private int headIndex; //头下标
    private int tailIndex; //尾下标
    private Vector2 firstItemAnchoredPos; //第一个元素的坐标

    private JSONNode rankInfo;

    private int countdown;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Init());
    }


    private void OnScroll()
    {
        //Debug.Log(v);
        //Debug.Log(content.anchoredPosition.y);
        //向下滚动
        while (content.anchoredPosition.y >=
               layout.padding.top + (headIndex + 1) * (layout.cellSize.y + layout.spacing.y)
               && tailIndex != totalCount - 1)
        {
            //将数据列表中的第一个元素移动到最后一个
            ShowPlayerInfo item = dataList[0];
            dataList.Remove(item);
            dataList.Add(item);

            //设置位置
            SetRankPos(item, tailIndex + 1);
            //设置显示
            SetRankInfo(item, tailIndex + 1);

            headIndex++;
            tailIndex++;
        }

        //向上滑
        while (content.anchoredPosition.y <= layout.padding.top + headIndex * (layout.cellSize.y + layout.spacing.y)
               && headIndex != 0)
        {
            //将数据列表中的最后一个元素移动到第一个
            ShowPlayerInfo item = dataList.Last();
            dataList.Remove(item);
            dataList.Insert(0, item);

            //设置位置
            SetRankPos(item, headIndex - 1);
            //设置显示
            SetRankInfo(item, headIndex - 1);

            headIndex--;
            tailIndex--;
        }
    }

    private void InitRankList()
    {
        for (int i = 0; i < fixedCount; i++)
        {
            ShowPlayerInfo tempItem = Instantiate(itemPrefab, content);
            dataList.Add(tempItem);
            dataListOrigin.Add(tempItem);
            SetRankInfo(tempItem, i);
        }
    }

    private void ReInitRankList()
    {
        dataList.Clear();
        for (int i = 0; i < fixedCount; i++)
        {
            dataList.Add(dataListOrigin[i]);
            SetRankPos(dataListOrigin[i], i);
            SetRankInfo(dataListOrigin[i], i);
        }

        headIndex = 0;
        tailIndex = fixedCount - 1;
    }

    private void GetFirstItemAnchoredPos()
    {
        firstItemAnchoredPos = new Vector2
        (
            layout.padding.left + layout.cellSize.x / 2,
            -layout.padding.top - layout.cellSize.y / 2
        );
    }

    private void SetRankPos(ShowPlayerInfo trans, int index)
    {
        trans.rectTransform.anchoredPosition = new Vector2
        (
            firstItemAnchoredPos.x,
            index == 0
                ? firstItemAnchoredPos.y
                : firstItemAnchoredPos.y - index * (layout.cellSize.y + layout.spacing.y)
        );
    }

    private void SetRankInfo(ShowPlayerInfo trans, int index)
    {
        UpdateINfo+= trans.ShowRankInfo;
        if (UpdateINfo != null) Manager.UpdateINfo(UserList[index], index);
        UpdateINfo-= trans.ShowRankInfo;
    }

    public string FormatDayTime(int totalSeconds) //时间格式转换
    {
        int minutes = totalSeconds/ 60;
        int seconds = totalSeconds % 60;
        int hours = minutes / 60;
        minutes %= 60;
        int days = hours / 24;
        hours %= 24;
        string dd = days < 10 ? "0" + days : days.ToString();
        string hh = hours < 10 ? "0" + hours : hours.ToString();
        string mm = minutes < 10f ? "0" + minutes : minutes.ToString();
        string ss = seconds < 10 ? "0" + seconds : seconds.ToString();
        return $"Ends in:{dd}d {hh}h {mm}m {ss}s";
    }

    //倒计时组件
    IEnumerator StartCountdown(int total)
    {
        for (; total > 0; total--)
        {
            Countdown.text = FormatDayTime(total);
            total -= 1;
            yield return new WaitForSeconds(1f);
        }
    }

    public void ToUI() //button 按键
    {
        UI.SetActive(true);
        Button.SetActive(false);
    }

    public void Close()
    {
        UI.SetActive(false);
        Button.SetActive(true);
        content.localPosition = new Vector3(0f, 0f, 0f);
        ReInitRankList();
    }

    IEnumerator Init()
    {
        StreamReader
            streamreader = new StreamReader(Application.dataPath + "/StreamingAssets/ranklist.json"); //读取数据，转换成数据流
        string str = streamreader.ReadToEnd();
        rankInfo = JSON.Parse(str);
        countdown = rankInfo["countDown"].AsInt;
        StartCoroutine(StartCountdown(countdown));
        var rankList = rankInfo["list"];
        for (int i = 0; i < rankList.Count; i++)
        {
            var node = rankList[i];
            Users user = new Users(node["uid"].AsLong, node["nickName"],
                node["trophy"].AsInt);
            UserList.Add(user);
        }

        UserList.Sort((x, y) => -x.Trophy.CompareTo(y.Trophy));
        SetRankInfo(PlayerCase, 0);
        totalCount = UserList.Count;
        content = scrollRect.content;
        scrollRect.onValueChanged.AddListener(v => OnScroll());
        //设置头下标和尾下标
        headIndex = 0;
        tailIndex = fixedCount - 1;

        //设置content大小
        content.sizeDelta = new Vector2(0, layout.padding.top + totalCount * (layout.cellSize.y + layout.spacing.y));

        //实例化Item
        InitRankList();

        //得到第一个Item的锚点位置
        GetFirstItemAnchoredPos();
        yield break;
    }
}