using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CShot : MonoBehaviour
{
    public float Speed = 3.0f;
    public int ShotPower = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float angle = 90.0f;
        transform.position += CUtility.GetDirection1(angle) * Speed * Time.deltaTime;

        // 弾が進行方向を向くようにする
        var angles = transform.localEulerAngles;
        angles.z = angle - 90;
        transform.localEulerAngles = angles;
        // 2 秒後に削除する
        Destroy(gameObject, 2);
    }
}