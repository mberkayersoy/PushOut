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
        // If a new popup is created before the popup animation completes, destroy the old pop-up
        if (currentPopUp != null)
        {
            currentPopUpSequence.Kill();
            Destroy(currentPopUp);
            currentPopUp = null; // Set currentPopUp to null after destroying it
        }

        // Create new pop-up
        currentPopUp = Instantiate(popupPrefab, popupTransform.position, Quaternion.identity, popupTransform);
        scoreText = currentPopUp.GetComponentInChildren<TextMeshProUGUI>();
        scoreText.color = SetTextColor(score);
        scoreText.text = "+" + score.ToString();

        if (scoreText.transform == null) return;

        currentPopUpSequence = DOTween.Sequence();
        currentPopUpSequence.Append(scoreText.transform.DOMoveY(scoreText.transform.position.y + moveDistance, popUpDuration)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {
                if (scoreText.transform == null) return;
                // Increase the scale value as you go up.
                scoreText.transform.DOScale(Vector3.one, popUpDuration);
            }));


        if (scoreText.transform == null) return;

        currentPopUpSequence.AppendInterval(popUpDuration);
        currentPopUpSequence.Append(scoreText.transform.DOMoveY(scoreText.transform.position.y - moveDistance, popUpDuration)
            .SetEase(Ease.InQuad)
            .OnStart(() =>
            {
                if (scoreText.transform == null) return;
                // Decrease the scale value as you go down.
                scoreText.transform.DOScale(Vector3.zero, popUpDuration);
            })
            .OnComplete(() =>
            {
                if (scoreText.transform == null) return;
                currentPopUpSequence.Kill();
                Destroy(currentPopUp);
                currentPopUp = null; // Set currentPopUp to null after destroying it
            }));
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
}
