using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//移動パターン8
//停滞してそのまま右上に
public class CEnemy8 : CEnemy
{
    void Update()
    {
        if (Cnt == Wait)
        {
            VX = 3.0f;
            VY = 5.0f;
            BulletDischarge = true;
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
        enemy_obj.AddComponent<CEnemy8>().EnemyStatus = enemy_status;
    }
}
