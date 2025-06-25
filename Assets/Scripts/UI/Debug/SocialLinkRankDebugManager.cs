using UnityEngine;
using TMPro; // TextMeshPro k�t�phanesini kullanmak i�in gerekli.
using PixelCrushers.DialogueSystem; // Usable ve DialogueLua i�in gerekli.

public class SocialLinkRankDebugManager : MonoBehaviour
{
    [Header("UI Referanslar�")]
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI pointsText;

    private Usable currentTarget; // Takip etti�imiz mevcut NPC

    void Start()
    {
        // Oyun ba��nda panelin kesinlikle gizli oldu�undan emin ol.
        if (debugPanel != null)
        {
            debugPanel.SetActive(false);
        }
    }

    void Update()
    {
        // E�er bir hedefimiz varsa, paneldeki bilgileri her frame g�ncelle.
        if (currentTarget != null)
        {
            UpdatePanelInfo();
        }
    }

    // Proximity Selector'dan hedef NPC'yi ayarlamak i�in �a�r�lacak.
    public void SetDebugTarget(Usable selectedUsable)
    {
        if (selectedUsable != null)
        {
            currentTarget = selectedUsable;
            if (debugPanel != null) debugPanel.SetActive(true);
            UpdatePanelInfo(); // Paneli hemen g�ncelle.
        }
    }

    // Proximity Selector'dan hedefi temizlemek i�in �a�r�lacak.
    public void ClearDebugTarget()
    {
        currentTarget = null;
        if (debugPanel != null) debugPanel.SetActive(false);
    }

    // Paneldeki metinleri g�ncelleyen fonksiyon.
    private void UpdatePanelInfo()
    {
        if (currentTarget == null) return;

        // Dialogue System'deki Actor'un ad�n� al.
        string actorName = currentTarget.GetComponent<SocialLinkNpc>().characterNameID;

        // Dialogue System'in haf�zas�ndan (Lua) verileri �ek.
        int rank = DialogueLua.GetActorField(actorName, "SocialLinkRank").AsInt;
        int points = DialogueLua.GetActorField(actorName, "SocialLinkPoints").AsInt;

        // UI metinlerini g�ncelle.
        npcNameText.text = "NPC: " + actorName;
        rankText.text = "Rank: " + rank;
        pointsText.text = "Points: " + points;
    }
}
