using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//行動パターン4
//すばやく降りてきて右へ
public class CEnemy4 : CEnemy
{
    void Update()
    {
        if (Cnt == 0)
        {
            VY = -5.0f;
        }
        if (Cnt == 100)
        {
            BulletDischarge = true;
        }
        if (Cnt < 300)
        {
            VX += 2.0f / 100.0f;//右向き加速
            VY += 1.0f / 100.0f;//減速
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
        enemy_obj.AddComponent<CEnemy4>().EnemyStatus = enemy_status;
    }
}
