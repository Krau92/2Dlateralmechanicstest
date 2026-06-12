using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
