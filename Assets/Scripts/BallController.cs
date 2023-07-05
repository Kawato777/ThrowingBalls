using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    /*
   * 1.マウスクリック
   * 2.クリックされた座標を取得
   * 3.取得した座標にSleap関数で飛ばす
   */

    //落下地点（クリックした座標）
    Vector3 ThrowPoint = new Vector3(0, 0, 0);

    // 射出角度
    public float ThrowingAngle = 60;

    [SerializeField]
    private Transform m_shootPoint = null;
    [SerializeField]
    private Transform m_target = null;
    [SerializeField]
    private GameObject m_shootObject = null;
    [SerializeField,Range(0.0f,1.0f)]
    private float GosaNum = 0.1f;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            _Throw();
        }
    }

    void _Throw()
    {

        ////左クリック時のマウス座標を取得
        //if (Input.GetMouseButtonDown(0))
        //{
        //    //スクリーン座標からワールド座標に変換してから代入
        //    var MousePos = Input.mousePosition;
        //    var ScreenPos = new Vector3(MousePos.x, MousePos.y, 40f);
        //    ThrowPoint = Camera.main.ScreenToWorldPoint(ScreenPos);
        //    ThrowPoint.y = 1;

            
        //}
        Throwing();
    }
    private void Throwing()
    {
        GameObject ball = Instantiate(m_shootObject, m_shootPoint.position, Quaternion.identity);

        // 射出速度を算出
        Vector3 velocity = CalculateVelocity(ball.transform.position, m_target.transform.position, ThrowingAngle);

        // 誤差を付与
        velocity.x = Random.Range(velocity.x * (1 - GosaNum), velocity.x * (1 + GosaNum));
        velocity.y = Random.Range(velocity.y * (1 - GosaNum), velocity.y * (1 + GosaNum));
        velocity.z = Random.Range(velocity.z * (1 - GosaNum), velocity.z * (1 + GosaNum));

        // 射出
        Rigidbody rid = ball.GetComponent<Rigidbody>();
        rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
    }

    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }
}
