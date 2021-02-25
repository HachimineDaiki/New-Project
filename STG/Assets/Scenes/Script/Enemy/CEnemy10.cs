using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//移動パターン10
//下がってきてウロウロして上がっていく
public class CEnemy10 : CEnemy
{
    float Speed = 0.0f;
    float Angle = 0.0f;
    CGameManager GameManager;
    void Start()
    {
        _Animator = GetComponent<Animator>();
        Pos = transform.position;
        GameManager = GameObject.Find("GameManager").GetComponent<CGameManager>();
    }
    Vector3 Pos = Vector3.zero;
    void Update()
    {
        int t = Cnt;
        if (t >= 40)
        {
            if (t % 60 == 0)
            {
                Vector3 player_pos = GameObject.FindGameObjectWithTag("Player").transform.position;
                float bullet_angle = Mathf.Atan2(player_pos.y - transform.position.y, player_pos.x - transform.position.x);
                CSoundPlayer.PlaySound("enemy_shot", true);
                GameManager.BulletFactory[EnemyStatus.BulletType].CreateBullet(transform.position, EnemyStatus.BulletColor, EnemyStatus.BulletScriptType, 3.0f, bullet_angle);

                int r = Mathf.Cos(Angle) < 0 ? 0 : 1;
                Speed = 6 + CUtility.Rang(2);
                Angle = CUtility.Rang(Mathf.PI / 4.0f) + Mathf.PI * r;
            }
            Speed *= 0.95f;
        }
        VX = Mathf.Cos(Angle) * Speed * Time.deltaTime;
        Pos.x += VX;
        Pos.y += Mathf.Sin(Angle) * Speed * Time.deltaTime;

        transform.position = new Vector3(Pos.x, Pos.y, 0.0f);
        _Animator.SetFloat("H", VX);

        Cnt++;

        if (CUtility.IsOut(transform.position))
        {
            Destroy(gameObject);
        }
    }
    public static void New(TagEnemyStatus enemy_status)
    {
        GameObject enemy_obj = Instantiate(enemy_status.EnemyObj, new Vector3(enemy_status.X, enemy_status.Y, 0), Quaternion.identity);
        enemy_obj.AddComponent<CEnemy10>().EnemyStatus = enemy_status;
    }
}
