using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

namespace XGame
{


    public class MenuDate
    {
        public int OpenBy = 0;
        public int PALUID;
        public GameObject dongtaiwuti;
        public GameObject Item;
    }

    public class UI_Menu : UIView
    {
        public Image PaluSprite;

        public override UILayers Layer
        {
            get
            {
                return UILayers.BackgroundLayer;
            }
        }

        MenuDate mdate;

        public Button mCloseBtn;

        public Text mGoldText;
        public override bool IsSingle => base.IsSingle;
        /// <summary>
        /// 初始化组件
        /// </summary>
        void InitCom()  // Awake  Start
        {

            mCloseBtn = transform.Find<Button>("Button");
        }


        public override void OnInit()
        {
            InitCom();
            mCloseBtn.AddClickEvent(OnCloseBtnClick);
            mCloseBtn.AddClickEvent(() =>
            {
                CloseSelf();
            });

            UpdateGoldText(null);
            this.RegisterEvent<SentUpdateGoldText>(UpdateGoldText);  //事件监测注册
        }

        void UpdateGoldText(SentUpdateGoldText e)
        {

            mGoldText.text = MainController.UserData.Gold.ToString();

            mGoldText.text = MainController.GetString(LanguageEnum.UI_Tip);

            if (e.isShowPalu)
            {
                PaluSprite.gameObject.SetActive(true);
            }
            else
            {
                PaluSprite.gameObject.SetActive(false);
            }
        }


        void OnCloseBtnClick()
        {
            CloseSelf();
        }

        public override void OnShow(object msg)  //面板展示
        {
            mdate = msg as MenuDate;

            var get =  mdate.dongtaiwuti;
            get.transform.SetParent(this.transform);
            if (mdate.PALUID == 1)
            {
                PaluSprite.gameObject.SetActive(true);
                PaluSprite.sprite = ResourceUtil.GetSprite("MainMenu","Pallu" + mdate.PALUID.ToString());
                
            }
            //this.StartTimer
        }

        public override void OnReShow()
        {

        }
        public override void OnHide()  //面板关闭
        {

            this.ReleaseTimer();
            this.UnRegisterEvent<SentUpdateGoldText>();  //事件注销
        }

        public override void DoShowAnim(Action cb)
        {
            
        }

        public override void DoHideAnim(Action cb)
        {
           
        }
        public override void SetLanguage()
        {
            var lc = gameObject.AddComponent<LanguageComponent>();
        }
    }



}

