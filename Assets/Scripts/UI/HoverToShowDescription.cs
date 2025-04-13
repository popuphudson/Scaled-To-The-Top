using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToShowDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemPickerManager _manager;
    [SerializeField] private int _index;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _manager.SetCurrentHighlighted(_index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _manager.SetCurrentHighlighted(-1);
    }
}
