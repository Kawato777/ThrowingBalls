using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsimotoSensor : MonoBehaviour
{
    [SerializeField]
    FieldManager fieldManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, fieldManager._Range, 3);
        foreach (Collider hitCollider in hitColliders)
        {
            // if (hitCollider.CompareTag(_tag))
            // {
            // Do something with the item
            if (hitCollider.CompareTag("Ball"))
            {
                Debug.Log("Item collected: " + hitCollider.gameObject.name);
            }
            // }
        }
    }
}
