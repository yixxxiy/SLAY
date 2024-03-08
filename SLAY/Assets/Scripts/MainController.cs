using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
