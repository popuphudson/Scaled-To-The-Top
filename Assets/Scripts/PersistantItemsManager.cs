using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantItemsManager : MonoBehaviour
{
    public static PersistantItemsManager Instance;
    private List<Item> _items = new List<Item>();

    private void Awake() {
        if(Instance) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetItems(List<Item> __items) {
        __items = _items;
    }

    public List<Item> GetItems() {
        return _items;
    }

    public void AddItem(Item __item) {
        _items.Add(__item);
    }
}
