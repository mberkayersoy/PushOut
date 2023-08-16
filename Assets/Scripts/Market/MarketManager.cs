using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public enum ItemType
{
    Food,
    Head,
    Hand,
    Body
}
public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private Button[] dressButtons;
    [SerializeField] private GameObject[] shops;
    [SerializeField] private Transform[] shopsContents;
    public int[] unlockCosts;
    [SerializeField] private Button unlockWithAdButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button unlockRandomButton;
    [SerializeField] public Dictionary<int, MarketItem[]> shopsItemsDic = new Dictionary<int, MarketItem[]>();
    public TextMeshProUGUI unlockPriceText;
    public TextMeshProUGUI totalMoneyText;
    int currentShopIndex;
    GameManager gameManager;
    bool isInitialized;
    void Start()
    {
        SetActiveShop(0);
        gameManager = GameManager.Instance;
        UpdateMoneyText();
        unlockRandomButton.onClick.AddListener(OnClickUnlockRandomButton);
        unlockWithAdButton.onClick.AddListener(OnClickUnclockWithAdButton);
        backButton.onClick.AddListener(OnClickBackButton);


        for (int i = 0; i < shopsContents.Length; i++)
        {
            MarketItem[] items = shopsContents[i].gameObject.GetComponentsInChildren<MarketItem>();

            shopsItemsDic.Add(i, items);
            for (int x = 0; x < items.Length; x++)
            {
                items[x].LoadData();
                if (items[x].IsActive)
                {
                    WearItem(items[x]);
                }
                //Debug.Log("------" + items[x].gameObject.name + "------");
                //Debug.Log(items[x].IsActive + " isActive");
                //Debug.Log(items[x].IsBought + " isBought");
                //Debug.Log("------------------------------");

            }
        }

    }

    public void OnClickBackButton()
    {
        gameManager.SetActivePanel(gameManager.MenuUI.name);
    }

    public void SetActiveShop(int shopIndex)
    {
        for (int i = 0; i < shops.Length; i++)
        {
            if (i == shopIndex)
            {
                shops[i].SetActive(true);
                currentShopIndex = shopIndex;
                unlockPriceText.text = unlockCosts[i].ToString();
            }
            else
            {
                shops[i].SetActive(false);
            }
        }
    }


    public void OnClickUnclockWithAdButton()
    {
        // To do: Watch advert and unlock item.
    }
    
    public void UpdateMoneyText()
    {
        totalMoneyText.text = gameManager.totalMoney.ToString();
        Debug.Log("totalMoney: " + gameManager.totalMoney.ToString());
    }
    public void OnClickUnlockRandomButton()
    {
        if (gameManager.totalMoney >= unlockCosts[currentShopIndex])
        {
            MarketItem[] marketItems = shopsContents[currentShopIndex].GetComponentsInChildren<MarketItem>();
            GetRandomItem(marketItems).BuyItem();
            gameManager.totalMoney -= unlockCosts[currentShopIndex];
            UpdateMoneyText();
            SetCurrentShopCost(currentShopIndex);
            unlockPriceText.text = unlockCosts[currentShopIndex].ToString();
            gameManager.SavePlayerMoney();
            // To do: Unlock Random Item.
        }
    }

    public void SetCurrentShopCost(int currentShopIndex)
    {
        unlockCosts[currentShopIndex] += 2;
        SaveData();
    }

    public void OnClickWearingItem(MarketItem marketItem, ItemType itemType)
    {
        if (shopsItemsDic.TryGetValue((int)itemType, out MarketItem[] selectedMarket))
        {
            foreach (MarketItem item in selectedMarket)
            {
                if (item == marketItem)
                {
                    item.BuyItem();
                    WearItem(item);
                }
                else
                {
                    item.DeActivateItem();
                    TakeOffItem(item);
                }
            }
        }

        SaveData();
    }

    public void WearItem(MarketItem item)
    {
        switch (item.ItemType)
        {
            case ItemType.Food:
                break;
            case ItemType.Head:
                item.itemPrefab.SetActive(true);
                break;
            case ItemType.Hand:
                break;
            case ItemType.Body:
                break;
            default:
                break;
        }

    }
    public void TakeOffItem(MarketItem item)
    {
        switch (item.ItemType)
        {
            case ItemType.Food:
                break;
            case ItemType.Head:
                item.itemPrefab.SetActive(false);
                break;
            case ItemType.Hand:
                break;
            case ItemType.Body:
                break;
            default:
                break;
        }
    }

    public List<MarketItem> GetItemsForGame()
    {
        List<MarketItem> currentItems = new List<MarketItem>();
        for (int i = 0; i < shops.Length; i++)
        {
            if (shopsItemsDic.TryGetValue(i, out MarketItem[] selectedMarket))
            {
                foreach (MarketItem item in selectedMarket)
                {
                    if (item.IsActive)
                    {
                        currentItems.Add(item);
                    }
                }
            }
        }
        return currentItems;
    }

    // Buying Random Item.
    public MarketItem GetRandomItem(MarketItem[] marketItems)
    {
        List<MarketItem> availableItems = new List<MarketItem>();

        foreach (MarketItem item in marketItems)
        {
            if (!item.IsBought)
            {
                availableItems.Add(item);
            }
        }

        return availableItems[Random.Range(0, availableItems.Count)];
    }

    void SaveData()
    {
        MarketData marketData = new MarketData();
        marketData.unlockCosts = unlockCosts;
        marketData.shopsItemsDic = shopsItemsDic;

        string json = JsonUtility.ToJson(marketData);
        PlayerPrefs.SetString($"{gameObject.name}_Data", json);
        PlayerPrefs.Save();
    }
    void LoadData()
    {
        string json = PlayerPrefs.GetString($"{gameObject.name}_Data", null);

        if (!string.IsNullOrEmpty(json))
        {
            MarketData marketData = JsonUtility.FromJson<MarketData>(json);

            unlockCosts = marketData.unlockCosts;
            shopsItemsDic = marketData.shopsItemsDic;
            isInitialized = marketData.isInitialized;
        }
        else
        {
            MarketData marketData = new MarketData();
            isInitialized = true;
            marketData.isInitialized = true;
            PlayerPrefs.SetString($"{gameObject.name}_Data", json);
            PlayerPrefs.Save();
        }
    }
    private void OnEnable()
    {
        LoadData();
    }
}

public class MarketData
{
    public int[] unlockCosts;
    public Dictionary<int, MarketItem[]> shopsItemsDic = new Dictionary<int, MarketItem[]>();
    public bool isInitialized;
}
