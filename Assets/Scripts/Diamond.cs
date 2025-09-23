using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    public int gems = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Player player = other.GetComponent<Player>();
        if (player!=null)
        {
            player.AddGems(gems);
            Destroy(this.gameObject);
        }
    }
}
