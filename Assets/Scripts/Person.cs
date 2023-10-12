using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField]
    Transform headTF, targetTF, bodyTF;
    int shootBallsNum;

    Vector3 shootPos;
    // Start is called before the first frame update
    void Awake()
    {
        
        //Debug.Log(Vector3.Angle(new Vector3(TargetTF.position.x - transform.position.x, 0f, TargetTF.position.z - transform.position.z), TargetTF.position - transform.position));
        //shootBallsNum = FieldManager.Instance.throwBallsCount;
        //StartCoroutine(ThrowBalls(shootBallsNum));
    }

    public void SetLocating(float distance,float personTall)
    {
        transform.position = new Vector3(distance, personTall/2, transform.position.z);
        transform.localScale = new Vector3(transform.localScale.x, personTall, transform.localScale.z);
        shootPos = headTF.position + new Vector3(0, 0.5f, 0f);
    }

    public IEnumerator ThrowBalls(int shootBallsNum,int shootAngle,bool isAirResistant)
    {
        bodyTF.tag = "Untagged";
        headTF.tag = "Untagged";
        for (int i = 0; i < shootBallsNum; i++)
        {
            BallController.Instance._Throw(shootAngle, shootPos, isAirResistant);
            yield return new WaitForEndOfFrame();
        }
        bodyTF.tag = "Ground";
        headTF.tag = "Ground";
    }
}
