using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bir kombo zincirini temsil eden ScriptableObject.
/// Her kombo (�rne�in Light Attack Combo) i�in bir tane olu�turulur.
/// </summary>
[CreateAssetMenu(fileName = "NewComboData", menuName = "Combat/Combo Data")]
public class ComboData : ScriptableObject
{
    [Header("Kombo Tan�mlamas�")]
    [Tooltip("Kombonun kod i�erisinden �a�r�laca�� benzersiz ID. �rne�in 'LightAttack' veya 'HeavyAttack'.")]
    [SerializeField] private int comboIndex;
    [Header("Kombo S�ralamas�")]
    [Tooltip("Bu komboyu yapmak i�in bas�lmas� gereken tu�lar�n s�ral� listesi.")]
    [SerializeField] private List<AttackInput> inputSequence;
    [Tooltip("Bu kombo zincirini olu�turan ve s�ral� olmas� gereken sald�r�lar.")]
    [SerializeField] private List<SkillData> comboSkills;

    // --- Public Properties ---
    public int ComboIndex => comboIndex;
    /// <summary>
    /// Kombonun tu� s�ralamas� (readonly).
    /// </summary>
    public IReadOnlyList<AttackInput> InputSequence => inputSequence;
    public List<SkillData> ComboSkills => comboSkills;
    /// <summary>
    /// Edit�rde daha kolay kontrol i�in bir do�rulama.
    /// Tu� s�ralamas� ile yetenek listesinin say�s� e�le�melidir.
    /// </summary>
    private void OnValidate()
    {
        if (inputSequence.Count != comboSkills.Count)
        {
            Debug.LogWarning($"Kombo Verisi Hatas� ({this.name}): 'Input Sequence' ({inputSequence.Count}) ve 'Combo Skills' ({comboSkills.Count}) listelerinin eleman say�lar� ayn� olmal�d�r.");
        }
    }
}