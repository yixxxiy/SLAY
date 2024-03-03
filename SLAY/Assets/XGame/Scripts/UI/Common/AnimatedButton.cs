using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using XGame;
using UnityEngine.UI;



public class AnimatedButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //[Serializable]
    //public class ButtonClickedEvent : UnityEvent { }

    //public bool interactable = true;

    //[SerializeField]
    //private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    Button button;
    public UnityAction call;

    public bool isNeedIdleAimn = true;
    //public Material mMat;//LightFlashButtonAim

    override protected void Start()
    {
        base.Start();     
        button = GetComponent<Button>();
        PlayIdleAnim();
       // ClearMat();
    }

  

    void PlayIdleAnim()
    {
        if (!isNeedIdleAimn) return;
       // ClearMat();
        this.transform.DOScale(new Vector3(1.02f, 0.85f, 1f), 0.3f).SetEase(Ease.Linear).SetId("quenceIdel").OnComplete(() =>
        {
            this.transform.DOScale(new Vector3(0.85f, 1.02f, 1f), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                this.transform.DOScale(new Vector3(1.01f, 0.9f, 1f), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    this.transform.DOScale(new Vector3(0.95f, 1.01f, 1f), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                    {              
                        this.transform.DOScale(new Vector3(1.01f, 0.9f, 1f), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            this.transform.DOScale(new Vector3(0.95f, 1.005f, 1f), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                this.transform.DOScale(new Vector3(1.005f, 0.95f, 1f), 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                                {
                                    this.transform.DOScale(new Vector3(1, 1, 1f), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                                    {                                       
                                        DOVirtual.DelayedCall(1f, () =>
                                        {
                                            PlayIdleAnim();
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        });

    }


    //public ButtonClickedEvent onClick
    //{
    //    get { return m_OnClick; }
    //    set { m_OnClick = value; }
    //}

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !button.interactable)
            return;
        Press();
    }


    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !button.interactable)
            return;
        Up();
    }

    public void Press()
    {
        this.transform.DOKill();
        this.transform.localScale = Vector3.one;
        Sequence quence = DOTween.Sequence();
        //quence.Append(transform.DOScale(new Vector3(1.1625f, 0.8f, 1f), 0.05f)).SetEase(Ease.Linear);
        quence.Append(transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.05f)).SetEase(Ease.Linear).OnComplete(() =>
        {
            
        });
        //quence.Append(transform.DOScale(new Vector3(1.05f, 1f, 1f), 0.1f)).SetEase(Ease.Linear);
        //quence.Append(transform.DOScale(Vector3.one, 0.05f)).SetEase(Ease.Linear).OnComplete(() =>
        //{
        //    InvokeOnClickAction();
        //});
    }

    public void Up()
    {
        this.transform.DOKill();
        this.transform.localScale = Vector3.one;
        //Sequence quence = DOTween.Sequence();
        //quence.Append(transform.DOScale(new Vector3(1, 1f, 1f), 0.05f)).SetEase(Ease.Linear).OnComplete(() =>
        //{
        //    PlayIdleAnim();
        //});

    }

    
}
