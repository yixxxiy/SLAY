using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace XGame
{

    public class FlyRewardManager : Singleton<FlyRewardManager>
    {
        const int MAX_FLY_COUNT_ONCE = 10;
        private readonly List<Image> mAllIconImages = new List<Image>();
        private readonly List<bool> mAllIconState = new List<bool>();
        private Vector2 mSize = new Vector2(180, 110);

        private Dictionary<Reward, Transform> mTargetPos = new Dictionary<Reward, Transform>();
        private Transform[] mKeytargetPos;


        private Transform mRoot; //根节点

        //气球根节点
        private Transform mBollonRoot;
        Transform Root
        {
            get
            {
                if (mRoot == null) mRoot = UIManager.Instance.GetLayerRoot(UILayers.RewardLayer);
                return mRoot;
            }
        }

        Dictionary<string, bool> mCanPlayBomb = new Dictionary<string, bool>();


        public void Init(Transform root, Transform bollonRoot)
        {
            mRoot = root;
            mBollonRoot = bollonRoot;
        }

        /// <summary>
        /// 可供外部调用，播放飞奖励动画
        /// </summary>
        /// <param name="fromPos">从哪飞</param>
        /// <param name="endPos">飞到哪</param>
        /// <param name="num">奖励的数额</param>
        /// <param name="iconSpr">奖励的图标</param>
        /// <param name="cb">第一个图标到达目标时的回调</param>
        public void PlayFlyToAnim(Vector3 fromPos, Vector3 endPos, int num, Sprite iconSpr, System.Action<float> cb = null)
        {
            List<Vector3> targetPosList = new List<Vector3>();
            int flyCount = Mathf.Min(num, MAX_FLY_COUNT_ONCE);
            List<int> icon_Index = new List<int>();
            int currentIconCount = mAllIconImages.Count;
            for (int i = 0; i < currentIconCount; i++)
            {
                if (!mAllIconState[i])
                {
                    icon_Index.Add(i);
                    mAllIconState[i] = true;
                }
                if (icon_Index.Count == flyCount)
                    break;
            }

            int oddFlyNeedIconCount = flyCount - icon_Index.Count;
            for (int i = 0; i < oddFlyNeedIconCount; i++)
            {
                mAllIconImages.Add(ResourceUtil.Instantiate<Image>(Root, "Prefabs/UI/FlyIcon"));
                mAllIconState.Add(true);
                icon_Index.Add(mAllIconState.Count - 1);
            }
            for (int i = 0; i < flyCount; i++)
            {
                Image icon = mAllIconImages[icon_Index[i]];
                icon.sprite = iconSpr;

                icon.transform.position = fromPos;
                Vector3 localStart = icon.transform.localPosition;
                targetPosList.Add(new Vector3(localStart.x + Random.Range(-mSize.x, mSize.x), localStart.y + Random.Range(-mSize.y, mSize.y)));
                icon.gameObject.SetActive(true);
            }
            Debug.Log(icon_Index.Count + " FlyAsync");
            FlyAsync(icon_Index, targetPosList, endPos, num, cb);
        }



        Sprite GetFlyIconByType(Reward reward)
        {      
            return UIUtil.GetRewardImage(reward.ToString());
        }
        void SendBeginRefreshTextEvent(Reward reward, float dur, int startNum)
        {
 
            MainController.PlaySound(AudioModel.TokenFly);
            switch (reward)
            {
                case Reward.Gold:
                    EventCenterManager.Send<UpdateGoldEvent>(new UpdateGoldEvent()
                    {
                        BeginValue = startNum,
                        AnimDur = dur,
                    });
                    break;            
                default:
                    break;

            }
        }
        //设置目标点
        public void SetTarget(Reward reward, Transform targetWorldPos)
        {
            mTargetPos[reward] = targetWorldPos;
        }
        //多个不同目标点
        public void SetKeyTarget(Transform[] targetWorldPos)
        {
            mKeytargetPos = targetWorldPos;
        }

        public void FlyTo(Reward reward, int rewardNum, Vector3 startWorldPos)
        {
            // if (mRoot == null) mRoot = UIManager.Instance.GetLayerRoot(UILayers.RewardLayer);
            var startNum = MainController.DataMgr.GetNumByType(reward);
            // mStartNum = MainController.DataMgr.GetNumByType(reward);
            Vector3 targetWorldPos;
            if (reward == Reward.Gold)
            {
                if (mKeytargetPos == null || mKeytargetPos.Length <= 0)
                {
                    LogUtil.Err("目标点不存在！" + reward);
                    return;
                }
                targetWorldPos = mKeytargetPos[Mathf.Clamp(startNum, 0, mKeytargetPos.Length - 1)].position;
            }
            else
            {
                if (!mTargetPos.ContainsKey(reward) || mTargetPos[reward] == null)
                {
                    LogUtil.Err("目标点不存在！" + reward);
                    return;
                }
                targetWorldPos = mTargetPos[reward].position;
            }

            PlayFlyToAnim(startWorldPos, targetWorldPos, rewardNum, GetFlyIconByType(reward), (dur) =>
            {
                SendBeginRefreshTextEvent(reward, dur, startNum);
            });
        }


        private async void FlyAsync(List<int> iconIndex, List<Vector3> spreadTargetLocalPosList, Vector3 targetWorldPos, int addNum, System.Action<float> cb)
        {

            List<int> rewardNumPerIcon = new List<int>();
            int iconCount = iconIndex.Count;
            for (int i = 0; i < iconCount; i++)
            {
                if (i == iconCount - 1)
                    rewardNumPerIcon.Add(addNum);
                else
                {
                    int num = (int)((float)addNum / iconCount);
                    rewardNumPerIcon.Add(num);
                    addNum -= num;
                }
            }
            float spreadTime = 0.2f;
            float timer = 0;
          
            Vector3 startLocalPos = mAllIconImages[iconIndex[0]].transform.localPosition;
            int imageCount = spreadTargetLocalPosList.Count;
            while (timer < spreadTime)
            {
                await new WaitForEndOfFrame();
                timer += Time.deltaTime;
                timer = Mathf.Clamp(timer, 0, spreadTime);
                for (int i = 0; i < imageCount; i++)
                {
                    mAllIconImages[iconIndex[i]].transform.localPosition = Vector3.Lerp(startLocalPos, spreadTargetLocalPosList[i], timer / spreadTime);
                }
            }
            for (int i = 0; i < imageCount; i++)
            {
                spreadTargetLocalPosList[i] = mAllIconImages[iconIndex[i]].transform.position;
            }
            float flyTime = 0.3f;
            float flyInterval = 0.05f;
            int startIndex = 0;
            int endIndex = 0;
            timer = 0;

            while (startIndex <= imageCount - 1)
            {
                await new WaitForEndOfFrame();
                timer += Time.deltaTime;
                if (timer >= (startIndex + 1) * flyInterval)
                    endIndex++;
                endIndex = Mathf.Clamp(endIndex, 0, imageCount - 1);
                for (int i = startIndex; i <= endIndex; i++)
                {
                    float progress = (timer - flyInterval * i) / flyTime;
                    progress = Mathf.Clamp(progress, 0, 1);
                    if (i == startIndex && progress >= 1)
                    {
                        if (startIndex == 0) cb?.Invoke(flyTime + (imageCount - 1) * flyInterval + 0.1f);
                        startIndex++;
                        progress = 1;
                        ShowFlyBomb(targetWorldPos);

                        mAllIconImages[iconIndex[i]].gameObject.SetActive(false);
                        mAllIconState[iconIndex[i]] = false;
                    }
                    mAllIconImages[iconIndex[i]].transform.position = Vector3.Lerp(spreadTargetLocalPosList[i], targetWorldPos, progress);
                }
            }
        }
        public void ShowFlyBomb(Vector3 pos, bool needCheck = true)
        {
            if (needCheck)
            {
                var key = $"{(int)pos.x}:{(int)pos.y}";
                if (!mCanPlayBomb.ContainsKey(key))
                {
                    mCanPlayBomb.Add(key, true);
                    DOVirtual.DelayedCall(0.15f, () =>
                    {
                        mCanPlayBomb.Remove(key);
                    });
                    var bomb = ResourceUtil.Instantiate<GameObject>(Root, "Prefabs/UI/FlyIconBomb");
                    bomb.transform.position = pos;
                }
            }
            else
            {
                var bomb = ResourceUtil.Instantiate<GameObject>(Root, "Prefabs/UI/FlyIconBomb");
                bomb.transform.position = pos;
            }

        }

    }
}
