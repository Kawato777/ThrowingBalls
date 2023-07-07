using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    bool isSend = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        //Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.CompareTag("Target"))
        {
            if (isSend == false)
            {
                FieldManager.Instance.BallCount(true);
                isSend = true;
            }
            Destroy(gameObject, 1.0f);
        }

        if(collision.gameObject.name == "Ground")
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
    }
}
