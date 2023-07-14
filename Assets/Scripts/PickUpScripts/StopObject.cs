using UnityEngine;
using DG.Tweening;

public class StopObject : MonoBehaviour
{
    Rigidbody rb;
    private string rotationID;
    //private string positionID;
    public bool isGrounded;
    void Start()
    {
        DOTween.Init();
        rb = GetComponentInParent<Rigidbody>();

        // I gave a unique ID to each dotween objects.
        // In this way, DOTween.Kill does not affect other objects when it runs.
        rotationID = "Rotation" + GetInstanceID().ToString();
        //positionID = "Position" + GetInstanceID().ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
            rb.isKinematic = true;
            RotateObject();
            //MoveObjectYaxis();
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    void RotateObject()
    {
        if (gameObject.transform != null)
        {
            transform.parent.DORotate(new Vector3(0f, 360f, 0f), 2f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetId(rotationID);
        }
    }
    //void MoveObjectYaxis()   
    //{
    //    if (gameObject.transform != null)
    //    {
    //        transform.parent.DOMoveY(transform.parent.position.y + 1f, 1f)
    //        .SetEase(Ease.InOutSine)
    //        .SetLoops(-1, LoopType.Yoyo);
    //    }
    //}

    // This method execute when gameobject will destroy. Prevent the dotween bugs/errors.
    private void OnDestroy()
    {
        DOTween.Kill(rotationID);
    }
}
