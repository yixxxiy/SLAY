using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame;

public class CommandPanelController : MonoBehaviour
{
    int fingerId;
    bool isTouched = false;
    float touchTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        HandleTouch();
    }

    void HandleTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                var touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                var hits = Physics2D.RaycastAll(touchPos, Vector2.zero);
                if (hits == null || hits.Length == 0) continue;

                foreach (var hit in hits)
                {
                    if (hit.transform.tag != "Pal") continue;
                    Debug.Log(hit.transform.name + " is touched");
                    isTouched = true;
                    fingerId = touch.fingerId;
                    touchTimer = 0f;
                }
            }

            if (isTouched && touch.fingerId == fingerId)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Stationary:
                    case TouchPhase.Moved:
                        touchTimer += Time.deltaTime;
                        if (touchTimer > 1f)
                        {
                            Open();
                            EndTouch();
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        EndTouch();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void EndTouch()
    {
        isTouched = false;
        touchTimer = 0f;
    }

    void Open()
    {
        XGame.MainController.ShowUI<UI_CommandPanel>();
    }
}
