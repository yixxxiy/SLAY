using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace XGame
{
    public class UI_QuestItem : UIView
    {
        [SerializeField] private TextMeshProUGUI description;

        [SerializeField] private TextMeshProUGUI reward;

        [SerializeField] private TextMeshProUGUI progress;

        [SerializeField] private TextMeshProUGUI status;

        private Quest quest;

        private string FINISHED = "已达成";

        private string SUBMITED = "已完成";

        public override UILayers Layer
        {
            get { return UILayers.NormalLayer; }
        }

        public override bool IsSingle => false;

        public override void OnHide()
        {
        }

        public override void OnInit()
        {
            description = this.transform.Find("description").GetComponent<TextMeshProUGUI>();
            reward = this.transform.Find("reward").GetComponent<TextMeshProUGUI>();
            progress = this.transform.Find("progress").GetComponent<TextMeshProUGUI>();
            status = this.transform.Find("status").GetComponent<TextMeshProUGUI>();
        }

        public override void OnShow(object obj)
        {
            quest = (Quest)obj;
            //任务名称
            description.text = quest.questName;
            //任务奖励
            StringBuilder rewardBuilder = new StringBuilder();
            rewardBuilder.Append("任务奖励\n");
            foreach (QuestReward questReward in quest.questRewardList)
            {
                rewardBuilder.Append(questReward.rewardObject + " " + questReward.rewardNum + "\n");
            }

            reward.text = rewardBuilder.ToString();
            //任务进度
            StringBuilder conditionBuilder = new StringBuilder();
            conditionBuilder.Append("任务进度\n");
            foreach (QuestCondition questCondition in quest.questConditionList)
            {
                conditionBuilder.Append(questCondition.currentNum + " / " + questCondition.conditionNum + "\n");
            }

            progress.text = conditionBuilder.ToString();
            this.gameObject.name = "quest: " + quest.questId;
            //任务状态
            switch (quest.questStatus)
            {
                case (byte)QuestStatusEnum.FINISHED:
                    status.text = FINISHED;
                    status.gameObject.SetActive(true);
                    break;
                case (byte)QuestStatusEnum.SUBMITED:
                    status.text = SUBMITED;
                    status.gameObject.SetActive(true);
                    break;
                default:
                    status.gameObject.SetActive(false);
                    break;
            }
        }

        public void onClick()
        {
            QuestManager.Instance.submitQuest(quest.questId);
        }
    }
}