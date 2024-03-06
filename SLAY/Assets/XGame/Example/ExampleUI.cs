using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame;
public class ExampleUI : MonoBehaviour
{
    public Transform FlyTarget;
    // Start is called before the first frame update

    void Start()
    {
       
        Wait();
    }
    async void Wait()
    {
        await new WaitUntil(() => UIManager.Instance.UiCanvas != null);
        await new WaitForEndOfFrame();
    }
    void OnDestroy()
    {
       
    }

   

    public void ShowSetting()
    {
      
    }
    public void ShowLoading()
    {
       // MainController.ShowUI<UI_Loading>();
    }


    public void ShowTips()
    {
       // MainController.ShowTips("这是一条提示");
    }
    public void ShowEffect()
    {
#if XFRAME_PARTICLE_EFFECTS
        if (Random.Range(0, 2) == 0) ParticleEffectsManager.Instance.PlayFireworkEffect();
        else ParticleEffectsManager.Instance.PlayMoneyEffect();
#endif
    }
    public void ShowGuide()
    {
        RectTransform rect = FlyTarget as RectTransform;
        
    }


    // Update is called once per frame
    void Update()
    {

    }
}
