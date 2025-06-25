using UnityEngine;
using TMPro; // TextMeshPro kütüphanesini kullanmak için gerekli.
using PixelCrushers.DialogueSystem; // Usable ve DialogueLua için gerekli.

public class SocialLinkRankDebugManager : MonoBehaviour
{
    [Header("UI Referanslarý")]
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI pointsText;

    private Usable currentTarget; // Takip ettiðimiz mevcut NPC

    void Start()
    {
        // Oyun baþýnda panelin kesinlikle gizli olduðundan emin ol.
        if (debugPanel != null)
        {
            debugPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Eðer bir hedefimiz varsa, paneldeki bilgileri her frame güncelle.
        if (currentTarget != null)
        {
            UpdatePanelInfo();
        }
    }

    // Proximity Selector'dan hedef NPC'yi ayarlamak için çaðrýlacak.
    public void SetDebugTarget(Usable selectedUsable)
    {
        if (selectedUsable != null)
        {
            currentTarget = selectedUsable;
            if (debugPanel != null) debugPanel.SetActive(true);
            UpdatePanelInfo(); // Paneli hemen güncelle.
        }
    }

    // Proximity Selector'dan hedefi temizlemek için çaðrýlacak.
    public void ClearDebugTarget()
    {
        currentTarget = null;
        if (debugPanel != null) debugPanel.SetActive(false);
    }

    // Paneldeki metinleri güncelleyen fonksiyon.
    private void UpdatePanelInfo()
    {
        if (currentTarget == null) return;

        // Dialogue System'deki Actor'un adýný al.
        string actorName = currentTarget.GetComponent<SocialLinkNpc>().characterNameID;

        // Dialogue System'in hafýzasýndan (Lua) verileri çek.
        int rank = DialogueLua.GetActorField(actorName, "SocialLinkRank").AsInt;
        int points = DialogueLua.GetActorField(actorName, "SocialLinkPoints").AsInt;

        // UI metinlerini güncelle.
        npcNameText.text = "NPC: " + actorName;
        rankText.text = "Rank: " + rank;
        pointsText.text = "Points: " + points;
    }
}
