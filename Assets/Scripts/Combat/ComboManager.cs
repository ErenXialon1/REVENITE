using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    #region Inner Class: ComboTrieNode
    /// <summary>
    /// Kombo aðacýndaki her bir adýmý temsil eden düðüm (node).
    /// </summary>
    private class ComboTrieNode
    {
        // Bu düðümden sonra hangi girdilerin gelebileceðini ve hangi düðüme gideceðini tutar.
        public Dictionary<AttackInput, ComboTrieNode> children = new Dictionary<AttackInput, ComboTrieNode>();

        // Eðer bu düðüm, geçerli bir kombonun bir adýmýný temsil ediyorsa, ilgili ComboData burada tutulur.
        public ComboData Combo { get; private set; }

        // Bu düðümün, kombodaki kaçýncý adýmý temsil ettiðini belirtir.
        public int ComboStep { get; private set; }

        public ComboTrieNode() { }

        public void SetComboStep(ComboData combo, int step)
        {
            this.Combo = combo;
            this.ComboStep = step;
        }
    }
    #endregion

    #region References and Settings
    [Header("Referanslar")]
    [Tooltip("Inputlarý okuyacak olan script'in referansý. IInputReader arayüzünü implemente etmeli.")]
    [SerializeField] private MonoBehaviour inputReaderRef;
    [Tooltip("Karakterin yapabildiði tüm kombolarý tutan CharacterCombos script'i.")]
    [SerializeField] private CharacterCombos characterCombos;
    [Tooltip("Asýl saldýrý mekaniklerini yürüten script.")]
    [SerializeField] private CombatController combatController;
    [Tooltip("Oyuncu durum makinesi.")]
    [SerializeField] private PlayerStateMachine stateMachine;
    // PlayerMain'e de referans gerekebilir, aþaðýdaki PlayerMain düzenlemesine bakýnýz.
    [SerializeField] private PlayerMain playerMain;


    [Header("Ayarlar")]
    [Tooltip("Bir saldýrýdan sonra yeni bir kombo adýmý için ne kadar süre bekleneceði.")]
    [SerializeField] private float comboWindow = 1.5f;

    // Arayüz (interface) referansý
    private IInputReader inputReader;

    // --- Trie Veri Yapýsý ---
    private ComboTrieNode rootNode; // Tüm kombolarýn baþlangýç noktasý olan kök düðüm.
    private ComboTrieNode currentNode; // Þu anki kombo dizisinde nerede olduðumuzu gösteren düðüm.

    // --- State ---
    private float lastInputTime; // En son ne zaman saldýrý tuþuna basýldýðý.
    private bool isComboActive = false; // Bir kombo zincirinin içinde miyiz?
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Referanslarý al
        inputReader = inputReaderRef as IInputReader;
        if (characterCombos == null) characterCombos = GetComponent<CharacterCombos>();
        if (combatController == null) combatController = GetComponent<CombatController>();
        if (stateMachine == null) stateMachine = GetComponent<PlayerStateMachine>();
        if (playerMain == null) playerMain = GetComponent<PlayerMain>();
    }

    private void Start()
    {
        BuildComboTrie();
        ResetCombo();
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            // Input event'ine abone ol
            inputReader.AttackEvent += OnAttackPressed;
        }
        if (combatController != null)
        {
            // Saldýrý animasyonu bittiðinde haberdar olmak için abone ol
            combatController.AttackFinished += OnAttackFinished;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.AttackEvent -= OnAttackPressed;
        }
        if (combatController != null)
        {
            combatController.AttackFinished -= OnAttackFinished;
        }
    }
    private void Update()
    {
        // Eðer bir kombo aktifse ve belirlenen süreden daha fazla beklediysek, komboyu sýfýrla.
        if (isComboActive && Time.time > lastInputTime + comboWindow)
        {
            ResetCombo();
        }
    }

    #endregion
    #region Trie Building
    /// <summary>
    /// CharacterCombos'taki tüm ComboData'larý okuyarak Trie (Prefix Tree) aðacýný oluþturur.
    /// </summary>
    private void BuildComboTrie()
    {
        rootNode = new ComboTrieNode();
        foreach (var combo in characterCombos.AvailableCombos)
        {
            AddComboToTrie(combo);
        }
    }

    /// <summary>
    /// Tek bir ComboData'yý Trie'ye ekler.
    /// </summary>
    private void AddComboToTrie(ComboData combo)
    {
        if (combo.InputSequence == null || combo.InputSequence.Count == 0) return;

        ComboTrieNode node = rootNode;
        for (int i = 0; i < combo.InputSequence.Count; i++)
        {
            AttackInput input = combo.InputSequence[i];
            if (!node.children.ContainsKey(input))
            {
                node.children[input] = new ComboTrieNode();
            }
            node = node.children[input];
            // Her adýmý, hangi komboya ait olduðu ve kaçýncý adým olduðu bilgisiyle iþaretle
            node.SetComboStep(combo, i);
        }
    }
    #endregion

    #region Combo Logic
    /// <summary>
    /// InputReader'dan bir saldýrý girdisi geldiðinde tetiklenir.
    /// </summary>
    private void OnAttackPressed(AttackInput input)
    {
        // Eðer karakter zaten bir saldýrý animasyonu oynatýyorsa, bu input'u þimdilik görmezden gel.
        // (Gelecekte "input buffering" için burasý geliþtirilebilir.)
        if (combatController.IsAttacking) return;

        AdvanceCombo(input);
    }

    /// <summary>
    /// Gelen input ile kombo aðacýnda bir sonraki adýma ilerlemeye çalýþýr.
    /// </summary>
    private void AdvanceCombo(AttackInput input)
    {
        // Mevcut düðümün çocuklarý arasýnda gelen input'a uygun bir yol var mý?
        if (currentNode.children.TryGetValue(input, out ComboTrieNode nextNode))
        {
            // Varsa, o yola ilerle
            currentNode = nextNode;
            isComboActive = true;
            lastInputTime = Time.time;
            ExecuteAttack();
        }
        else
        {
            // Yoksa, bu geçersiz bir girdi. Komboyu sýfýrla.
            ResetCombo();

            // Sýfýrladýktan sonra, bu yeni girdinin yeni bir kombo baþlatýp baþlatmadýðýný kontrol et.
            // Bu sayede oyuncu yanlýþ tuþa bassa bile hemen yeni bir komboya baþlayabilir.
            if (rootNode.children.TryGetValue(input, out ComboTrieNode firstNode))
            {
                currentNode = firstNode;
                isComboActive = true;
                lastInputTime = Time.time;
                ExecuteAttack();
            }
        }
    }

    /// <summary>
    /// Mevcut kombo adýmýna karþýlýk gelen saldýrýyý baþlatýr.
    /// </summary>
    private void ExecuteAttack()
    {
        if (currentNode.Combo == null)
        {
            // Bu durum normalde yaþanmamalý, ama bir güvenlik önlemi.
            //Debug.LogError("Mevcut kombo düðümünde ComboData bulunamadý!");
            ResetCombo();
            return;
        }

        // State machine'e saldýrý durumuna geçmesini söyle
        playerMain.StartAttack(currentNode.Combo, currentNode.ComboStep);
    }

    /// <summary>
    /// Bir saldýrý animasyonu tamamlandýðýnda CombatController tarafýndan tetiklenir.
    /// Kombo penceresini baþlatýr.
    /// </summary>
    private void OnAttackFinished()
    {
        // Kombonun devam etmesi için zamaný baþlat.
        lastInputTime = Time.time;
        isComboActive = true;

        // Eðer mevcut düðüm bir kombonun son adýmýysa, bir sonraki tuþa basýldýðýnda
        // yeni bir kombo baþlamasý için currentNode'u baþa al.
        if (currentNode.children.Count == 0)
        {
            ResetCombo();
        }
    }

    /// <summary>
    /// Kombo durumunu baþlangýç noktasýna sýfýrlar.
    /// </summary>
    private void ResetCombo()
    {
        currentNode = rootNode;
        isComboActive = false;
        //Debug.Log("Kombo sýfýrlandý.");
    }
    #endregion

}
