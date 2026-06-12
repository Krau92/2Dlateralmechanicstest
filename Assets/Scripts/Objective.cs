using UnityEngine;

public class Objective : MonoBehaviour
{
    public GameObject closedDoor;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shuriken") && closedDoor != null)
        {
            Destroy(closedDoor);
            spriteRenderer.color = Color.green;
        }
    }
}
