using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using LitJson;

namespace XGame
{
    [System.Serializable]
    public class QuestCollection
    {
        public List<Quest> questList;
    }

    public class QuestManager
    {
        private static HashSet<int> finishedQuestSet = new HashSet<int>();

        private static Dictionary<int, Quest> questDict = new Dictionary<int, Quest>();


        /// <summary>
        /// 读取Json并转化为任务字典
        /// </summary>
        /// <param name="json"></param>
        public static void parseJson(string json)
        {
            QuestCollection questCollection = JsonUtility.FromJson<QuestCollection>(json);
            questDict = questCollection.questList.Select(quest =>
            {
                //如果前置任务不为空，则将其分割并转化为List
                if (!string.IsNullOrEmpty(quest.preQuests))
                {
                    quest.preQuestList = quest.preQuests.Split(",").Select(preQuestId => int.Parse(preQuestId))
                        .ToList();
                }

                //如果任务奖励不为空，则将其转化为奖励列表
                if (!string.IsNullOrEmpty(quest.questRewardObjects) && !string.IsNullOrEmpty(quest.questRewardNums))
                {
                    string[] rewardObjectArray = quest.questRewardObjects.Split(",");
                    string[] rewardNumArray = quest.questRewardNums.Split(",");
                    //奖励数量和奖励物品数量不一致时，直接报错
                    if (rewardNumArray.Length != rewardObjectArray.Length)
                    {
                        throw new ArgumentException(quest.questId + " have different reward Number");
                    }

                    List<QuestReward> questRewardList = new List<QuestReward>();
                    for (int i = 0; i < rewardNumArray.Length; i++)
                    {
                        QuestReward questReward = new QuestReward();
                        questReward.RewardType = byte.Parse(rewardObjectArray[i]);
                        questReward.RewardNum = int.Parse(rewardNumArray[i]);
                        questRewardList.Add(questReward);
                    }

                    quest.questRewardList = questRewardList;
                }

                //如果任务达成条件不为空，则将其转化为达成条件列表
                if (!string.IsNullOrEmpty(quest.questConditionObjects) &&
                    !string.IsNullOrEmpty(quest.questConditionNums) && !string.IsNullOrEmpty(quest.questConditionTypes))
                {
                    string[] conditionObjectArray = quest.questConditionObjects.Split(",");
                    string[] conditionTypeArray = quest.questConditionTypes.Split(",");
                    string[] conditionNumtArray = quest.questConditionNums.Split(",");
                    //达成条件数量、达成条件物品数量、达成条件类型不一致时，直接报错
                    if (conditionObjectArray.Length != conditionTypeArray.Length ||
                        conditionTypeArray.Length != conditionNumtArray.Length)
                    {
                        throw new ArgumentException(quest.questId + " have different condition Number");
                    }
                    List<QuestCondition> questConditionList = new List<QuestCondition>();
                    for (int i = 0; i < conditionObjectArray.Length; i++)
                    {
                        QuestCondition questCondition = new QuestCondition();
                        questCondition.conditionType = byte.Parse(conditionTypeArray[i]);
                        questCondition.conditionNum = int.Parse(conditionNumtArray[i]);
                        questCondition.conditionObjectId = int.Parse(conditionObjectArray[i]);
                        questCondition.achieved = false;
                        questCondition.currentNum = 0;
                        questConditionList.Add(questCondition);
                    }

                    quest.questConditionList = questConditionList;
                }

                return quest;
            }).ToDictionary(quest => quest.questId);
        }


        /// <summary>
        /// 刷新单个任务的状态
        /// </summary>
        /// <param name="quest"></param>
        public static void freshQuestStatus(Quest quest)
        {
            if (quest == null)
            {
                return;
            }

            if (quest.questStatus == (byte)QuestStatusEnum.FINISHED)
            {
                return;
            }

            if (finishedQuestSet.Contains(quest.questId))
            {
                quest.questStatus = (byte)QuestStatusEnum.FINISHED;
                return;
            }

            if (quest.preQuestList == null || quest.preQuestList.Count == 0)
            {
                quest.questStatus = (byte)QuestStatusEnum.ACTIVE;
                return;
            }

            bool allExist = quest.preQuestList.All(questId => finishedQuestSet.Contains(questId));
            if (allExist)
            {
                quest.questStatus = (byte)QuestStatusEnum.ACTIVE;
            }
            else
            {
                quest.questStatus = (byte)QuestStatusEnum.HIDE;
            }
        }

        /// <summary>
        /// 刷新任务列表的状态
        /// </summary>
        /// <param name="questList"></param>
        private static void freshQuestStatus()
        {
            foreach (int questId in questDict.Keys)
            {
                freshQuestStatus(questDict[questId]);
            }
        }

        /// <summary>
        /// 将任务设为已完成
        /// </summary>
        /// <param name="questId"></param>
        public static void finishQuest(int questId)
        {
            Quest quest = questDict[questId];
            if (quest == null)
            {
                Debug.LogError(questId + " not found");
                return;
            }

            finishedQuestSet.Add(questId);
            quest.questStatus = (byte)QuestStatusEnum.FINISHED;
        }

        /// <summary>
        /// 获取激活的任务列表
        /// </summary>
        /// <returns></returns>
        public static List<Quest> getActiveQuestList()
        {
            freshQuestStatus();
            return questDict.Values.Where(quest => quest.questStatus == (byte)QuestStatusEnum.ACTIVE)
                .OrderBy(quest => quest.questId).ToList();
        }

        /// <summary>
        /// 获取已完成的任务列表
        /// </summary>
        /// <returns></returns>
        public static List<Quest> getFinishedQuestList()
        {
            freshQuestStatus();
            return questDict.Values.Where(quest => quest.questStatus == (byte)QuestStatusEnum.FINISHED)
                .OrderBy(quest => quest.questId).ToList();
        }
    }
}