using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallNotAI : MonoBehaviour
{
    bool isSend = false;
    bool isUP = false;
    public float coefficient;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(DesroyBall());
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10)
        {
            FieldManager_NoAI.Instance.BallCount(false);
            Destroy(gameObject);
        }

        //if(transform.position.y > 7.65f)
        //{
          //  isUP = true;
        //}        
    }

    void FixedUpdate()
    {
        // ãÛãCíÔçRÇó^Ç¶ÇÈ
        // rb.AddForce(-coefficient * rb.velocity);
    }

    IEnumerator DesroyBall()
    {
        yield return new WaitForSeconds(7f);
        FieldManager_NoAI.Instance.BallCount(false);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        //Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.CompareTag("Target"))
        {
            if (isSend == false)
            {
                //            FieldManager.Instance.BallCount(isUP);
                FieldManager_NoAI.Instance.BallCount(true);
                isSend = true;
            }
            Destroy(gameObject, 1.0f);
        }

        if(collision.gameObject.CompareTag("Ground"))
        {
            
            rb.isKinematic = true;
            rb.isKinematic = false;
            if(isSend == false)
            {
                FieldManager_NoAI.Instance.BallCount(false);
                isSend = true;
            }
        }

        //if (collision.gameObject.CompareTag("Target2"))
        //{
        //    if (isSend == false)
        //    {
        //        FieldManager.Instance.BallCount(false);
        //        isSend = true;
        //    }
        //    Destroy(gameObject);
        //}
    }
}
