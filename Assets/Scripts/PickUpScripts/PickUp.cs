using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private float addScore = 100f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterFeatures features))
        {
            Destroy(gameObject);
            features.SetScore(addScore);
        }
    }
}