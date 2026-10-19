
using UnityEngine;
using UnityEngine.InputSystem;

public class EnchantmentFlashlight : MonoBehaviour
{
    [Header("Flashlight")]
    public Transform beamOrigin;
    public Light spotLight;

    [Range(1f, 90f)]
    public float coneAngle = 30f;

    [Header("Enchanted Materials")]
    public Material[] enchantedMaterials;

    [Header("Toggle Settings")]
    public bool startsOn = false;

    private bool isOn;

    [Header("Lightsaber Blade")]
    public Transform lightsaberBlade;

    [Min(0f)]
    public float bladeLength = 1f;

    [Min(0.01f)]
    public float extensionSpeed = 4f;

    public float bladeRadius = 0.025f;
    public float bladeStartOffset = 0.25f;

    private float currentBladeLength = 0f;

    void Start()
    {
        isOn = startsOn;

        currentBladeLength = isOn ? bladeLength : 0f;

        UpdateFlashlight();
        UpdateBladeTransform();
    }

    void Update()
    {
        // PC controls: press F to toggle
        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }

        // Smoothly extend or retract the blade
        float targetLength = isOn ? bladeLength : 0f;

        currentBladeLength = Mathf.MoveTowards(
            currentBladeLength,
            targetLength,
            extensionSpeed * Time.deltaTime
        );

        UpdateBladeTransform();
    }

    void LateUpdate()
    {
        Transform source = beamOrigin != null
            ? beamOrigin
            : transform;

        foreach (Material mat in enchantedMaterials)
        {
            if (mat == null)
                continue;

            // Flashlight world-space position
            mat.SetVector(
                "_Flashlight_Origin",
                source.position
            );

            // Flashlight world-space direction
            mat.SetVector(
                "_Flashlight_Direction",
                source.forward.normalized
            );

            // Flashlight cone angle
            mat.SetFloat(
                "_Flashlight_ConeAngle",
                coneAngle
            );

            // Enable/disable magical reveal and boundary
            mat.SetFloat(
                "_Flashlight_Enabled",
                isOn ? 1f : 0f
            );
        }
    }

    public void ToggleFlashlight()
    {
        isOn = !isOn;
        UpdateFlashlight();
    }

    void UpdateFlashlight()
    {
        // Toggle physical spotlight
        if (spotLight != null)
            spotLight.enabled = isOn;

        // Toggle enchanted materials
        foreach (Material mat in enchantedMaterials)
        {
            if (mat != null)
            {
                mat.SetFloat(
                    "_Flashlight_Enabled",
                    isOn ? 1f : 0f
                );
            }
        }
    }

    void UpdateBladeTransform()
    {
        if (lightsaberBlade == null)
            return;

        // Unity cylinder has a default height of 2 units.
        // Rotate the blade 90 degrees around X in the Inspector.

        lightsaberBlade.localScale = new Vector3(
            bladeRadius,
            currentBladeLength / 2f,
            bladeRadius
        );

        // Keep blade base at the end of the flashlight
        lightsaberBlade.localPosition = new Vector3(
            0f,
            0f,
            bladeStartOffset + currentBladeLength / 2f
        );
    }

    void OnDisable()
    {
        // Turn off spotlight
        if (spotLight != null)
            spotLight.enabled = false;

        // Disable magical effect
        foreach (Material mat in enchantedMaterials)
        {
            if (mat != null)
            {
                mat.SetFloat(
                    "_Flashlight_Enabled",
                    0f
                );
            }
        }
    }
}
