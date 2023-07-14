using System.Collections;
using System.Collections.Generic;
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
    protected float scaleMultiplier = 0.001f; // How much does the score variable affect the scale variable.

    //MOVEMENT VARIABLES
    protected string characterName;
    public Vector3 movement;
    public bool isDead;

    // GROUND VARIABLES
    protected float groundRadius = 2f;
    public LayerMask groundLayers;
    bool isGrounded;
    protected Vector3 groundOffset;

    // Who was the last one I got pushed by?
    protected CharacterFeatures lastPushedPlayer;

    public void FixedUpdate()
    {
        GroundCheck();   
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

}