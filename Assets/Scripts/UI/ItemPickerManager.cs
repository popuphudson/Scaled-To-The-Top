using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPickerManager : MonoBehaviour
{
    [SerializeField] private Sound _selectSound;
    [SerializeField] private GameObject[] _itemButtons;
    [SerializeField] private Sprite[] _itemSprites;
    [SerializeField] private GameObject _startGameButton;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image[] _shownItemSprites;
    [SerializeField] private GameObject _backButton;
    private List<Item> _allItems = new List<Item>();
    private Item[] _rolledItems = new Item[3];
    private int _itemsLeftToGive = 4;
    private int _currentHighlighted;
    private void Start() {
        PersistantItemsManager.Instance.SetItems(new List<Item>());
        _startGameButton.SetActive(false);
        _allItems.Add(new DelayedHitItem());
        _allItems.Add(new FlameFuryItem());
        _allItems.Add(new ReflectiveCoatingItem());
        _allItems.Add(new MissileLauncherItem());
        _allItems.Add(new GamblingDiceItem());
        _allItems.Add(new ElectricAOEItem());
        _allItems.Add(new StandingGroundItem());
        _allItems.Add(new PrimaryLongRangeItem());
        _allItems.Add(new SecondaryLongRangeItem());
        _allItems.Add(new LifeStealItem());
        _currentHighlighted = -1;
        _itemsLeftToGive = 4;
        RollItems();
    }

    public void RollItems() {
        _itemsLeftToGive--;
        if(_itemsLeftToGive < 0) {
            _rolledItems = null;
            _startGameButton.SetActive(true);
            UpdateShownItems();
            return;
        }
        List<Item> allUnRolledItems = new List<Item>(_allItems);
        foreach(Item item in PersistantItemsManager.Instance.GetItems()) {
            allUnRolledItems.Remove(item);
        }
        for(int i=0; i<3; i++) {
            _rolledItems[i] = allUnRolledItems[Random.Range(0, allUnRolledItems.Count)];
            allUnRolledItems.Remove(_rolledItems[i]);
        }
        UpdateShownItems();
    }

    public void PickItem(int __i) {
        if(_rolledItems == null) return;
        AudioManager.Instance.PlaySoundVaried(_selectSound);
        _backButton.SetActive(false);
        PersistantItemsManager.Instance.AddItem(_rolledItems[__i]);
        _shownItemSprites[3-_itemsLeftToGive].sprite = _itemSprites[_allItems.IndexOf(_rolledItems[__i])];
        _shownItemSprites[3-_itemsLeftToGive].gameObject.SetActive(true);
        RollItems();
    }

    public void UpdateShownItems() {
        SetCurrentHighlighted(_currentHighlighted);
        for(int i=0; i<3; i++) {
            if(_rolledItems == null) {
                _itemButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                _itemButtons[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            } else {
                _itemButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = _itemSprites[_allItems.IndexOf(_rolledItems[i])];
                _itemButtons[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _rolledItems[i].GiveName();
            }
        }
    }

    public void SetCurrentHighlighted(int __i) {
        if(_rolledItems == null) return;
        _currentHighlighted = __i;
        if(__i == -1) {
            _descriptionText.text = "";
            return;
        }
        _descriptionText.text = _rolledItems[__i].GiveDescription();
    }
}
