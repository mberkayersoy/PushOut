using UnityEngine;

public class DeadDetection : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    private void OnCollisionEnter(Collision collision)
    {
        // If AI is dead.
        if (collision.gameObject.TryGetComponent(out EnemyFeatures deadAI))
        {
            deadAI.GetComponent<Animator>().SetTrigger("Dead");
            deadAI.GetComponent<Collider>().isTrigger = true;
            deadAI.GetComponent<Rigidbody>().isKinematic = true;
            deadAI.SetScoreLastPushedPlayer();
            gameManager.UpdateLeaderBoard(deadAI);

        }
        // If player is dead.
        else if (collision.gameObject.TryGetComponent(out PlayerFeatures deadPlayer))
        {
            deadPlayer.SetScoreLastPushedPlayer();
            deadPlayer.GetComponent<Animator>().SetTrigger("Dead");

            gameManager.UpdateLeaderBoard(deadPlayer);
            gameManager.isGameActive = false;
            gameManager.DisplayLeaderBoard();
            return;

        }
        if (gameManager.deadCharacters.Count == gameManager.spawnManager.enemyCount)
        {
            gameManager.isGameActive = false;
            gameManager.DisplayLeaderBoard();

        } 
    }    
}
