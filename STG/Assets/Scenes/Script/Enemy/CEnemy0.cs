using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public struct TagEnemyStatus
{
    public GameObject EnemyObj;
    public float X, Y;
    public int Life, GraphType, WaitTime, BulletPattern;
    public int BulletInterval, BulletType, BulletColor, BulletScriptType;

    public TagEnemyStatus(GameObject enemy_obj, float x = 0.0f, float y = 0.0f,
        int life = 1, int graph_type = 0, int wait_time = 180, int bullet_pattern = 0,
        int bullet_interval = 60, int bullet_type = 0, int bullet_color = 0, int bullet_script_type = 0)
    {
        EnemyObj = enemy_obj;
        X = x;
        Y = y;
        Life = life;
        GraphType = graph_type;
        WaitTime = wait_time;
        BulletPattern = bullet_pattern;
        BulletInterval = bullet_interval;
        BulletType = bullet_type;
        BulletColor = bullet_color;
        BulletScriptType = bullet_script_type;
    }
}

public abstract class CEnemy : MonoBehaviour
{
    protected TagEnemyStatus EnemyStatus;
    protected CBulletController BulletController;
    protected Animator _Animator;
    protected bool BulletDischarge = false; // 弾幕発射フラグ
    protected int Cnt = 0;
    protected int Life = 60;
    protected int Wait = 100;
    protected float VX = 0.0f, VY = 0.0f;
    void Start()
    {
        Life = EnemyStatus.Life;
        Wait = EnemyStatus.WaitTime;
        _Animator = GetComponent<Animator>();
        switch (EnemyStatus.BulletPattern)
        {
            case 0:
                BulletController = new CAimBulletController(
                    EnemyStatus.BulletType, EnemyStatus.BulletColor, EnemyStatus.BulletScriptType);
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shot")
        {
            Life -= collision.GetComponent<CShot>().ShotPower;
            if (Life <= 0)
            {
                Destroy(gameObject);
                GameObject go = CGameManager.GetObjectHandle("EnemyDestroyEffect");
                GameObject ede = Instantiate(go, transform.position, Quaternion.identity);
                CSoundPlayer.PlaySound("enemy_death");
                Destroy(ede, 1.5f);
            }
            Destroy(collision.gameObject);
        }
    }
}

//移動パターン0 
//下がってきて停滞して上がっていく
public class CEnemy0 : CEnemy
{
    void Update()
    {
        if (Cnt == 0)
        {
            VY = -3.0f;
        }
        if (Cnt == 100)
        {
            VY = 0.0f;
            BulletDischarge = true;
        }
        if (Cnt == 100 + Wait)
        {
            VY = 3.0f;
            BulletDischarge = false;
        }

        transform.position += new Vector3(VX, VY, 0.0f) * Time.deltaTime;

        _Animator.SetFloat("H", VX);

        if (BulletController != null && BulletDischarge)
        {
            BulletController.Move(transform.position);
        }

        Cnt++;

        if (CUtility.IsOut(transform.position))
        {
            Destroy(gameObject);
        }
    }
    public static void New(TagEnemyStatus enemy_status)
    {
        GameObject enemy_obj = Instantiate(enemy_status.EnemyObj, new Vector3(enemy_status.X, enemy_status.Y, 0), Quaternion.identity);
        enemy_obj.AddComponent<CEnemy0>().EnemyStatus = enemy_status;
    }
}