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


    //[SerializeField]
    //private Transform m_shootPoint = null;
    //[SerializeField]
    //private Transform m_target = null;
    //[SerializeField]
    //private GameObject m_shootObject = null;
    //[SerializeField]
    //private float angle = 60.0f;


    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && m_target != null)
    //    {
    //        Shoot(m_target.position);
    //    }
    //}

    ////private void Shoot(Vector3 i_targetPosition)
    ////{
    ////    // Todo: 目標地点にいい感じで山なりを描いて飛んでいく力を計算して渡すのだ！
    ////    Vector3 force = Vector3.zero;

    ////    if (m_shootObject == null)
    ////    {
    ////        throw new System.NullReferenceException("m_shootObject");
    ////    }

    ////    if (m_shootPoint == null)
    ////    {
    ////        throw new System.NullReferenceException("m_shootPoint");
    ////    }

    ////    var obj = Instantiate<GameObject>(m_shootObject, m_shootPoint.position, Quaternion.identity);
    ////    var rigidbody = obj.AddComponent<Rigidbody>();
    ////    rigidbody.AddForce(force, ForceMode.Impulse);
    ////}

    //private void Shoot(Vector3 i_targetPosition)
    //{
    //    // とりあえず適当に60度でかっ飛ばすとするよ！
    //    ShootFixedAngle(i_targetPosition, angle);
    //}

    //private void ShootFixedAngle(Vector3 i_targetPosition, float i_angle)
    //{
    //    float speedVec = ComputeVectorFromAngle(i_targetPosition, i_angle);
    //    if (speedVec <= 0.0f)
    //    {
    //        // その位置に着地させることは不可能のようだ！
    //        Debug.LogWarning("!!");
    //        return;
    //    }

    //    Vector3 vec = ConvertVectorToVector3(speedVec, i_angle, i_targetPosition);
    //    InstantiateShootObject(vec);
    //}

    //private float ComputeVectorFromAngle(Vector3 i_targetPosition, float i_angle)
    //{
    //    // xz平面の距離を計算。
    //    Vector2 startPos = new Vector2(m_shootPoint.transform.position.x, m_shootPoint.transform.position.z);
    //    Vector2 targetPos = new Vector2(i_targetPosition.x, i_targetPosition.z);
    //    float distance = Vector2.Distance(targetPos, startPos);

    //    float x = distance;
    //    float g = Physics.gravity.y;
    //    float y0 = m_shootPoint.transform.position.y;
    //    float y = i_targetPosition.y;

    //    // Mathf.Cos()、Mathf.Tan()に渡す値の単位はラジアンだ。角度のまま渡してはいけないぞ！
    //    float rad = i_angle * Mathf.Deg2Rad;

    //    float cos = Mathf.Cos(rad);
    //    float tan = Mathf.Tan(rad);

    //    float v0Square = g * x * x / (2 * cos * cos * (y - y0 - x * tan));

    //    // 負数を平方根計算すると虚数になってしまう。
    //    // 虚数はfloatでは表現できない。
    //    // こういう場合はこれ以上の計算は打ち切ろう。
    //    if (v0Square <= 0.0f)
    //    {
    //        return 0.0f;
    //    }

    //    float v0 = Mathf.Sqrt(v0Square);
    //    return v0;
    //}

    //private Vector3 ConvertVectorToVector3(float i_v0, float i_angle, Vector3 i_targetPosition)
    //{
    //    Vector3 startPos = m_shootPoint.transform.position;
    //    Vector3 targetPos = i_targetPosition;
    //    startPos.y = 0.0f;
    //    targetPos.y = 0.0f;

    //    Vector3 dir = (targetPos - startPos).normalized;
    //    Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
    //    Vector3 vec = i_v0 * Vector3.right;

    //    vec = yawRot * Quaternion.AngleAxis(i_angle, Vector3.forward) * vec;

    //    return vec;
    //}

    //private void InstantiateShootObject(Vector3 i_shootVector)
    //{
    //    if (m_shootObject == null)
    //    {
    //        throw new System.NullReferenceException("m_shootObject");
    //    }

    //    if (m_shootPoint == null)
    //    {
    //        throw new System.NullReferenceException("m_shootPoint");
    //    }

    //    var obj = Instantiate<GameObject>(m_shootObject, m_shootPoint.position, Quaternion.identity);
    //    var rigidbody = obj.AddComponent<Rigidbody>();

    //    // 速さベクトルのままAddForce()を渡してはいけないぞ。力(速さ×重さ)に変換するんだ
    //    Vector3 force = i_shootVector * rigidbody.mass;

    //    rigidbody.AddForce(force, ForceMode.Impulse);
    //}
}
