using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
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

    [Header("Menu Panel")]
    public GameObject MenuUI;
    public TMP_InputField nameInput;
    public TextMeshProUGUI moneyText;
    public int totalMoney = 0;
    public Button startButton;
    public Button quitButton;
    public Button marketButton;
    private const string playerNameKey = "PlayerName";
    private const string playerMoneyKey = "Money";

    [Header("Market Panel")]
    public GameObject MarketUI;

    [Space(5)]

    [Header("Countdown Panel")]
    public GameObject CountdownUI;
    public TextMeshProUGUI countdownText;
    public int countdownValue;

    [Space(5)]

    [Header("Game Panel")]
    public GameObject GameUI;
    public Button pauseButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI remainingTimeText;
    public TextMeshProUGUI livingCharacterText;
    public float livingCharacterCount;
    public SpawnManager spawnManager;
    public float gameTime = 75f;
    float remainingTime;
    public bool isGameActive;
    public List<CharacterFeatures> livingCharacters = new List<CharacterFeatures>();
    public List<CharacterFeatures> deadCharacters = new List<CharacterFeatures>();

    [Header("Pause Panel")]
    public GameObject PauseUI;
    public Button continueButton;
    public Button menuButton;

    [Space(5)]

    [Header("GameEnd Panel")]
    public GameObject GameEndUI;
    public Transform leaderboardContent;
    public GameObject leaderboardRowPrefab;
    public TextMeshProUGUI youareText;
    public TextMeshProUGUI bestScoreText;
    public Button restartButton;
    public TextMeshProUGUI[] leaderBoardRows;

    private void OnEnable()
    {
        LoadPlayerName();
        LoadPlayerMoney();
    }
    private void Start()
    {
        DOTween.Init();
        remainingTime = gameTime;
        SetActivePanel(MenuUI.name);
        startButton.onClick.AddListener(OnClickStartButton);
        pauseButton.onClick.AddListener(OnClickPauseButton);
        continueButton.onClick.AddListener(OnClickPauseButton);
        marketButton.onClick.AddListener(OnClickMarketButton);
        menuButton.onClick.AddListener(RestartScene);
        restartButton.onClick.AddListener(RestartScene);
        DOTween.SetTweensCapacity(500, 500);
        moneyText.text = totalMoney.ToString();
        leaderBoardRows = leaderboardContent.GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (isGameActive)
        {
            DisplayRemainingTime();
            remainingTime -= 1 * Time.deltaTime; 

            if (remainingTime <= 0)
            {
                isGameActive = false;
                DisplayLeaderBoard();
            }
        }
    }

    // Update lists when a character dies.
    public void UpdateLeaderBoard(CharacterFeatures character)
    {
        deadCharacters.Add(character);
        livingCharacters.Remove(character);
    }

    public void UpdateLivingCharacters()
    {
        livingCharacterCount--;
        livingCharacterText.text = livingCharacterCount.ToString();
    }
    // Show the leader board when the game is over.
    public void DisplayLeaderBoard()
    {
        int totalRows = leaderBoardRows.Length;
        int numLivingCharacters = livingCharacters.Count;
        int numDeadCharacters = deadCharacters.Count;

        // More than one character can survive as the time runs out.
        //So when sorting the characters, put them in order according to the score value.
        if (numLivingCharacters >= 2)
        {
            livingCharacters.Sort((x, y) => y.GetScore().CompareTo(x.GetScore()));
            livingCharacters.Reverse();
        }

        for (int i = 0; i < totalRows; i++)
        {
            // First, set the dead characters.
            if (i < numDeadCharacters)
            {
                CharacterFeatures deadCharacter = deadCharacters[i];
                leaderBoardRows[i].text = GetRowText(totalRows - i, deadCharacter.GetName());
                if (CheckIsPlayer(deadCharacter))
                {
                    SetPlayerInfo(i, deadCharacter);
                }
            }
            // Second, set the living characters.
            else if (i >= numDeadCharacters && i < numDeadCharacters + numLivingCharacters)
            {
                CharacterFeatures livingCharacter = livingCharacters[i - numDeadCharacters];
                leaderBoardRows[i].text = GetRowText(totalRows - i, livingCharacter.GetName());
                if (CheckIsPlayer(livingCharacter))
                {
                    SetPlayerInfo(i, livingCharacter);
                    if (leaderBoardRows.Length - i == 1)
                    {
                        livingCharacter.GetComponent<Animator>().SetTrigger("Victory");
                    }
                }
                else
                {
                    leaderBoardRows[i].text = GetRowText(totalRows - i, "-----");
                }
            }
            else
            {
                leaderBoardRows[i].text = GetRowText(totalRows - i, "-----");
            }
        }

        SetActivePanel(GameEndUI.name);
        GameEndUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.75f).SetEase(Ease.OutBounce);
    }
    private string GetRowText(int position, string playerName)
    {
        return position.ToString() + "     " + playerName;
    }

    // Check if the deceased character is a player.
    private bool CheckIsPlayer(CharacterFeatures character)
    {
        string currentPlayerName = nameInput.text;
        return character.GetName().Equals(currentPlayerName) || character.GetName().Equals("LocalPlayer");
    }

    // Set the player's ranking and score.
    private void SetPlayerInfo(int position, CharacterFeatures character)
    {
        leaderBoardRows[position].color = Color.red;
        youareText.text = "You're #" + (leaderBoardRows.Length - position).ToString();
        bestScoreText.text = "BEST SCORE \n" + character.GetScore().ToString();

        totalMoney += (leaderBoardRows.Length - position) switch
        {
            1 => 100,
            2 => 50,
            3 => 25,
            _ => 5,
        };
        SavePlayerMoney();
    }
    public void OnClickStartButton()
    {
        SetActivePanel(CountdownUI.name);
        StartCoroutine(CountDownDisplay());
        spawnManager.gameObject.SetActive(true);
    }
    public void OnClickMarketButton()
    {
        SetActivePanel(MarketUI.name);
    }

    public void OnClickPauseButton()
    {
        if (Time.timeScale == 1)
        {
            SetActivePanel(PauseUI.name);
            // Pause panel animation up
            PauseUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.75f).SetEase(Ease.OutBounce).OnComplete(() =>
            { 
            Time.timeScale = 0;
            });
        }
        else
        {
            Time.timeScale = 1;
            // Pause panel animation down
            PauseUI.GetComponent<RectTransform>().DOAnchorPosY(-PauseUI.GetComponent<RectTransform>().rect.height, 0.75f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                SetActivePanel(GameUI.name);
            });
        }
    }

    
    public void RestartScene()
    {
        // Since the menu button and the restart button use the same function, we control it with a small condition.
        if (isGameActive) 
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            //GameEnd panel animation up
            GameEndUI.GetComponent<RectTransform>().DOAnchorPosY(-PauseUI.GetComponent<RectTransform>().rect.height, 0.75f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
 
    }

    IEnumerator CountDownDisplay()
    {
        for (int i = countdownValue; i > 0; i--)
        {
            CountDownAnimation();
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        CountDownAnimation();
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        isGameActive = true;
        SetActivePanel(GameUI.name);
    }

    public void CountDownAnimation()
    {
        countdownText.transform.DOScale(3f, 0.5f).SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                countdownText.transform.DOScale(0f, 0.5f).SetEase(Ease.OutQuad);
            });
    }

    public void UpdateScoreText(float totalScore)
    {
        scoreText.text = totalScore.ToString();
    }


    public void DisplayRemainingTime()
    {
        float min = remainingTime / 60;
        float second = remainingTime % 60;
        if (second >= 10)
        {
            remainingTimeText.text = ((int)min).ToString() + ":" + ((int)second).ToString();
        }
        else
        {
            remainingTimeText.text = ((int)min).ToString() + ":0" + ((int)second).ToString();
        }

    }
    public void SetActivePanel(string activePanel)
    {
        MenuUI.SetActive(activePanel.Equals(MenuUI.name));
        CountdownUI.SetActive(activePanel.Equals(CountdownUI.name));
        GameUI.SetActive(activePanel.Equals(GameUI.name));
        GameEndUI.SetActive(activePanel.Equals(GameEndUI.name));
        PauseUI.SetActive(activePanel.Equals(PauseUI.name));
        MarketUI.SetActive(activePanel.Equals(MarketUI.name));
    }

    public void SavePlayerName()
    {
        string playerName = nameInput.text;
        PlayerPrefs.SetString(playerNameKey, playerName);

    }

    public void SavePlayerMoney()
    {
        PlayerPrefs.SetInt(playerMoneyKey, totalMoney);
        moneyText.text = totalMoney.ToString();
    }
    public void LoadPlayerMoney()
    {
        if (PlayerPrefs.HasKey(playerMoneyKey))
        {
            int playerMoney = PlayerPrefs.GetInt(playerMoneyKey);
            totalMoney = playerMoney;
            moneyText.text = totalMoney.ToString();
        }
    }

    private void LoadPlayerName()
    {
        if (PlayerPrefs.HasKey(playerNameKey))
        {
            string playerName = PlayerPrefs.GetString(playerNameKey);
            nameInput.text = playerName;
        }

    }
}
