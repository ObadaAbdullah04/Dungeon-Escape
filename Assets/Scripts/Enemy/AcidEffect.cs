using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidEffect : MonoBehaviour, IDamageable
{
    public int Health { get; set; }

    public void Damage()
    {

    }

    private void Start()
    {
        Destroy(gameObject, 3.0f);
    }
    private void Update()
    {
        transform.Translate(Vector3.right * 3 * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Try get IDamageable interface, avoids null check
        if (other.TryGetComponent<IDamageable>(out var hit))
        {
            hit.Damage();
            Destroy(gameObject);
        }
    }
}
