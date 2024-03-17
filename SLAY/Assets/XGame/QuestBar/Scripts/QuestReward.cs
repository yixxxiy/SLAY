using System;

namespace XGame
{
    /**
     * 任务奖励类
     */
    [Serializable]
    public class QuestReward
    {
        /**
         * 任务奖励物品类型
         */
        private byte rewardType;

        /**
         * 任务奖励数量
         */
        private int rewardNum;

        public byte RewardType
        {
            get => rewardType;
            set => rewardType = value;
        }

        public int RewardNum
        {
            get => rewardNum;
            set => rewardNum = value;
        }
    }
}