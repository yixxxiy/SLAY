using TMPro;
using UnityEngine.UI;

namespace XGame
{
    public class UI_QuestDialog : UIView
    {
        private TextMeshProUGUI dialogInfo;

        private Button accept;
        
        public override UILayers Layer
        {
            get { return UILayers.MainLayer; }
        }


        public override void OnInit()
        {
            dialogInfo = this.transform.Find("dialogInfo").GetComponent<TextMeshProUGUI>();
            accept = this.transform.Find("accept").GetComponent<Button>();
            accept.onClick.AddListener(onClick);
        }

        public override void OnShow(object obj)
        {
            dialogInfo.text = (string)obj;
        }

        public override void OnHide()
        {
           
        }

        private void onClick()
        {
            XGame.MainController.HideUI<UI_QuestDialog>();
        }
    }
}