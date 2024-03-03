using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace XGame
{
    public class MySelector : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
           // ADManager.IsRvBack = true;
        }

        public void OnDeselect(BaseEventData eventData)
        {

        }

    }
}

