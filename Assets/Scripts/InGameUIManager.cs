using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image healthBarFill;
    public TextMeshProUGUI ammoText;
    public Image crosshair;

    [Header("Health Bar Flash Settings")]
    public float lowHealthThreshold = 50f;
    public float maxFlashSpeed = 8f;  // Flash frequency at critical health
    public float minFlashSpeed = 2f;  // Flash frequency at threshold

    [Header("Ammo Flash Settings")]
    public float reserveEmptyFlashSpeed = 2.5f;
    public float weaponEmptyFlashSpeed = 6f;
    public float reloadingFlashSpeed = 3.5f;
    public Color reserveEmptyColor = new Color(1f, 0.9f, 0.35f, 1f);
    public Color weaponEmptyColor = new Color(1f, 0.25f, 0.25f, 1f);
    public Color reloadingColor = new Color(0.55f, 0.85f, 1f, 1f);

    private float currentHealth;
    private float maxHealth;
    private bool isFlashing;
    private float flashTimer;
    private Color originalHealthBarColor;
    private Color originalAmmoTextColor;
    private bool isAmmoFlashing;
    private bool isWeaponCompletelyEmpty;
    private bool isReloading;
    private float ammoFlashTimer;

    private void Start()
    {
        if (healthBarFill != null)
        {
            originalHealthBarColor = healthBarFill.color;
        }

        if (ammoText != null)
        {
            originalAmmoTextColor = Color.white;
            ammoText.color = originalAmmoTextColor;
        }
    }

    private void Update()
    {
        if (isFlashing && healthBarFill != null)
        {
            flashTimer += Time.deltaTime;
            
            // Calculate flash speed based on health (lower = faster flash)
            float healthPercent = currentHealth / maxHealth;
            float flashSpeed = Mathf.Lerp(maxFlashSpeed, minFlashSpeed, healthPercent);
            
            // Create pulsing effect using sine wave (0.5 to 1.0 alpha range)
            float alpha = Mathf.Sin(flashTimer * flashSpeed * Mathf.PI) * 0.5f + 0.5f;
            
            Color flashColor = originalHealthBarColor;
            flashColor.a = alpha;
            healthBarFill.color = flashColor;
        }

        if (isAmmoFlashing && ammoText != null)
        {
            ammoFlashTimer += Time.deltaTime;

            float flashSpeed;
            float alphaMin;
            Color baseColor;

            if (isWeaponCompletelyEmpty)
            {
                flashSpeed = weaponEmptyFlashSpeed;
                alphaMin = 0.2f;
                baseColor = weaponEmptyColor;
            }
            else if (isReloading)
            {
                flashSpeed = reloadingFlashSpeed;
                alphaMin = 0.5f;
                baseColor = reloadingColor;
            }
            else
            {
                flashSpeed = reserveEmptyFlashSpeed;
                alphaMin = 0.45f;
                baseColor = reserveEmptyColor;
            }

            float alpha = Mathf.Sin(ammoFlashTimer * flashSpeed * Mathf.PI) * 0.5f + 0.5f;
            alpha = Mathf.Lerp(alphaMin, 1f, alpha);
            baseColor.a = alpha;
            ammoText.color = baseColor;
        }
    }

    // Hàm để đồng đội (code nhân vật) gọi khi bị bắn trúng
    public void UpdateHealth(float newHealth, float max)
    {
        currentHealth = newHealth;
        maxHealth = Mathf.Max(1f, max);

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        // Check if should flash based on threshold
        if (currentHealth < lowHealthThreshold && currentHealth > 0)
        {
            if (!isFlashing)
            {
                isFlashing = true;
                flashTimer = 0f;
            }
        }
        else
        {
            // Stop flashing and restore normal appearance
            if (isFlashing)
            {
                isFlashing = false;
                if (healthBarFill != null)
                {
                    Color color = originalHealthBarColor;
                    color.a = 1f;
                    healthBarFill.color = color;
                }
            }
        }
    }

    // Hàm để đồng đội gọi khi bắn hoặc thay đạn
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + maxAmmo;
        }

        int loadedAmmo = Mathf.Max(0, currentAmmo);
        int reserveAmmo = Mathf.Max(0, maxAmmo);

        bool reserveEmpty = reserveAmmo <= 0;
        bool weaponEmpty = loadedAmmo <= 0 && reserveAmmo <= 0;
        bool shouldFlash = reserveEmpty || isReloading;

        if (shouldFlash)
        {
            if (!isAmmoFlashing)
            {
                isAmmoFlashing = true;
                ammoFlashTimer = 0f;
            }

            isWeaponCompletelyEmpty = weaponEmpty;
            return;
        }

        if (isAmmoFlashing)
        {
            isAmmoFlashing = false;
            isWeaponCompletelyEmpty = false;
            ammoFlashTimer = 0f;

            if (ammoText != null)
            {
                Color resetColor = originalAmmoTextColor;
                resetColor.a = 1f;
                ammoText.color = resetColor;
            }
        }
    }

    public void SetAmmoReloading(bool value)
    {
        isReloading = value;

        if (!isReloading)
        {
            return;
        }

        if (!isAmmoFlashing)
        {
            isAmmoFlashing = true;
            ammoFlashTimer = 0f;
        }
    }

    public void SetAmmoVisible(bool visible)
    {
        if (ammoText == null)
        {
            return;
        }

        ammoText.gameObject.SetActive(visible);

        // Reset transient states so old flashing does not persist across weapon swaps.
        isReloading = false;
        isAmmoFlashing = false;
        isWeaponCompletelyEmpty = false;
        ammoFlashTimer = 0f;

        Color resetColor = originalAmmoTextColor;
        resetColor.a = 1f;
        ammoText.color = resetColor;
    }
}