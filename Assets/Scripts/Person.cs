using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField]
    Transform headTF;
    [SerializeField]
    Transform TargetTF;
    [SerializeField]
    float shootAngle = 65f;
    [SerializeField]
    int shootBallsNum = 100;

    Vector3 shootPos;
    // Start is called before the first frame update
    void Start()
    {
        shootPos = headTF.position + new Vector3(0, 0.2f, 0f);
        //Debug.Log(Vector3.Angle(new Vector3(TargetTF.position.x - transform.position.x, 0f, TargetTF.position.z - transform.position.z), TargetTF.position - transform.position));
        FieldManager.Instance.SetThrowBallsCount(shootBallsNum);
        for(int i = 0; i < shootBallsNum; i++)
        {
            BallController.Instance._Throw(shootAngle, shootPos);
        }
    }
}
