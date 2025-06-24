using UnityEngine;

[CreateAssetMenu(fileName = "New Social Link Data", menuName = "Social Link/Social Link Data")]
public class SocialLinkData : ScriptableObject
{
    [Header("Character Info")]
    public string characterName; // Bu isim, diyaloglarda ve kodda karakteri tanýmak için kullanýlacak. Her karakterin farklý olmak zorunda.

    [Header("Social Link Ranks")]
   
    public int[] rankUpPointsNeeded; //listenin (n)'inci sýrasýndaki deðer (n+1). sýradaki ranka ulaþmak için gereken puandýr.

    
}