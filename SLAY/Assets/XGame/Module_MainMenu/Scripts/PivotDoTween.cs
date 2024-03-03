using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PivotDoTween : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        Vector3 endv = this.transform.localEulerAngles + new Vector3(0, 90f, 0);
        this.transform.DORotate(endv, 2.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    // Update is called once per frame
    private void OnDisable()
    {
        
    }
}
