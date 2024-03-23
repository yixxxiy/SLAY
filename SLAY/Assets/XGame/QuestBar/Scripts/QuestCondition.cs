using System;

namespace XGame
{
    public enum QuestConditionTypeEnum
    {
        /**
         * 捕获帕鲁
         */
        CAPTURE = 0,

        /**
         * 获取物品
         */
        OBTAIN = 1,
     
        /**
         * 拥有物品
         */
        POSSESS = 2,
        
        /**
         * 击杀
         */
        KILLED = 3
    }

    public enum QuestConditionObjectEnum
    {
        /**
         * 木材
         */
        WOOD = 1,

        /**
         * 苹果
         */
        APPLE = 0
    }
    [Serializable]

    public class QuestCondition
    {
        /**
         * 达成条件类型，详情见QuestConditionTypeEnum
         */
        public byte conditionType;

        /**
         * 达成条件数量
         */
        public int conditionNum;

        /**
         * 目前已达成条件数量
         */
        public int currentNum;

        /**
         * 达成条件物品ID
         */
        public int conditionObject;

        /**
         * 是否已达成
         */
        public bool achieved;
    }
}