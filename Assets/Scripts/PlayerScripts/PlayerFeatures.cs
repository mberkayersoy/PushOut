public class PlayerFeatures : CharacterFeatures
{
    private HudManager hudManager;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        hudManager = GetComponent<HudManager>();
        scale = transform.localScale.x;
    }

    public override void SetScore(float addScore)
    {
        totalScore += addScore;
        //Update the scale value every time the score is updated.
        //Because the scale variable depends on the score variable.
        SetScale(addScore);
        gameManager.UpdateScoreText(totalScore);
        hudManager.ShowScorePopUp(addScore);
    }

}
