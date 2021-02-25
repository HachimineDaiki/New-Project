using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CBulletController
{
    protected int Cnt, BulletType, ScriptType, BulletColor;
    protected float Speed, Angle;
    protected CGameManager GameManager;
    public CBulletController(int bullet_type, int bullet_color, int script_type, float speed, float angle)
    {
        Cnt = 0;
        BulletType = bullet_type;
        BulletColor = bullet_color;
        ScriptType = script_type;
        Speed = speed;
        Angle = angle;
        GameManager = GameObject.Find("GameManager").GetComponent<CGameManager>();
    }
    public virtual void Move(Vector3 pos)
    {

    }
}

public class CAimBulletController : CBulletController
{
    public CAimBulletController(int bullet_type, int bullet_color, int script_type) : base(bullet_type, bullet_color, script_type, 3.0f, 0.0f)
    {

    }
    public override void Move(Vector3 pos)
    {
        if (Cnt % 24 == 0)
        {
            GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
            if (PlayerObj != null)
            {
                Vector3 player_pos = PlayerObj.transform.position;
                Angle = Mathf.Atan2(player_pos.y - pos.y, player_pos.x - pos.x);
            }
            else // プレイヤーが見つからなければ下に発射
            {
                Angle = Mathf.PI + (Mathf.PI / 2);
            }
            CSoundPlayer.PlaySound("enemy_shot", true);
            GameManager.BulletFactory[BulletType].CreateBullet(pos, BulletColor, ScriptType, Speed, Angle);
        }
        Cnt++;
    }
}