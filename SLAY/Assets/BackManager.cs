using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame;
public class BackManager : MonoBehaviour
{
 
    public TestItem mItem;
    List<TestItem> mItems = new List<TestItem>();
    const int BackCount = 10;
    private void OnEnable()
    {
        mItems.Add(mItem);
        for(int i = 0; i < BackCount; i++) 
        {
            mItems.Add(Instantiate(mItem.gameObject, mItem.transform.parent).GetComponent<TestItem>());
        }

        foreach(var item in mItems)
        {
            item.Init();
        }
    }
}
