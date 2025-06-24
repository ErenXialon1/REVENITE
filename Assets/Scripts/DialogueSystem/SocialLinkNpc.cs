using UnityEngine;
using PixelCrushers.DialogueSystem;

public class SocialLinkNpc : MonoBehaviour
{
    public SocialLinkManager socialLinkManager;
    public string characterNameID;
    private void Start()
    {
        socialLinkManager = SocialLinkManager.Instance;
    }
    // Bu fonksiyon, Player'daki Proximity Selector taraf�ndan 'OnUse' mesaj� g�nderildi�inde �al���r.
    public void OnUse()
    {
        if (DialogueManager.IsConversationActive) return;

        // Karakterle tan���l�p tan���lmad���n� kontrol et.
        if (DialogueLua.GetActorField(characterNameID, "IsMet").AsBool == true)
        {
            socialLinkManager.StandardConversationStart(characterNameID, socialLinkManager.GetCharSocialLinkRank(characterNameID));
            Debug.Log("1");
        }
        else
        {
            // Hay�r, tan���lmad�: Tan��ma diyalo�unu ba�lat.
            socialLinkManager.MeetingConversationStart(characterNameID);
            Debug.Log("2");
        }
    }
}