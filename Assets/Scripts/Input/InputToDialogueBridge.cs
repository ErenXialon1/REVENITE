using UnityEngine;
using PixelCrushers.DialogueSystem; // Proximity Selector için gerekli

[RequireComponent(typeof(ProximitySelector))]
public class InputToDialogueBridge : MonoBehaviour
{
    private IInputReader inputReader;
    private ProximitySelector proximitySelector;

    private void Awake()
    {
        // Gerekli component'leri bu GameObject üzerinden al.
        inputReader = GetComponentInParent<IInputReader>();
        proximitySelector = GetComponent<ProximitySelector>();
    }

    private void OnEnable()
    {
        // InputReader'daki InteractEvent'ine abone ol.
        if (inputReader != null)
        {
            inputReader.InteractEvent += OnInteractPressed;
        }
    }

    private void OnDisable()
    {
        // Aboneliði iptal etmeyi unutma.
        if (inputReader != null)
        {
            inputReader.InteractEvent -= OnInteractPressed;
        }
    }

    /// <summary>
    /// Interact tuþuna basýldýðýnda bu fonksiyon tetiklenir.
    /// </summary>
    private void OnInteractPressed()
    {
        // Proximity Selector'a "seçili olan nesneyi kullan" komutunu ver.
        if (proximitySelector != null)
        {
            proximitySelector.UseCurrentSelection();
        }
    }
}