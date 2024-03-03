using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XGame
{
    [System.Serializable]
    public class WeaponInfo
    {
        public int Id;
        public int PieceListIndex;
        public int CurSkinId;
        public bool IsGet;
    }
    [System.Serializable]
    public class GameData
    {
        //账号基础数据
        public System.DateTime RegisterTime;    //注册时间
        public System.DateTime LastLoginTime; //上次登陆时间
        public System.DateTime LastRefreshDataTime; //最近刷新数据时间
        public System.DateTime NextOffLineRewardTime;
        public string ADID;
        public int PlayGameTimes;
        public int LoginDay;
        public int Gold;
    }

}
