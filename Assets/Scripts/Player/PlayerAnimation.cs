using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator _anim;
    public Animator _swordAnimator;
    void Start()
    {
        _anim = transform.GetChild(0).GetComponent<Animator>();
        _swordAnimator = transform.GetChild(1).GetComponent<Animator>();
    }

    public void Move(float move)
    {
        _anim.SetFloat("Move", Mathf.Abs(move));
    }
    public void Jump(bool isJumped)
    {
        _anim.SetBool("IsJumping", isJumped);
    }
    public void Attack()
    {
        _anim.SetTrigger("Attack");
        _swordAnimator.SetTrigger("SwordAnimation");
    }
    public void Damage()
    {
        _anim.SetTrigger("Hit");
    }    
    public void Die()
    {
        _anim.SetTrigger("Die");
    }
}
