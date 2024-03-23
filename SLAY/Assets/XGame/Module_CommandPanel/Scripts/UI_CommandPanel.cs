using UnityEngine.UI;
using UnityEngine;

namespace XGame
{
    public class UI_CommandPanel : UIView
    {
        public override UILayers Layer
        {
            get
            {
                return UILayers.PopupLayer;
            }
        }
        public override void OnHide()
        {
        }

        public override void OnInit()
        {
            transform.Find<Button>("RadialMenu/Active Components/Elements/ElementA/Button").AddClickEvent(() => { Debug.Log("Inspect clicked"); CloseSelf(); });
            transform.Find<Button>("RadialMenu/Active Components/Elements/ElementB/Button").AddClickEvent(() => { Debug.Log("Follow clicked"); CloseSelf(); });
            transform.Find<Button>("RadialMenu/Active Components/Elements/ElementC/Button").AddClickEvent(() => { Debug.Log("Work clicked"); CloseSelf(); });
            transform.Find<Button>("RadialMenu/Active Components/Elements/ElementE/Button").AddClickEvent(() => { Debug.Log("Cancel clicked"); CloseSelf(); });
        }

        public override void OnShow(object obj)
        {
        }
    }
}
