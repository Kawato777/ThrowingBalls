using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField]
    Transform headTF;
    [SerializeField]
    Transform TargetTF;
    [SerializeField,Range(65,89)]
    int shootAngle = 65;
    int shootBallsNum;

    Vector3 shootPos;
    // Start is called before the first frame update
    void Awake()
    {
        shootPos = headTF.position + new Vector3(0, 0.2f, 0f);
        //Debug.Log(Vector3.Angle(new Vector3(TargetTF.position.x - transform.position.x, 0f, TargetTF.position.z - transform.position.z), TargetTF.position - transform.position));
        //shootBallsNum = FieldManager.Instance.throwBallsCount;
        //StartCoroutine(ThrowBalls(shootBallsNum));
    }

    public IEnumerator ThrowBalls(int shootBallsNum)
    {
        for (int i = 0; i < shootBallsNum; i++)
        {
            BallController.Instance._Throw(shootAngle, shootPos);
            yield return new WaitForEndOfFrame();
        }
    }
}
