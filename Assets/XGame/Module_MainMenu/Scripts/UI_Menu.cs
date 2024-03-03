using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace XGame
{

    public class UI_Menu : UIView
    {

     
        public override UILayers Layer
        {
            get
            {
                return UILayers.BackgroundLayer;
            }
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        void InitCom()
        {

            
        }
       

        public override void OnInit()
        {
            InitCom();

        }

        public override void OnShow(object msg)
        {
          
        }

        
        public override void OnHide()
        {

            this.ReleaseTimer();
        }
        public override void SetLanguage()
        {
            var lc = gameObject.AddComponent<LanguageComponent>();
        }
    }

  

}

