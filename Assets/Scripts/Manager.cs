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
    public GameObject UI; //UI的Canvas
    public GameObject Button; //按钮的Canvas
    public ShowPlayerInfo PlayerCase; //显示用户信息的组件
    public Text Countdown; //倒计时文本
    private List<Users> UserList = new List<Users>(); //用户信息列表
    public ScrollRect scrollRect; //滚动栏组件
    public RectTransform rectTransform; //滚动栏RectTransform，获取高度用
    [Header("元素的预制体")] public ShowPlayerInfo itemPrefab; //滚动栏中单个显示元素的预置体
    public long userID = 3716954261; //用户的id，查询用户用
    private RectTransform content; //滑动框的Content
    [SerializeField] private GridLayoutGroup layout; //布局组件

    private List<ShowPlayerInfo> dataList = new List<ShowPlayerInfo>(); //元素实例列表，控制滚动时变动哪个元素
    private List<ShowPlayerInfo> dataListOrigin = new List<ShowPlayerInfo>(); //保存原始顺序
    private int totalCount; //总的数据数量
    private int headIndex; //头下标
    private int tailIndex; //尾下标
    private Vector2 firstItemAnchoredPos; //第一个元素的坐标
    private int fixedCount; //总的滚动元素需要数量
    private JSONNode rankInfo; //读取排名信息用
    private int countdown; //倒计时，单位秒
    private bool initTrigger = true; //第一次按按钮初始化，之后不用

    /// <summary>
    /// 监听滚动的事件，在检测到有元素超出显示范围时复用
    /// </summary>
    private void OnScroll()
    {
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

    /// <summary>
    /// 初始化排名显示
    /// </summary>
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

    /// <summary>
    /// 关闭按钮调用，重新初始化排名列表，减少创建开销
    /// </summary>
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

    /// <summary>
    /// 获取第一个显示元素的位置
    /// </summary>
    private void GetFirstItemAnchoredPos()
    {
        firstItemAnchoredPos = new Vector2
        (
            layout.padding.left + layout.cellSize.x / 2,
            -layout.padding.top - layout.cellSize.y / 2
        );
    }

    /// <summary>
    /// 设置每个显示元素的位置
    /// </summary>
    /// 显示元素
    /// <param name="trans"></param>
    /// 显示的是第几个元素
    /// <param name="index"></param>
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

    /// <summary>
    /// 设置显示元素内容
    /// </summary>
    /// 显示元素
    /// <param name="trans"></param>
    /// 对应显示内容顺序
    /// <param name="index"></param>
    private void SetRankInfo(ShowPlayerInfo trans, int index)
    {
        trans.ShowRankInfo(UserList[index], index);
    }

    /// <summary>
    /// 事件格式转换
    /// </summary>
    /// 秒的数量
    /// <param name="totalSeconds"></param>
    /// 返回倒计时字符串样式
    /// <returns></returns>
    public string FormatDayTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
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

    /// <summary>
    /// 刷新倒计时用的协程
    /// </summary>
    /// 秒的数量
    /// <param name="total"></param>
    /// <returns></returns>
    IEnumerator StartCountdown(int total)
    {
        for (; total > 0; total--)
        {
            Countdown.text = FormatDayTime(total);
            total -= 1;
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 切换到排名UI
    /// </summary>
    public void ToUI()
    {
        UI.SetActive(true);
        Button.SetActive(false);
        if (initTrigger) StartCoroutine(Init()); //用协程初始化，避免用户等待
    }

    /// <summary>
    /// 关闭排名UI
    /// </summary>
    public void Close()
    {
        UI.SetActive(false);
        Button.SetActive(true);
        content.localPosition = new Vector3(0f, 0f, 0f);
        ReInitRankList();
    }

    /// <summary>
    /// 初始化用的协程
    /// </summary>
    /// <returns></returns>
    IEnumerator Init()
    {
        fixedCount = (int) (rectTransform.rect.size.y / (layout.spacing.y + layout.cellSize.y)) + 2; //计算需要的显示元素数量，减少开销
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

        UserList.Sort((x, y) => -x.Trophy.CompareTo(y.Trophy)); //给用户排名
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