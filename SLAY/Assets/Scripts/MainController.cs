using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XGame;

public class MainController : MonoBehaviour
{
    public GameObject dropItemPrefab;

    void Awake()
    {
        ItemTypes.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveManager.Test();
    }

    /// <summary>
    /// 仅用于测试中显示任务栏
    /// </summary>
    public void onClckShowQuestBar()
    {
        if (!UIManager.Instance.isShowing<UI_QuestBar>())
        {
            Debug.Log("试试看任务列表展示把");
            XGame.MainController.ShowUI<UI_QuestBar>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0f, 1f) < 0.001)
        {
            AddDrop();
        }
    }

    public void AddDrop()
    {
        Debug.Log("AddDrop");
        GameObject newDrop = Instantiate(dropItemPrefab, new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0), Quaternion.identity);
        DropItemScript dropItem = newDrop.GetComponent<DropItemScript>();
        dropItem.SetItem(new Item("木头", "", 1));
    }
}
