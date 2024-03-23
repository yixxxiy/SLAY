using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace XGame
{
    /**
     * 任务更新事件
     */
    public class QuestUpdateEvent
    {
        public Quest quest;

        public QuestUpdateEvent(Quest quest)
        {
            this.quest = quest;
        }
    }

    public class UI_QuestBar : UIView
    {
        [SerializeField] private GameObject UI_QuestItemPrefab;

        private Button exitButton;

        private Button testButton;

        private Dictionary<int, UI_QuestItem> questItemDict;

        private Transform contentTransform;

        public override UILayers Layer
        {
            get { return UILayers.NormalLayer; }
        }

        public override bool IsSingle => true;

        public override void OnHide()
        {
            foreach (UI_QuestItem questItem in questItemDict.Values)
            {
                questItem.OnHide();
                questItem.gameObject.SetActive(false);
            }

            this.UnRegisterEvent<QuestUpdateEvent>();
        }

        public override void OnInit()
        {
            exitButton = this.transform.Find("exit").GetComponent<Button>();
            exitButton.onClick.AddListener(onClickExit);
            testButton = this.transform.Find("updateQuestForTest").GetComponent<Button>();
            testButton.onClick.AddListener(onClickTest);
            contentTransform = this.transform.Find("ScrollView").Find("Viewport").Find("Content");
            questItemDict = new Dictionary<int, UI_QuestItem>();
            QuestManager.Instance.loadQuest();
        }

        public override void OnShow(object obj)
        {
            this.RegisterEvent<QuestUpdateEvent>(questUpdate);
            List<Quest> activeQuestList = QuestManager.Instance.getQuestListExceptHide();

            foreach (Quest quest in activeQuestList)
            {
                UI_QuestItem questItem = getQuestItem(quest);
                questItem.OnShow(quest);
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
            questItemDict.Add(quest.questId, questItem);
            return questItem;
        }

        /// <summary>
        /// 任务更新时，调用此方法，更新任务项UI
        /// </summary>
        /// <param name="questUpdateEvent"></param>
        private void questUpdate(QuestUpdateEvent questUpdateEvent)
        {
            Quest quest = questUpdateEvent.quest;
            UI_QuestItem questItem = getQuestItem(quest);
            questItem.OnShow(quest);
            questItem.gameObject.SetActive(true);
        }

        /// <summary>
        /// 退出任务栏
        /// </summary>
        private void onClickExit()
        {
            XGame.MainController.HideUI<UI_QuestBar>();
        }

        /// <summary>
        /// 测试专用，用于测试任务更新
        /// </summary>
        private void onClickTest()
        {
            QuestManager.Instance.updateQuestCondition(QuestConditionTypeEnum.CAPTURE,
                QuestConditionObjectEnum.WOOD, 1);
        }
    }
}