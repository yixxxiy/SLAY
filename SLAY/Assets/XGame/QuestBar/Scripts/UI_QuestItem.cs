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
        [SerializeField]
        private TextMeshProUGUI description;

        [SerializeField]
        private TextMeshProUGUI reward;

        [SerializeField]
        private TextMeshProUGUI progress;
        
        public override UILayers Layer
        {
            get
            {
                return UILayers.NormalLayer;
            }
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
        }

        public override void OnShow(object obj)
        {
            Quest quest = (Quest)obj;
            //任务名称
            description.text = quest.questName;
            //任务奖励
            StringBuilder rewardBuilder = new StringBuilder();
            rewardBuilder.Append("任务奖励\n");
            foreach (QuestReward questReward in quest.questRewardList)
            {
                rewardBuilder.Append(questReward.RewardType + " " + questReward.RewardNum + "\n");
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
            this.gameObject.name = "quest: "+ quest.questId;
           
            
        }
    }
}

