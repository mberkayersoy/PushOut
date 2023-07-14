using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class HeadAnimation : MonoBehaviour
{
    TextMeshProUGUI titleText;
    float animationDuration = 1f;
    float oscillationDuration = 1f;
    float scaleFactor = 1f;

    private void Start()
    {
        titleText = GetComponent<TextMeshProUGUI>();
        // Instant scale animation
        titleText.transform.localScale = Vector3.zero;
        titleText.transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack).OnComplete(StartOscillation);
    }

    private void StartOscillation()
    {
        // Idle scale animation
        titleText.transform.DOScale(new Vector3(scaleFactor, scaleFactor, scaleFactor), oscillationDuration).
            SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

}
