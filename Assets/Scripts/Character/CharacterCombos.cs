using System.Collections.Generic;
using UnityEngine;

public class CharacterCombos : MonoBehaviour
{
    [Tooltip("Karakterin ba�lang��ta bildi�i kombolar. Inspector'dan atanabilir.")]
    [SerializeField] private List<ComboData> availableCombos = new List<ComboData>();

    public IReadOnlyList<ComboData> AvailableCombos => availableCombos;

    /// <summary>
    /// Karaktere yeni bir kombo ��retir.
    /// </summary>
    public void AddCombo(ComboData comboData)
    {
        if (comboData != null && !availableCombos.Contains(comboData))
        {
            availableCombos.Add(comboData);
            Debug.Log($"{gameObject.name} learned the '{comboData.ComboIndex}' combo.");
        }
    }

    /// <summary>
    /// Karakterin bir komboyu unutmas�n� sa�lar.
    /// </summary>
    public void DiscardCombo(ComboData comboData)
    {
        if (comboData != null && availableCombos.Contains(comboData))
        {
            availableCombos.Remove(comboData);
            Debug.Log($"{gameObject.name} forgot the '{comboData.ComboIndex}' combo.");
        }
    }

    /// <summary>
    /// Karakterin belirtilen ID'ye sahip bir komboyu bilip bilmedi�ini kontrol eder.
    /// </summary>
    public bool IsComboAvailable(int comboIndex)
    {
        return availableCombos.Exists(c => c.ComboIndex == comboIndex);
    }
}
