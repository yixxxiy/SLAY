using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGame
{

    public class GameDataManager : Singleton<GameDataManager>
    {
        string saveKey
        {
            get
            {
                return "X" + "Game" + "Data";
            }

        }
        public GameData Data;

        /// <summary>
        /// 读取玩家信息
        /// </summary>
        public void LoadData()
        {
            if (!PlayerPrefs.HasKey(saveKey))
            {
                NewData();
                Save();
            }
            else
            {
                Data = LocalDataUtil.GetObject<GameData>(saveKey);
                //更新最后登录时间

            }
        }




        /// <summary>
        /// 清零每天数据
        /// </summary>
        public void RefreshDayData()
        {
            Data.LastRefreshDataTime = System.DateTime.Now;
            Data.LoginDay++;
            EventCenterManager.Send<NewDayEvent>();
        }


        /// <summary>
        /// 保存数据
        /// </summary>
        public void Save()
        {
            if (Data != null) LocalDataUtil.SetObject<GameData>(saveKey, Data);
            PlayerPrefs.Save();
        }


        /// <summary>
        /// 生成新的玩家数据
        /// </summary>
        void NewData()
        {
            Data = new GameData()
            {
                //账号基础数据
                RegisterTime = System.DateTime.Now,
                LastLoginTime = System.DateTime.Now,
                LastRefreshDataTime = System.DateTime.Now,
                ADID = string.Empty,
                LoginDay = 1,
                Gold = 0,

                ISuODA = false,
            };
            QuestManager.Instance.parseJson();
            setQuestDict();
        }

        /// <summary>
        /// 获取一种奖励类型的数量
        /// </summary>
        /// <returns></returns>
        public int GetNumByType(Reward rewardtype)
        {
            switch (rewardtype)
            {
                case Reward.Gold:
                    return Data.Gold;
                default:
                    return 0;
            }
        }



        /// <summary>
        ///  播放动画并添加相应数值
        /// </summary>
        /// <param name="rewardtype"></param>
        /// <param name="value"></param>
        public void AddRewardWithAnim(Reward rewardtype, int value, Vector3 fromP)
        {
            if (fromP == null) return;
            if (rewardtype == Reward.Gold)
            {
                FlyRewardManager.Instance.FlyTo(rewardtype, value, fromP);
                AddReward(rewardtype, value, false);
            }
            else
            {
                AddReward(rewardtype, value, true);
            }

        }


        /// <summary>
        /// 增加数值
        /// </summary>
        /// <param name="rewardtype"></param>
        /// <param name="value"></param>
        /// <param name="updateText"></param>
        public void AddReward(Reward rewardtype, int value, bool updateText = true)
        {
            switch (rewardtype)
            {
                case Reward.Gold:
                    Data.Gold += value;
                    if (updateText)
                    {
                        EventCenterManager.Send<UpdateGoldEvent>();
                    }
                    break;
                default:
                    break;

            }
        }

       /// <summary>
       /// 将读取到的任务赋给data
       /// </summary>
        public void setQuestDict()
        {
            Data.QuestDict = QuestManager.Instance.questDict;
        }

    }
}

