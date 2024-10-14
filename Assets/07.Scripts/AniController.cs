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
        anim.SetBool("isMoving", isMoving); // isMoving ������ �����Ͽ� �̵� �ִϸ��̼��� ����
    }*/

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetTrigger("BasicAttack");
        }
    }
}