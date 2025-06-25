using UnityEngine;
using PixelCrushers.DialogueSystem;

public class SocialLinkNpc : MonoBehaviour
{
    public SocialLinkManager socialLinkManager;
    [Tooltip("Core_DB'deki Actor'un tam ad�.")]
    public string characterNameID;
    private void Start()
    {
        socialLinkManager = SocialLinkManager.Instance;
    }
    // Bu fonksiyon, Player'daki Proximity Selector taraf�ndan 'OnUse' mesaj� g�nderildi�inde �al���r.
    public void OnUse()
    {
        if (DialogueManager.IsConversationActive) return;

        // Art�k if/else yok. Sadece Manager'daki karar mekanizmas�n� �a��r�yoruz.
        SocialLinkManager.Instance.DecideAndStartConversation(characterNameID);
    }
}