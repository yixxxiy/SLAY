using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace XGame
{
    public class ParticleEffectsManager : Singleton<ParticleEffectsManager>
    {
        private GameObject mParticleEffectsMgr;
        GameObject mMoneyEffect;
        GameObject mFireworkEffect;

        ParticleSystem mMoneyEffectParticle;
        List<ParticleSystem> mFireworkEffectParticles = new List<ParticleSystem>();

        public void Init()
        {
            if (mParticleEffectsMgr != null) return;
            mParticleEffectsMgr = ResourceUtil.Instantiate<GameObject>("Prefabs/UI/ParticleEffects");
            GameObject.DontDestroyOnLoad(mParticleEffectsMgr);
            mParticleEffectsMgr.name = "ParticleEffectsManager";
            mParticleEffectsMgr.GetComponent<Canvas>().worldCamera = UIManager.Instance.UiCanvas.GetComponent<Canvas>().worldCamera;

            // mMoneyEffect = mParticleEffectsMgr.transform.Find("PopRewardEffect/Money").gameObject;
            // mFireworkEffect = mParticleEffectsMgr.transform.Find("PopRewardEffect/Firework").gameObject;

            mMoneyEffectParticle = mParticleEffectsMgr.transform.Find<ParticleSystem>("PopRewardEffect/Money");
            mFireworkEffectParticles.Add(mParticleEffectsMgr.transform.Find<ParticleSystem>("PopRewardEffect/Firework/Left/band1"));
            mFireworkEffectParticles.Add(mParticleEffectsMgr.transform.Find<ParticleSystem>("PopRewardEffect/Firework/Left/band2"));
            mFireworkEffectParticles.Add(mParticleEffectsMgr.transform.Find<ParticleSystem>("PopRewardEffect/Firework/Right/band1"));
            mFireworkEffectParticles.Add(mParticleEffectsMgr.transform.Find<ParticleSystem>("PopRewardEffect/Firework/Right/band2"));
        }
        /// <summary>
        /// 播放喷花特效
        /// </summary>
        public void PlayFireworkEffect()
        {
            foreach (var p in mFireworkEffectParticles)
            {
                p.gameObject.SetActive(false);
                p.gameObject.SetActive(true);
                p.Play();
            }
        }
        /// <summary>
        /// 播放钱炸开特效
        /// </summary>
        public void PlayMoneyEffect()
        {
            mMoneyEffectParticle.gameObject.SetActive(false);
            mMoneyEffectParticle.gameObject.SetActive(true);
            mMoneyEffectParticle.Play();
        }


    }

}
