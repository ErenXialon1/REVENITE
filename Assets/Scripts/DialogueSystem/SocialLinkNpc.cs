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
    // Bu fonksiyon, Player'daki Proximity Selector tarafýndan 'OnUse' mesajý gönderildiðinde çalýþýr.
    public void OnUse()
    {
        if (DialogueManager.IsConversationActive) return;

        // Karakterle tanýþýlýp tanýþýlmadýðýný kontrol et.
        if (DialogueLua.GetActorField(characterNameID, "IsMet").AsBool == true)
        {
            socialLinkManager.StandardConversationStart(characterNameID, socialLinkManager.GetCharSocialLinkRank(characterNameID));
            Debug.Log("1");
        }
        else
        {
            // Hayýr, tanýþýlmadý: Tanýþma diyaloðunu baþlat.
            socialLinkManager.MeetingConversationStart(characterNameID);
            Debug.Log("2");
        }
    }
}