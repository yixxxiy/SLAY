using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class UI_Loading : UIView
    {
      

        private void LoadOverTrigger()
        {
            //进游戏
            MainController.ShowUI<UI_Menu>();
        }
  

        [SerializeField]
        [Header("进度条")]
        private Image mProgressImage;
        [SerializeField]
        [Header("进度文本")]
        private Text mProgressText;
        [SerializeField]
        [Header("提示文字")]
        private Text mTipsText;
        private float loadingMaxTime = 15;
        private float loadingMinTime = 6;
        private async void LoadingAsync()
        {
            float timer = 0;
            float speed = 1;
            float maxSpeed = loadingMaxTime / loadingMinTime;
            float progress = 0;
            mProgressText.text = "0%";
            mProgressImage.fillAmount = 0;

            while (timer < loadingMaxTime)
            {
                await new WaitForEndOfFrame();
                if (speed != maxSpeed)
                {
                    speed = maxSpeed;
                }
                timer += Time.deltaTime * speed;
                if (timer > loadingMaxTime)
                {
                    timer = loadingMaxTime;
                }
                progress = timer / loadingMaxTime;
                mProgressText.text = (int)(progress * 100) + "%";
                mProgressImage.fillAmount = progress;
            }
            await new WaitForEndOfFrame();
            CloseSelf(true);
            LoadOverTrigger();

            
        }

        public override UILayers Layer
        {
            get
            {
                return UILayers.LoadingLayer;
            }
        }
        public override void OnInit()
        {
            mTipsText.text = "";
        }
        public override void OnShow(object msg)
        {
            LoadingAsync();
        }
        public override void OnHide()
        {

        }

        public override void SetLanguage()
        {

        }
    }

}