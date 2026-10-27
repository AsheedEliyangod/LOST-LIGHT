using UnityEngine;
using UnityEngine.UI;

public class LanternEnergyUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LanternSystem lanternSystem;
    [SerializeField] private Image fillImage;

    private void Update()
    {
        if (lanternSystem == null || fillImage == null)
            return;

        float energyPercent =
            lanternSystem.CurrentEnergy /
            lanternSystem.MaxEnergy;

        fillImage.fillAmount =
            Mathf.Clamp01(energyPercent);
    }
}