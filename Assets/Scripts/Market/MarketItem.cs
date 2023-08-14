using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    [SerializeField] public GameObject itemPrefab;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Button button;
    [SerializeField] private bool isBought;
    [SerializeField] private bool isActive;
    [SerializeField] public ItemType ItemType;

    public bool IsBought { get => isBought; set => isBought = value; }
    public bool IsActive { get => isActive; set => isActive = value; }


    void Start()
    {
        button = GetComponent<Button>();
        GetComponent<Image>().sprite = itemSprite;

        button.onClick.AddListener(OnClickItem);

        if (isBought)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }

    }

    public void ActivateItem()
    {
        IsBought = true;
        button.interactable = true;
        SaveData();
    }

    public void DeActivateItem()
    {
        IsActive = false;
        SaveData();
    }

    public void OnClickItem()
    {
        // To do: wear the item.
        IsActive = true;
        MarketManager.Instance.OnClickWearingItem(this, ItemType);
        SaveData();
    }

    public void LoadData()
    {
        string json = PlayerPrefs.GetString($"{gameObject.name}_Data", null);

        if (!string.IsNullOrEmpty(json))
        {
            MarketItemData marketItemData = JsonUtility.FromJson<MarketItemData>(json);
            isActive = marketItemData.isActive;
            isBought = marketItemData.isBought;
        }
    }

    void SaveData()
    {
        MarketItemData marketItemData = new MarketItemData();
        marketItemData.isActive = isActive;
        marketItemData.isBought = isBought;

        string json = JsonUtility.ToJson(marketItemData);
        PlayerPrefs.SetString($"{gameObject.name}_Data", json);
        PlayerPrefs.Save();
    }
}

public class MarketItemData
{
    public bool isBought;
    public bool isActive;
}
