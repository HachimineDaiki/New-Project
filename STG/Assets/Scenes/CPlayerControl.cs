using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerControl : MonoBehaviour
{
    Animator _Animator;
    // Start is called before the first frame update
    public GameObject[] ShotObjs;
    void Start()
    {
        _Animator = GetComponent<Animator>();
        Sqrt2 = 1.0f / Mathf.Sqrt(2.0f);
        InputArray["Fire1"] = 0;
        InputArray["Fire2"] = 0;
        InputArray["Fire3"] = 0;
    }
    float VX = 0;
    float VY = 0;
    float Sqrt2;
    Dictionary<string, int> InputArray = new Dictionary<string, int>();
    void CalcInput()
    {
        string[] str = { "Fire1", "Fire2", "Fire3" };
        for (int i = 0; i < str.Length; ++i)
        {
            if (Input.GetButton(str[i]))
            {
                ++InputArray[str[i]];
            }
            else
            {
                InputArray[str[i]] = 0;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        CalcInput();
        VX = VY = 0 < InputArray["Fire3"] ? 2.5f * Time.deltaTime : 7.0f * Time.deltaTime;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x < 0) VX = -VX; else if (x == 0) VX = 0;
        if (y < 0) VY = -VY; else if (y == 0) VY = 0;
        // 斜め方向の速度調節
        if (VX != 0 && VY != 0)
        {
            VX = VX * Sqrt2;
            VY = VY * Sqrt2;
        }
        transform.position = CUtility.ClampPosition(new Vector3(transform.position.x + VX, transform.position.y + VY, 0));

        // Zキー押しっぱなしで6フレームに1度ショットを発射する
        if (0 < InputArray["Fire1"] && InputArray["Fire1"] % 6 == 0)
        {
            CSoundPlayer.PlaySound("cshot", true);
            if (0 < InputArray["Fire3"])//低速移動中なら
            {
                LowerSpeedShot();
            }
            else // 通常移動中なら
            {
                HiSpeedShot();
            }
        }
        _Animator.SetFloat("H", x);
    }
    int[] CShot0Num = { 2, 4 };
    Vector3[] HiSpeedShotOffsetPos =
    {
        new Vector3(0.40f,0.8f),
        new Vector3(0.7f,0.8f),
        new Vector3(0.10f,0.4f),
        new Vector3(1.0f,0.4f),
    };
    // 通常ショット登録
    void HiSpeedShot()
    {
        //	Power < 200 ?0 : 1
        for (int i = 0; i < CShot0Num[1]; i++)
        {
            //SGP.Shot.Add(new CShot(X + CShot0Pos_X[i], Y + CShot0Pos_Y[i], 0.75f, 10));
            Instantiate(ShotObjs[0], transform.position + HiSpeedShotOffsetPos[i], Quaternion.identity);
        }
    }
    Vector3[] LowerSpeedShotOffsetPos =
{
        new Vector3(0.40f,0.8f),
        new Vector3(0.7f,0.8f),
        new Vector3(0.10f,0.4f),
        new Vector3(1.0f,0.4f),
    };
    //低速通常ショット登録
    void LowerSpeedShot()
    {
        for (int i = 0; i < CShot0Num[1]; i++)
        {
            //SGP.Shot.Add(new CShot(X + CShot0Pos_X[i], Y + CShot0Pos_Y[i], 0.75f, 10));
            Instantiate(ShotObjs[0], transform.position + LowerSpeedShotOffsetPos[i], Quaternion.identity);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}