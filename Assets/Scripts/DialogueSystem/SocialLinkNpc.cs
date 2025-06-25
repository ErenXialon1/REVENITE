using UnityEngine;
using PixelCrushers.DialogueSystem;

public class SocialLinkNpc : MonoBehaviour
{
    public SocialLinkManager socialLinkManager;
    [Tooltip("Core_DB'deki Actor'un tam adý.")]
    public string characterNameID;
    private void Start()
    {
        socialLinkManager = SocialLinkManager.Instance;
    }
    // Bu fonksiyon, Player'daki Proximity Selector tarafýndan 'OnUse' mesajý gönderildiðinde çalýþýr.
    public void OnUse()
    {
        if (DialogueManager.IsConversationActive) return;

        // Artýk if/else yok. Sadece Manager'daki karar mekanizmasýný çaðýrýyoruz.
        SocialLinkManager.Instance.DecideAndStartConversation(characterNameID);
    }
}