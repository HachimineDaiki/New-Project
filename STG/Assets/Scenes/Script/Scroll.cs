using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    //スクロールスピード
    [SerializeField] float speed = 1;

    void Update()
    {
        //下方向にスクロール
        transform.position -= new Vector3(0, Time.deltaTime * speed);

        //Yが-11まで来れば、21.33まで移動する
        if (transform.position.y <= -11f)
        {
            transform.position = new Vector2(0, 21.33f);
        }
    }
}
