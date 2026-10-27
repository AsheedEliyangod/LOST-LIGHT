using UnityEngine;

public class LightOrbPickup : MonoBehaviour
{
    [Header("Recharge")]
    [SerializeField] private float rechargeAmount = 30f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        LanternSystem lanternSystem =
            other.GetComponent<LanternSystem>();

        if (lanternSystem == null)
            return;

        // Don't waste the orb if energy is already full.
        if (lanternSystem.CurrentEnergy >= lanternSystem.MaxEnergy)
            return;

        lanternSystem.RechargeEnergy(rechargeAmount);

        Destroy(gameObject);
    }
}