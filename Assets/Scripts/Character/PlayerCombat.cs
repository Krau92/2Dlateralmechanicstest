
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    #region Serialized Fields

    [Header("Weapons")]
    [SerializeField] private Transform weaponSpawnPoint;
    [SerializeField] private GameObject meleePrefab;
    [SerializeField] private GameObject rangedPrefab;

    [Header("Cooldowns")]
    [SerializeField] private float meleeCooldown = 0.3f;
    [SerializeField] private float rangedCooldown = 0.5f;

    [Header("Attack Slow")]
    [SerializeField] private float attackFallGravityMultiplier = 0.1f;
    [SerializeField] private float attackHorizontalDrag = 40f;
    [SerializeField] private float attackVerticalDrag = 50f;

    #endregion

    #region Private Fields

    private float meleeCooldownTimer;
    private float rangedCooldownTimer;

    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        if (weaponSpawnPoint == null)
            weaponSpawnPoint = transform;

        if (meleePrefab == null)
            Debug.LogWarning("meleePrefab is not assigned", this);
        if (rangedPrefab == null)
            Debug.LogWarning("rangedPrefab is not assigned", this);
    }

    #endregion

    #region Controller Callbacks

    public void UpdateTimers(float dt)
    {
        if (meleeCooldownTimer > 0f)
            meleeCooldownTimer -= dt;

        if (rangedCooldownTimer > 0f)
            rangedCooldownTimer -= dt;
    }

    public void OnAttack(MovementState movementState)
    {
        if (meleeCooldownTimer <= 0f && meleePrefab != null && !movementState.IsDashing)
        {
            GameObject melee = Instantiate(meleePrefab, weaponSpawnPoint.position, transform.rotation, transform);
            melee.transform.localPosition = weaponSpawnPoint.localPosition;
            meleeCooldownTimer = meleeCooldown;
        }
    }

    public void OnShoot(MovementState movementState)
    {
        if (rangedCooldownTimer <= 0f && rangedPrefab != null && !movementState.IsDashing)
        {
            Instantiate(rangedPrefab, weaponSpawnPoint.position, transform.rotation);
            rangedCooldownTimer = rangedCooldown;
        }
    }

    public CombatState GetState()
    {
        return new CombatState(
            meleeCooldownTimer > 0f || rangedCooldownTimer > 0f,
            attackHorizontalDrag,
            attackVerticalDrag,
            attackFallGravityMultiplier
        );
    }

    #endregion
}
