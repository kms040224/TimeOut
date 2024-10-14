using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniController : MonoBehaviour
{
    public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

  /*  public void PlayMoveAnimation(bool isMoving)
    {
        anim.SetBool("isMoving", isMoving); // isMoving 변수를 설정하여 이동 애니메이션을 제어
    }*/

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetTrigger("BasicAttack");
        }
    }
}