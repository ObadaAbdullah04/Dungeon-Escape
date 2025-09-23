using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool _canDamage = true;
    [SerializeField] private float DamageCoolDown = 1f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable hit = other.GetComponent<IDamageable>();

        if (hit!=null)
        {
            if (_canDamage)
            {
                hit.Damage();
                StartCoroutine(DamageCoolDownCoroutine());
            }
        }
    }
    IEnumerator DamageCoolDownCoroutine()
    {
        _canDamage = false;
        yield return new WaitForSeconds(DamageCoolDown);
        _canDamage = true;
    }
}
