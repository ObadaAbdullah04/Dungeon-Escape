using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                AudioManager.Instance.PlayPlayerJump();

                // Trigger jump animation if available
                PlayerAnimation anim = other.GetComponent<PlayerAnimation>();
                if (anim != null)
                {
                    anim.Jump(true);
                }
            }
        }
    }
}
