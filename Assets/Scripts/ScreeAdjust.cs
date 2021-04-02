using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreeAdjust : MonoBehaviour
{
    public List<CanvasScaler> canvass=new List<CanvasScaler>();//涉及屏幕适配的所有画布
    
    // Start is called before the first frame update
    private void Start()
    {
        MatchScreen();
    }

    /// <summary>
    /// 调整屏幕适配的方案，根据用户屏幕宽高比调整适配方向
    /// </summary>
    private void MatchScreen()
    {
        if (Screen.width / (float)Screen.height > 1080 / (float)1920)
        {
            foreach (var t in canvass)
            {
                t.matchWidthOrHeight = 1;
            }
        }
        else
        {
            foreach (var t in canvass)
            {
                t.matchWidthOrHeight = 0;
            }
        }
    }
}
