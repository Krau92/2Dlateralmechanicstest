using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RangedWeapon : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float safetyTimeout = 3f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        float dir = pm != null && pm.FacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, 0f);
        Destroy(gameObject, safetyTimeout);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if ((groundLayer & (1 << other.gameObject.layer)) != 0)
            Destroy(gameObject);
    }
}
