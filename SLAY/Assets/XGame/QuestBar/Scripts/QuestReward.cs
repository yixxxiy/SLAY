using System;

namespace XGame
{
    public enum QuestRewardObjectEnum
    {
        APPLE = 1,
        
        EXP = 2
    }
    /**
     * 任务奖励类
     */
    [Serializable]
    public class QuestReward
    {
        /**
         * 任务奖励物品类型
         */
        public byte rewardObject;

        /**
         * 任务奖励数量
         */
        public int rewardNum;
    }
}