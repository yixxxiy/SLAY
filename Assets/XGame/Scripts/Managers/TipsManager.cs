using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
namespace XGame
{
    /// <summary>
    /// 简易的提示系统
    /// </summary>
    public class TipsManager : Singleton<TipsManager>
    {
        private GameObject tipsManager;

        private CanvasGroup mCG;

        public List<CanvasGroup> mHideList = new List<CanvasGroup>();

        public List<CanvasGroup> mWaiUpList = new List<CanvasGroup>();
        public void Init()
        {
            if (tipsManager != null) return;
            tipsManager = ResourceUtil.Instantiate<GameObject>("Prefabs/UI/TipsCanvas");
            GameObject.DontDestroyOnLoad(tipsManager);
            tipsManager.name = "TipsManager";

            mCG = tipsManager.transform.GetChild(0).GetComponent<CanvasGroup>();
            mCG.gameObject.SetActive(false);
            mHideList.Add(mCG);
        }
        public void ShowTips(string content, float durationTime = 1f)
        {
            CanvasGroup t;
            if (mWaiUpList.Count > 0)
            {
                t = mWaiUpList[0];
            }
            else
            {
                if (mHideList.Count > 0)
                {
                    t = mHideList[0];
                    mHideList.RemoveAt(0);
                }
                else
                {
                    t = GameObject.Instantiate(mCG, mCG.transform.parent);
                }
                mWaiUpList.Add(t);

                DOVirtual.DelayedCall(0.4f, () =>
                {
                    if (mWaiUpList.Count > 0)
                    {
                        var t2 = mWaiUpList[0];
                        mWaiUpList.RemoveAt(0);
                        t2.transform.DOLocalMoveY(430, 1).SetDelay(0.5f).onComplete += () =>
                        {
                            t2.DOFade(0, 0.3f).SetDelay(0.3f).onComplete += () =>
                            {
                                t2.gameObject.SetActive(false);
                                mHideList.Add(t2);
                            };
                        };
                    }
                });
            }

            t.gameObject.SetActive(true);
            t.transform.GetChild(0).GetComponent<Text>().text = content;
            t.alpha = 1;
            t.transform.localScale = new Vector3(1, 0, 1);
            t.transform.localPosition = new Vector3(0, 159, 0);
            t.transform.DOKill();
            t.transform.DOScaleY(1, 0.04f);
        }
    }
}
