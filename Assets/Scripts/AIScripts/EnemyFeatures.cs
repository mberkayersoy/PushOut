
public class EnemyFeatures : CharacterFeatures
{
    void Start()
    {
        scale = transform.localScale.x;
    }
    public override void SetScore(float addScore)
    {
        totalScore += addScore;
        //Update the scale value every time the score is updated.
        //Because the scale variable depends on the score variable.
        SetScale(addScore);
    }
}
