using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bir kombo zincirini temsil eden ScriptableObject.
/// Her kombo (örneðin Light Attack Combo) için bir tane oluþturulur.
/// </summary>
[CreateAssetMenu(fileName = "NewComboData", menuName = "Combat/Combo Data")]
public class ComboData : ScriptableObject
{
    [Header("Kombo Tanýmlamasý")]
    [Tooltip("Kombonun kod içerisinden çaðrýlacaðý benzersiz ID. Örneðin 'LightAttack' veya 'HeavyAttack'.")]
    [SerializeField] private int comboIndex;
    [Header("Kombo Sýralamasý")]
    [Tooltip("Bu komboyu yapmak için basýlmasý gereken tuþlarýn sýralý listesi.")]
    [SerializeField] private List<AttackInput> inputSequence;
    [Tooltip("Bu kombo zincirini oluþturan ve sýralý olmasý gereken saldýrýlar.")]
    [SerializeField] private List<SkillData> comboSkills;

    // --- Public Properties ---
    public int ComboIndex => comboIndex;
    /// <summary>
    /// Kombonun tuþ sýralamasý (readonly).
    /// </summary>
    public IReadOnlyList<AttackInput> InputSequence => inputSequence;
    public List<SkillData> ComboSkills => comboSkills;
    /// <summary>
    /// Editörde daha kolay kontrol için bir doðrulama.
    /// Tuþ sýralamasý ile yetenek listesinin sayýsý eþleþmelidir.
    /// </summary>
    private void OnValidate()
    {
        if (inputSequence.Count != comboSkills.Count)
        {
            Debug.LogWarning($"Kombo Verisi Hatasý ({this.name}): 'Input Sequence' ({inputSequence.Count}) ve 'Combo Skills' ({comboSkills.Count}) listelerinin eleman sayýlarý ayný olmalýdýr.");
        }
    }
}