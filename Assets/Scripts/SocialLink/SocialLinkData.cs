using UnityEngine;

[CreateAssetMenu(fileName = "New Social Link Data", menuName = "Social Link/Social Link Data")]
public class SocialLinkData : ScriptableObject
{
    [Header("Character Info")]
    public string characterName; // Bu isim, diyaloglarda ve kodda karakteri tan�mak i�in kullan�lacak. Her karakterin farkl� olmak zorunda.

    [Header("Social Link Ranks")]
   
    public int[] rankUpPointsNeeded; //listenin (n)'inci s�ras�ndaki de�er (n+1). s�radaki ranka ula�mak i�in gereken puand�r.

    
}