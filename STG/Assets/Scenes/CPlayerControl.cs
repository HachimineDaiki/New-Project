using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerControl : MonoBehaviour
{
    Animator _Animator;
    // Start is called before the first frame update
    void Start()
    {
        _Animator = GetComponent<Animator>();
        Sqrt2 = 1.0f / Mathf.Sqrt(2.0f);
    }
    float VX = 0;
    float VY = 0;
    float Sqrt2;
    // Update is called once per frame
    void Update()
    {
        VX = VY = 8.0f * Time.deltaTime;
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
        transform.position += new Vector3(VX, VY, 0);
        _Animator.SetFloat("H", x);
    }
}