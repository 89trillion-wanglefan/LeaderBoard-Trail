using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用户的数据类
/// </summary>
public class Users
{
    public long Id { get; set; }
    public string Nickname { get; set; }
    public int Trophy { get; set; }

    public Users()
    {
    }

    public Users(long id, string nickname, int trophy)
    {
        Id = id;
        Nickname = nickname;
        Trophy = trophy;
    }
}