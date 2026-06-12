using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Melee"))
        {
            Destroy(gameObject);
        }
    }
}
