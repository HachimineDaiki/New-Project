using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//移動パターン1
//下がってきて停滞して左下に行く
public class CEnemy1 : CEnemy
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
            VX = -1.0f;
            VY = 3.0f;
        }

        transform.position += new Vector3(VX, VY, 0.0f) * Time.deltaTime;

        if (BulletController != null && BulletDischarge)
        {
            BulletController.Move(transform.position);
        }

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
        enemy_obj.AddComponent<CEnemy1>().EnemyStatus = enemy_status;
    }
}