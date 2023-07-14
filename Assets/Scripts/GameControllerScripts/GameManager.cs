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
    public Button startButton;
    public Button quitButton;

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

    private void Start()
    {
        DOTween.Init();
        remainingTime = gameTime;
        SetActivePanel(MenuUI.name);
        startButton.onClick.AddListener(OnClickStartButton);
        pauseButton.onClick.AddListener(OnClickPauseButton);
        continueButton.onClick.AddListener(OnClickPauseButton);
        menuButton.onClick.AddListener(RestarScene);
        restartButton.onClick.AddListener(RestarScene);
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
                InstantiateDeadsRow();
                InstantiateLivingsRow();
                DisplayLeaderBoard();
            }
        }
    }

    public void UpdateDeadList(CharacterFeatures character)
    {
        deadCharacters.Add(character);
        livingCharacters.Remove(character);
    }

    public void InstantiateDeadsRow()
    {
        int rowCounter = spawnManager.enemyCount + 1;
        foreach (CharacterFeatures character in deadCharacters)
        {
            GameObject row = Instantiate(leaderboardRowPrefab, leaderboardContent);
            row.GetComponentInChildren<TextMeshProUGUI>().text = rowCounter.ToString() + "     " + character.GetName();


            if (character.GetName().Equals(nameInput.text) || character.GetName().Equals("LocalPlayer"))
            {
                youareText.text = "You're #" + rowCounter.ToString();
                bestScoreText.text = "BEST SCORE \n" + character.GetScore().ToString();
            }

            rowCounter--;
        }

    }

    public void InstantiateLivingsRow()
    {
        int aliveCharacter = (spawnManager.enemyCount + 1) - deadCharacters.Count;
        livingCharacters.Sort((x, y) => y.GetScore().CompareTo(x.GetScore()));

        for (int i = aliveCharacter; i > 0; i--)
        {
            GameObject row = Instantiate(leaderboardRowPrefab, leaderboardContent);
            row.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString() + "     " + livingCharacters[i - 1].GetName();

            if (livingCharacters[i - 1].GetName().Equals(nameInput.text) || livingCharacters[i - 1].GetName().Equals("LocalPlayer"))
            {
                youareText.text = "You're #" + i.ToString();
                Debug.Log("LeaderBoardLivings :" + livingCharacters[i - 1].GetScore().ToString());
                bestScoreText.text = "BEST SCORE \n" + livingCharacters[i - 1].GetScore().ToString();
            }
        }
    }
    public void DisplayLeaderBoard()
    {
        SetActivePanel(GameEndUI.name);
        GameEndUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.75f).SetEase(Ease.OutBounce);
    }
    public void OnClickStartButton()
    {
        SetActivePanel(CountdownUI.name);
        StartCoroutine(CountDownDisplay());
        spawnManager.gameObject.SetActive(true);
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

    
    public void RestarScene()
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
    }
}
