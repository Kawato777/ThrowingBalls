using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    bool isSend = false;
    bool isUP = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DesroyBall());
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10)
        {
            FieldManager.Instance.BallCount(false);
            Destroy(gameObject);
        }
        //if(transform.position.y > 7.65f)
        //{
          //  isUP = true;
        //}
        
    }

    IEnumerator DesroyBall()
    {
        yield return new WaitForSeconds(7f);
        FieldManager.Instance.BallCount(false);
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
                FieldManager.Instance.BallCount(true);
                isSend = true;
            }
            Destroy(gameObject, 1.0f);
        }

        if(collision.gameObject.CompareTag("Ground"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.isKinematic = false;
            if(isSend == false)
            {
                FieldManager.Instance.BallCount(false);
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
