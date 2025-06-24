using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class SocialLinkManager : MonoBehaviour
{
    // --- Singleton ---
    public static SocialLinkManager Instance { get; private set; }

    // Bu liste karakterlerin "deðiþmeyen deðerlerini" tutuyor.
    [SerializeField] private List<SocialLinkData> socialLinkConfigs;

    
    public Dictionary<string, SocialLinkData> configDictionary = new Dictionary<string, SocialLinkData>();

    private void Awake()
    {
        // --- Singleton Kurulumu ---
        // Sahnede zaten bir SocialLinkManager varsa, bu yenisini yok et. Sadece bir tane olmalý.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Bu manager'ýn yeni bir sahne yüklendiðinde yok olmasýný engeller.
            DontDestroyOnLoad(gameObject);

            // Sözlüðü hazýrlayarak sistemi baþlat.
            SetupDictionary();
        }
    }
    private void OnEnable()
    {
        Lua.RegisterFunction("CheckForRankUp", this, SymbolExtensions.GetMethodInfo(() => CheckForRankUp(default(string))));
    }
    private void OnDisable()
    {
        Lua.UnregisterFunction("CheckForRankUp");
    }
    /// <summary>
    /// Editörden atanan listeyi, hýzlý eriþim için bir sözlüðe dönüþtürür.
    /// </summary>
    private void SetupDictionary()
    {
       foreach (var config in socialLinkConfigs)
        {
            if (config != null && !configDictionary.ContainsKey(config.characterName))
            {
                configDictionary.Add(config.characterName, config);
            }
        }
    }


    /// <summary>
    /// Bir karakterin rank atlayýp atlamadýðýný kontrol eder. 
    /// Bu fonksiyon artýk diyalog içerisinden CSharp.Run() ile çaðrýlacak.
    /// </summary>
    /// <param name="characterName">Kontrol edilecek karakterin adý.</param>
    public void CheckForRankUp(string characterName)
    {
        // Karakterin ayar dosyasýný sözlükten bul.
        if (configDictionary.TryGetValue(characterName, out SocialLinkData config))
        {
            // Güncel Puan ve Rank deðerlerini doðrudan Dialogue System'in hafýzasýndan oku.
            int currentPoints = DialogueLua.GetActorField(characterName, "SocialLinkPoints").AsInt;
            int currentRank = DialogueLua.GetActorField(characterName, "SocialLinkRank").AsInt;

            // Gerekli puan dizisinin sýnýrlarýný aþýp aþmadýðýmýzý kontrol et.
            if (currentRank >= config.rankUpPointsNeeded.Length)
            {
                return; // Zaten maksimum rank'ta.
            }

            // Gerekli puaný ayar dosyasýndan al.
            int pointsNeeded = config.rankUpPointsNeeded[currentRank];

            // Rank atlama koþulu saðlanýyor mu?
            if (currentPoints >= pointsNeeded)
            {
                RankUp(characterName, currentRank + 1);
            }
        }
        Debug.Log("31");
    }

    public void RankUp(string characterName, int newRank)
    {
        DialogueLua.SetActorField(characterName, "SocialLinkRank", newRank);
        Debug.LogWarning(characterName + " ile sosyal baðýn Rank " + newRank + " seviyesine YÜKSELDÝ!");

        
    }
    public void RankUpConversationStart(string characterName, int newRank)
    {
        // Rank atlama diyaloðunun adýný dinamik olarak oluþtur.
        string rankUpConversationName = $"SL_{characterName}_RankUp_{newRank}";

        // Dinamik olarak oluþturulan isimdeki konuþmayý baþlat.
        DialogueManager.StartConversation(rankUpConversationName);
    }
    public void MeetingConversationStart(string characterName)
    {
        // Rank atlama diyaloðunun adýný dinamik olarak oluþtur.
        string meetingConversationName = $"{characterName}_MeetingConversation";

        // Dinamik olarak oluþturulan isimdeki konuþmayý baþlat.
        DialogueManager.StartConversation(meetingConversationName);
    }
    public void StandardConversationStart(string characterName, int socialLinkRank)
    {
        // Rank atlama diyaloðunun adýný dinamik olarak oluþtur.
        string standardConversationName = $"{characterName}_Rank_{socialLinkRank}_StandardConversation";

        // Dinamik olarak oluþturulan isimdeki konuþmayý baþlat.
        DialogueManager.StartConversation(standardConversationName);
    }
    public int GetCharSocialLinkRank(string characterName)
    {
        int currentRank = DialogueLua.GetActorField(characterName, "SocialLinkRank").AsInt;
        return currentRank;
    }

}