using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace XGame
{
    

    public class UI_QuestBar : UIView
    {
        [SerializeField]
        private TextAsset missionList;

        [SerializeField]
        private GameObject UI_QuestItemPrefab;

        private Dictionary<int, UI_QuestItem> questItemDict;

        private Transform contentTransform;
        public override UILayers Layer
        {
            get
            {
                return UILayers.NormalLayer;
            }
        }

        public override bool IsSingle => true;
        public override void OnHide()
        {
            foreach (UI_QuestItem questItem in questItemDict.Values)
            {
                questItem.OnHide();
                questItem.gameObject.SetActive(false);
            }
            
        }

        public override void OnInit()
        {
            contentTransform = this.transform.Find("ScrollView").Find("Viewport").Find("Content");
            questItemDict = new Dictionary<int, UI_QuestItem>();
        }

        public override void OnShow(object obj)
        {
            QuestManager.parseJson(missionList.text);
            List<Quest> activeQuestList = QuestManager.getActiveQuestList();
            
            
            foreach (Quest quest in activeQuestList)
            {
                UI_QuestItem questItem = getQuestItem(quest);
                questItem.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 从缓存中获取单个任务UI，若不存在则生成并添加到缓存中
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        private UI_QuestItem getQuestItem(Quest quest)
        {
            if (questItemDict.ContainsKey(quest.questId))
            {
                return questItemDict[quest.questId];
            }
            UI_QuestItem questItem = Instantiate(UI_QuestItemPrefab, contentTransform).GetComponent<UI_QuestItem>();
            questItem.OnInit();
            questItem.OnShow(quest);
            questItemDict.Add(quest.questId, questItem);
            return questItem;
        }
    }
}
