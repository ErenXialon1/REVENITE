using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    #region Inner Class: ComboTrieNode
    /// <summary>
    /// Kombo a�ac�ndaki her bir ad�m� temsil eden d���m (node).
    /// </summary>
    private class ComboTrieNode
    {
        // Bu d���mden sonra hangi girdilerin gelebilece�ini ve hangi d���me gidece�ini tutar.
        public Dictionary<AttackInput, ComboTrieNode> children = new Dictionary<AttackInput, ComboTrieNode>();

        // E�er bu d���m, ge�erli bir kombonun bir ad�m�n� temsil ediyorsa, ilgili ComboData burada tutulur.
        public ComboData Combo { get; private set; }

        // Bu d���m�n, kombodaki ka��nc� ad�m� temsil etti�ini belirtir.
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
    [Tooltip("Inputlar� okuyacak olan script'in referans�. IInputReader aray�z�n� implemente etmeli.")]
    [SerializeField] private MonoBehaviour inputReaderRef;
    [Tooltip("Karakterin yapabildi�i t�m kombolar� tutan CharacterCombos script'i.")]
    [SerializeField] private CharacterCombos characterCombos;
    [Tooltip("As�l sald�r� mekaniklerini y�r�ten script.")]
    [SerializeField] private CombatController combatController;
    [Tooltip("Oyuncu durum makinesi.")]
    [SerializeField] private PlayerStateMachine stateMachine;
    // PlayerMain'e de referans gerekebilir, a�a��daki PlayerMain d�zenlemesine bak�n�z.
    [SerializeField] private PlayerMain playerMain;


    [Header("Ayarlar")]
    [Tooltip("Bir sald�r�dan sonra yeni bir kombo ad�m� i�in ne kadar s�re beklenece�i.")]
    [SerializeField] private float comboWindow = 1.5f;

    // Aray�z (interface) referans�
    private IInputReader inputReader;

    // --- Trie Veri Yap�s� ---
    private ComboTrieNode rootNode; // T�m kombolar�n ba�lang�� noktas� olan k�k d���m.
    private ComboTrieNode currentNode; // �u anki kombo dizisinde nerede oldu�umuzu g�steren d���m.

    // --- State ---
    private float lastInputTime; // En son ne zaman sald�r� tu�una bas�ld���.
    private bool isComboActive = false; // Bir kombo zincirinin i�inde miyiz?
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Referanslar� al
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
            // Sald�r� animasyonu bitti�inde haberdar olmak i�in abone ol
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
        // E�er bir kombo aktifse ve belirlenen s�reden daha fazla beklediysek, komboyu s�f�rla.
        if (isComboActive && Time.time > lastInputTime + comboWindow)
        {
            ResetCombo();
        }
    }

    #endregion
    #region Trie Building
    /// <summary>
    /// CharacterCombos'taki t�m ComboData'lar� okuyarak Trie (Prefix Tree) a�ac�n� olu�turur.
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
    /// Tek bir ComboData'y� Trie'ye ekler.
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
            // Her ad�m�, hangi komboya ait oldu�u ve ka��nc� ad�m oldu�u bilgisiyle i�aretle
            node.SetComboStep(combo, i);
        }
    }
    #endregion

    #region Combo Logic
    /// <summary>
    /// InputReader'dan bir sald�r� girdisi geldi�inde tetiklenir.
    /// </summary>
    private void OnAttackPressed(AttackInput input)
    {
        // E�er karakter zaten bir sald�r� animasyonu oynat�yorsa, bu input'u �imdilik g�rmezden gel.
        // (Gelecekte "input buffering" i�in buras� geli�tirilebilir.)
        if (combatController.IsAttacking) return;

        AdvanceCombo(input);
    }

    /// <summary>
    /// Gelen input ile kombo a�ac�nda bir sonraki ad�ma ilerlemeye �al���r.
    /// </summary>
    private void AdvanceCombo(AttackInput input)
    {
        // Mevcut d���m�n �ocuklar� aras�nda gelen input'a uygun bir yol var m�?
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
            // Yoksa, bu ge�ersiz bir girdi. Komboyu s�f�rla.
            ResetCombo();

            // S�f�rlad�ktan sonra, bu yeni girdinin yeni bir kombo ba�lat�p ba�latmad���n� kontrol et.
            // Bu sayede oyuncu yanl�� tu�a bassa bile hemen yeni bir komboya ba�layabilir.
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
    /// Mevcut kombo ad�m�na kar��l�k gelen sald�r�y� ba�lat�r.
    /// </summary>
    private void ExecuteAttack()
    {
        if (currentNode.Combo == null)
        {
            // Bu durum normalde ya�anmamal�, ama bir g�venlik �nlemi.
            //Debug.LogError("Mevcut kombo d���m�nde ComboData bulunamad�!");
            ResetCombo();
            return;
        }

        // State machine'e sald�r� durumuna ge�mesini s�yle
        playerMain.StartAttack(currentNode.Combo, currentNode.ComboStep);
    }

    /// <summary>
    /// Bir sald�r� animasyonu tamamland���nda CombatController taraf�ndan tetiklenir.
    /// Kombo penceresini ba�lat�r.
    /// </summary>
    private void OnAttackFinished()
    {
        // Kombonun devam etmesi i�in zaman� ba�lat.
        lastInputTime = Time.time;
        isComboActive = true;

        // E�er mevcut d���m bir kombonun son ad�m�ysa, bir sonraki tu�a bas�ld���nda
        // yeni bir kombo ba�lamas� i�in currentNode'u ba�a al.
        if (currentNode.children.Count == 0)
        {
            ResetCombo();
        }
    }

    /// <summary>
    /// Kombo durumunu ba�lang�� noktas�na s�f�rlar.
    /// </summary>
    private void ResetCombo()
    {
        currentNode = rootNode;
        isComboActive = false;
        //Debug.Log("Kombo s�f�rland�.");
    }
    #endregion

}
