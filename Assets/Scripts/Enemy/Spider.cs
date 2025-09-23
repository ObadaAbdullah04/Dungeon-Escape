using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy ,IDamageable
{

    public GameObject acidEffectPrefab;

    public override float GetIdleDuration()
    {
        return 1.5f;
    }
    public override void CalculateMovementSpeed()
    {
       
    }
    public override void Movement()
    {
        
    }
    public override void Flip(bool faceRight)
    {
     
    }
    public override void FacePlayer()
    {
    }
    public override void shouldFollowPlayer()
    {
      
    }

    public override void Damage()
    {
        Health--;
        if (Health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public void Attack()
    {
        Instantiate(acidEffectPrefab, transform.position, Quaternion.identity);
    }
}
