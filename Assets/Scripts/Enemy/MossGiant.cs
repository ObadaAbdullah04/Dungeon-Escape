using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossGiant : Enemy, IDamageable
{
    public override float GetIdleDuration()
    {
        return 3f;
    }
}