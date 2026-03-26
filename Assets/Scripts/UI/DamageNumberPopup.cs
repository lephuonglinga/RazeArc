using TMPro;
using UnityEngine;

public class DamageNumberPopup : MonoBehaviour
{
    const float DefaultLifetime = 0.75f;
    const float DefaultRiseSpeed = 1.8f;
    const float DefaultHorizontalJitter = 0.35f;
    const float DefaultVerticalOffset = 0.7f;
    const float BaseFontSize = 4.2f;
    const float MinFontSize = 3.6f;
    const float MaxFontSize = 8.8f;
    const float DamageForMaxSize = 150f;

    TextMeshPro textMesh;
    Camera mainCamera;
    Color baseColor;
    float lifetime;
    float elapsed;
    Vector3 worldVelocity;

    public static void Spawn(Vector3 worldPosition, float damageAmount)
    {
        GameObject popupObject = new GameObject("DamageNumberPopup");
        DamageNumberPopup popup = popupObject.AddComponent<DamageNumberPopup>();
        popup.Initialize(worldPosition, damageAmount);
    }

    void Initialize(Vector3 worldPosition, float damageAmount)
    {
        transform.position = worldPosition + Vector3.up * DefaultVerticalOffset;
        lifetime = DefaultLifetime;

        textMesh = gameObject.AddComponent<TextMeshPro>();
        textMesh.text = Mathf.Max(0f, damageAmount).ToString("0");
        textMesh.fontSize = GetFontSizeForDamage(damageAmount);
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = new Color(1f, 0.87f, 0.3f, 1f);
        textMesh.outlineWidth = 0.2f;
        textMesh.outlineColor = new Color(0f, 0f, 0f, 0.9f);

        baseColor = textMesh.color;
        mainCamera = Camera.main;

        float jitterX = Random.Range(-DefaultHorizontalJitter, DefaultHorizontalJitter);
        float jitterZ = Random.Range(-DefaultHorizontalJitter, DefaultHorizontalJitter);
        worldVelocity = new Vector3(jitterX, DefaultRiseSpeed, jitterZ);
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float normalized = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, lifetime));

        transform.position += worldVelocity * Time.deltaTime;

        // Keep text facing the camera for readability.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            Vector3 lookDirection = transform.position - mainCamera.transform.position;
            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        if (textMesh != null)
        {
            Color tint = baseColor;
            tint.a = 1f - normalized;
            textMesh.color = tint;
        }

        if (normalized >= 1f)
        {
            Destroy(gameObject);
        }
    }

    float GetFontSizeForDamage(float damageAmount)
    {
        float safeDamage = Mathf.Max(0f, damageAmount);
        float t = Mathf.Clamp01(safeDamage / Mathf.Max(1f, DamageForMaxSize));
        float boosted = Mathf.Lerp(0f, 1f, Mathf.Sqrt(t));
        float size = Mathf.Lerp(BaseFontSize, MaxFontSize, boosted);
        return Mathf.Clamp(size, MinFontSize, MaxFontSize);
    }
}