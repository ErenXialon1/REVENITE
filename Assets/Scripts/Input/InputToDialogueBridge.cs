using UnityEngine;
using PixelCrushers.DialogueSystem; // Proximity Selector i�in gerekli

[RequireComponent(typeof(ProximitySelector))]
public class InputToDialogueBridge : MonoBehaviour
{
    private IInputReader inputReader;
    private ProximitySelector proximitySelector;

    private void Awake()
    {
        // Gerekli component'leri bu GameObject �zerinden al.
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
        // Aboneli�i iptal etmeyi unutma.
        if (inputReader != null)
        {
            inputReader.InteractEvent -= OnInteractPressed;
        }
    }

    /// <summary>
    /// Interact tu�una bas�ld���nda bu fonksiyon tetiklenir.
    /// </summary>
    private void OnInteractPressed()
    {
        // Proximity Selector'a "se�ili olan nesneyi kullan" komutunu ver.
        if (proximitySelector != null)
        {
            proximitySelector.UseCurrentSelection();
        }
    }
}