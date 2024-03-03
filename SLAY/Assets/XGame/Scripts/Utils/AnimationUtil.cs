using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace XGame
{
    public static class AnimationUtil
    {
        static Dictionary<int, Tween> mAnim = new Dictionary<int, Tween>();
        static string GetTextByType(Reward rewardType, int num)
        {
            switch (rewardType)
            {
                case Reward.Gold:
                    return num.GetTokenShowString();
                default:
                    return num.ToString();
            }
        }

        //播放数字变化动画 配合飞金币动画使用
        public static void PlayChangeNum(this UnityEngine.UI.Text text, Reward rewardType, int bNum)
        {
            int eNum = MainController.DataMgr.GetNumByType(rewardType);
            if (mAnim.ContainsKey(text.GetHashCode()) && mAnim[text.GetHashCode()] != null)
            {
                mAnim[text.GetHashCode()].Kill();
            }
            var anim = DOTween.To(() => bNum, n => text.text = GetTextByType(rewardType, n), eNum, 0.3f);
            anim.onComplete += () =>
            {
                mAnim.Remove(text.GetHashCode());
            };
            mAnim[text.GetHashCode()] = anim;
        }
        public static void ShowWindowAnim(Transform transform, System.Action cb)
        {
            if (transform.childCount < 3)
            {
                LogUtil.Err("结构不对!不能使用此动画");
                return;
            }
            Transform maskbg = transform.GetChild(0);
            if (maskbg.GetComponent<AutoFitBg>() == null) maskbg.gameObject.AddComponent<AutoFitBg>();
            CanvasGroup maskBGcg = maskbg.GetComponent<CanvasGroup>();
            if (maskBGcg == null)
            {
                maskBGcg = maskbg.gameObject.AddComponent<CanvasGroup>();
            }

            maskBGcg.alpha = 0;
            maskBGcg.DOFade(1, 0.1f);

            Transform ts = transform.GetChild(1);//这对面板的搭建有一定的要求
            CanvasGroup tsCg = ts.GetComponent<CanvasGroup>();
            if (tsCg == null)
            {
                tsCg = ts.gameObject.AddComponent<CanvasGroup>();
            }
            tsCg.alpha = 0;

            Transform mask = transform.GetChild(2);//这对面板的搭建有一定的要求

            mask.gameObject.SetActive(true);
            ts.localScale = Vector3.one / 2;


            Sequence quence = DOTween.Sequence();
            quence.Append(ts.DOScale(Vector3.one * 1.2f, 0.2f));
            quence.Append(ts.DOScale(Vector3.one, 0.05f));
            quence.Insert(0, tsCg.DOFade(1, 0.2f));
            quence.AppendCallback(() =>
            {
                mask.gameObject.SetActive(false);
                cb?.Invoke();
            });
            quence.Play();
        }
        public static void HideWindowAnim(Transform transform, System.Action cb)
        {
            if (transform.childCount < 3)
            {
                LogUtil.Err("结构不对!不能使用此动画");
                return;
            }
            Transform maskbg = transform.GetChild(0);
            CanvasGroup maskBGcg = maskbg.GetComponent<CanvasGroup>();
            if (maskBGcg == null)
            {
                maskBGcg = maskbg.gameObject.AddComponent<CanvasGroup>();
            }
            maskBGcg.alpha = 1;
            maskBGcg.DOFade(0, 0.1f);

            Transform ts = transform.GetChild(1);
            CanvasGroup tsCg = ts.GetComponent<CanvasGroup>();
            if (tsCg == null)
            {
                tsCg = ts.gameObject.AddComponent<CanvasGroup>();
            }


            Transform mask = transform.GetChild(2);
            mask.gameObject.SetActive(true);
            ts.localScale = Vector3.one;
            tsCg.alpha = 1;
            Sequence quence = DOTween.Sequence();
            quence.Append(ts.DOScale(Vector3.one * 1.2f, 0.05f));
            quence.Append(ts.DOScale(Vector3.one / 2, 0.2f));
            quence.Insert(0.05f, tsCg.DOFade(0, 0.2f));
            quence.AppendCallback(() =>
            {
                cb?.Invoke();
                mask.gameObject.SetActive(false);
            }
            );
            quence.Play();
        }

        /// <summary>
        /// 播放星星动画
        /// </summary>
        /// <param name="stars"></param>
        public static void PlayStarAnim(List<Transform> stars)
        {
            foreach (var img in stars)
            {
                img.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0);
            }
            var tmp = new List<Transform>();
            tmp.AddRange(stars);

            int starNum = Random.Range(2, 6);
            var values = new List<Transform>();
            while (values.Count < starNum && tmp.Count > 0)
            {
                var r = Random.Range(0, tmp.Count);
                values.Add(tmp[r]);
                tmp.RemoveAt(r);
            }
            for (int i = 0; i < values.Count; i++)
            {
                float randstart = Random.Range(0f, 0.3f);
                values[i].transform.localScale = new Vector3(randstart, randstart, randstart);
                var index = i;
                float randend = Random.Range(0.5f, 0.7f);
                var image = values[i].GetComponent<UnityEngine.UI.Image>();

                values[i].gameObject.SetActive(true);
                values[i].transform.DOScale(randend, 0.35f).SetDelay(i * 0.2f);
                image.DOFade(1, 0.35f).SetDelay(i * 0.2f).onComplete += () =>
                 {
                     image.DOFade(0, 0.35f);
                     values[index].transform.DOScale(randstart, 0.35f).onComplete += () =>
                     {
                         values[index].gameObject.SetActive(false);
                         if (index == values.Count - 1)
                         {
                             values.Clear();
                             PlayStarAnim(stars);
                         }
                     };
                 };
            }
        }
    }

}
