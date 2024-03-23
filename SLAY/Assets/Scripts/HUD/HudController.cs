using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame;

public class HudController : MonoSingleton<HudController>
{
    public float showDuration { get => 3f; }

    public void Init()
    {
        this.RegisterEvent<ShowHudEvent>(ShowHud);
        Show();
    }

    public void Clear()
    {
        this.UnRegisterEvent<ShowHudEvent>();
    }

    void ShowHud(ShowHudEvent e)
    {
        Show();
    }

    void Show()
    {
        XGame.MainController.ShowUI<UI_AutoHide>();
        CancelInvoke("Hide");
        Invoke("Hide", showDuration);
    }

    void Hide()
    {
        XGame.MainController.HideUI<UI_AutoHide>();
    }

    void OnDestroy()
    {
        Clear();
    }
}
