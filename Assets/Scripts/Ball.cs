using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 startPoint;    // 始点
    public Vector3 endPoint;      // 終点
    public float launchAngle;     // 射出角度（度数法）

    private void Start()
    {
        Launch();
    }

    private void Launch()
    {
        startPoint = transform.position;

        // 射出速度を求める
        // float launchVelocity = Mathf.Sqrt(Mathf.Abs(horizontalDistance * Physics.gravity.y) / Mathf.Sin(2f * launchAngleRad));
        float launchVelocity = 3f;
        // 始点と終点の間の距離を計算(x,z平面)
        Vector2 startPoint_XZ = new Vector2(startPoint.x, startPoint.z);
        Vector2 endPoint_XZ = new Vector2(endPoint.x, endPoint.z);

        float distance = Vector2.Distance(startPoint_XZ, endPoint_XZ);

        float cosine = (endPoint_XZ.x - startPoint_XZ.x) / distance;
        float sine = (endPoint_XZ.y - startPoint_XZ.y) / distance;
        float z = endPoint.z - startPoint.z;

        float launchAngleRad = launchAngle * Mathf.Deg2Rad;

        float velocityX = launchVelocity * Mathf.Cos(launchAngleRad) * cosine;
        float velocityY = launchVelocity * Mathf.Sin(launchAngleRad);
        float velocityZ = launchVelocity * Mathf.Cos(launchAngleRad) * sine;

        // 射出角度をラジアンに変換
        

        // 水平方向と鉛直方向の距離を計算
        float horizontalDistance = distance * Mathf.Cos(launchAngleRad);
        float verticalDistance = distance * Mathf.Sin(launchAngleRad);

        

        // Rigidbodyを取得して初速を設定
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(velocityX, velocityY, velocityZ);

        // 一定時間後にオブジェクトを破壊
        // Destroy(gameObject, distance / launchVelocity);
    }
}
