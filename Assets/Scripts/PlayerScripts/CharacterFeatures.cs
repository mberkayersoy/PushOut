using UnityEngine;
using DG.Tweening;

// Since many properties of AI characters and player are the same,
// I defined them under a common abstract class.
// So we got rid of code duplication.
public abstract class CharacterFeatures : MonoBehaviour
{
    // DEFAULT VARIABLES
    protected float totalScore;
    protected float scale; // Scale reference of character
    protected float scaleMultiplier = 0.0008f; // How much does the score variable affect the scale variable.
    private int money;

    //MOVEMENT VARIABLES
    protected string characterName;
    public Vector3 movement;
    public bool isDead;
    public Transform headTransform;

    // GROUND VARIABLES
    protected float groundRadius = 0.8f;
    public LayerMask groundLayers;
    bool isGrounded;
    public Vector3 groundOffset;
    bool edgeCheck;


    // Who was the last one I got pushed by?
    protected CharacterFeatures lastPushedPlayer;

    public int Money { get => money; set => money = value; }

    public void FixedUpdate()
    {
        GroundCheck();
        JitterCheck();
    }
    public abstract void SetScore(float addScore);
    public float GetScore()
    {
        return totalScore;
    }
    public void SetLastPushedPlayer(CharacterFeatures enemy)
    {
        lastPushedPlayer = enemy;
    }
    public void SetScoreLastPushedPlayer()
    {
        if (lastPushedPlayer != null)
        {
            lastPushedPlayer.SetScore(1000f + GetScore());
        }

    }
    public string GetName()
    {
        return characterName;
    }
    public void SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "LocalPlayer";
        }
        characterName = name;
    }
    public float GetScale()
    {
        return scale;
    }
    public void SetScale(float addScale)
    {
        float scaleIncrement = addScale * scaleMultiplier;
        scale += scaleIncrement;
        groundRadius = scale / 5;
        // Create scale animation using DOTween
        GetComponent<Rigidbody>().mass += addScale * 0.008f;
        transform.DOScale(scale, 0.5f);
    }
    public bool GroundCheck()
    {
        gameObject.GetComponent<Animator>().SetBool("isGrounded", isGrounded);
        return isGrounded = Physics.CheckSphere(transform.position + groundOffset, groundRadius, groundLayers
            , QueryTriggerInteraction.Ignore);
    }

    public void JitterCheck()
    {
        if (!isGrounded)
        {
            edgeCheck = Physics.CheckSphere(transform.position + groundOffset, groundRadius * 2.5f, groundLayers
                , QueryTriggerInteraction.Ignore);
            if (edgeCheck)
            {
                //Debug.Log("JitterCheck");
                //GetComponent<Rigidbody>().AddForce(Vector3.down * 30f, ForceMode.VelocityChange);

            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(transform.position + groundOffset, groundRadius);

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position + groundOffset, groundRadius * 2.5f);
    //}

}