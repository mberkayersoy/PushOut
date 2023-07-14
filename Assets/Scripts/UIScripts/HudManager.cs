using UnityEngine;
using TMPro;
using DG.Tweening;

public class HudManager : MonoBehaviour
{
    public GameObject popupPrefab;
    private GameObject currentPopUp;
    public Transform popupTransform;
    private Sequence currentPopUpSequence;
    TextMeshProUGUI scoreText;
    public float popUpDuration = 1f;
    public float moveDistance = 5f;

    public void ShowScorePopUp(float score)
    {
        // If a new popup is created before the popup animation complete. Destory old pop-up
        if (currentPopUp != null)
        {
            Destroy(currentPopUp);
        }

        // Create new pop-up
        currentPopUp = Instantiate(popupPrefab, popupTransform.position, Quaternion.identity, popupTransform);
        scoreText = currentPopUp.GetComponentInChildren<TextMeshProUGUI>();
        scoreText.color = SetTextColor(score);
        scoreText.text = "+" + score.ToString();


        currentPopUpSequence = DOTween.Sequence();
        currentPopUpSequence.Append(scoreText.transform.DOMoveY(scoreText.transform.position.y + moveDistance, popUpDuration)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {

        // Increase the scale value as you go up.
        scoreText.transform.DOScale(Vector3.one, popUpDuration);
            }));
        currentPopUpSequence.AppendInterval(popUpDuration);
        currentPopUpSequence.Append(scoreText.transform.DOMoveY(scoreText.transform.position.y - moveDistance, popUpDuration)
            .SetEase(Ease.InQuad)
            .OnStart(() =>
            {
        // Decrease the scale value as you go down.
        scoreText.transform.DOScale(Vector3.zero, popUpDuration);
            })
            .OnComplete(() => Destroy(currentPopUp)));
    }

    // Set text color based on score value.
    public Color SetTextColor(float score)
    {
        if (score <= 200)
        {
            return Color.green;
        }
        else if (score > 200 && score < 2000)
        {
            return new Color(1f, 0.627f, 0f); // Color Orange RGB
        }
        else
        {
            return Color.red;
        };
    }

    //private void DestroyPopUp(GameObject popUp)
    //{
    //    Destroy(popUp);
    //}
}
