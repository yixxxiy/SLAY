namespace XGame
{
    public enum QuestConditionTypeEnum
    {
        /**
         * 捕获帕鲁
         */
        CAPTURE = 0,
        
        /**
         * 获得物品
         */
        OBTAIN = 1,
        
        /**
         * 制造物品
         */
        MANUFACTURE = 2,
        
        /**
         * 拥有物品
         */
        POSSESS = 3
    }
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
        public int conditionObjectId;

        /**
         * 是否已达成
         */
        public bool achieved;
    }
}