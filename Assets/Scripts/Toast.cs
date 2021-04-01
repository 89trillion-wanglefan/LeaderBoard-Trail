using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(Toast))]
public class Toast : MonoBehaviour
{
    private static Toast instance;
    private Toast(){}
    
    [SerializeField] private Image toast;
    [SerializeField] private Text txt;

    private void Awake()
    {
        instance = this;
    }

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