using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
/// <summary>
/// toast的单例实现
/// </summary>
[RequireComponent(typeof(Toast))]
public class Toast : MonoBehaviour
{
    private static Toast instance;
    private Toast(){}
    
    [SerializeField] private Image toast;
    [SerializeField] private Text txt;

    /// <summary>
    /// 初始化时创建
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 公共属性
    /// </summary>
    public static Toast Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("tot");
                instance = obj.AddComponent<Toast>();
            }

            return instance;
        }
    }

    /// <summary>
    /// toast的显示动画
    /// </summary>
    /// 显示的内容
    /// <param name="info"></param>
    public void ShowToast(string info)
    {
        txt.text = info;
        toast.DOFade(1, 0);
        toast.DOFade(1, 1);
        txt.DOFade(1, 0);
        txt.DOFade(1, 1).OnComplete(() =>
        {
            txt.DOFade(0, 1); 
            toast.DOFade(0, 1);
        });
    }
}