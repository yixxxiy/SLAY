using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace XGame
{
    
    /**
     * 任务类
     */
    public enum QuestStatusEnum : byte
    {
        /**
         * 隐藏状态
         */
        HIDE = 0,

        /**
         * 激活状态
         */
        ACTIVE = 1,

        /**
         * 完成状态
         */
        FINISHED = 2,
        
        /**
         * 已提交状态
         */
        SUBMITED = 3
    }

    public enum QuestTypeEnum : byte
    {
        /**
         * 新手任务
         */
        BEGINNER = 0
    }

    [System.Serializable]
    public class Quest
    {
        /**
         * 任务ID
         */
        public int questId;

        /**
         * 任务名称
         */
        public string questName;

        /**
         * 任务描述
         */
        public string questDescription;

        /**
         * 任务类型，详情见QuestTypeEnum
         */
        public byte questType;

        /**
         * 任务激活等级，仅当玩家等级达到特定等级才可以激活
         */
        public int activeLevel;

        /**
         * 前置任务列表字符串，仅用于解析
         */
        public string preQuests;
        /**
         * 前置任务列表，当前置任务均完成时，该任务激活
         */
        public List<int> preQuestList;
        
        /**
         * 任务状态, 详情见QuestStatusEnum
         */
        public byte questStatus;
        
        /**
         * 任务奖励类型字符串，仅用于解析
         */
        public string questRewardObjects;

        /**
         * 任务奖励数量字符串，仅用于解析
         */
        public string questRewardNums;
        /**
         * 任务奖励
         */
        public List<QuestReward> questRewardList;

        /**
         * 任务达成条件物品ID，仅用于解析
         */
        public string questConditionObjects;

        /**
         * 任务达成条件物品数量，仅用于解析
         */
        public string questConditionNums;

        /**
         * 任务达成条件类型，仅用于解析
         */
        public string questConditionTypes;
        
        /**
         * 任务达成条件
         */
        public List<QuestCondition> questConditionList;

    }
}