using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRevivalPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    float _Time;
    float Y = 0;
    SpriteRenderer _SpriteRenderer;
    void Start()
    {
        _Time = Time.time;
        _SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CalcApha();

        // 一定時間経ったらプレイヤーを復帰させる
        if (Time.time - _Time > 3.0f)
        {
            Destroy(gameObject);
            GameObject player = CGameManager.GetObjectHandle("Player");
            Instantiate(player, transform.position, Quaternion.identity);
        }
        else // プレイヤーを上に移動させる
        {
            Y += 0.008f * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y + Y, transform.position.z);
        }
    }
    int Cnt = 0;
    void CalcApha()
    {
        ++Cnt;
        float alpha = (Mathf.Sin(Cnt * 0.1f) + 1) / 2;
        _SpriteRenderer.color = new Color(1, 1, 1, alpha);
    }
}
