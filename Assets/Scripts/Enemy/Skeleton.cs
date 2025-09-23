using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy, IDamageable
{
    public override float GetIdleDuration()
    {
        return 1.5f;
    }
}
