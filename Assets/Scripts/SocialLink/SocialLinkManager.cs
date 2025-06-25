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
    private string lastConversantActorName;

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
        Lua.RegisterFunction("CheckForRankUp", this, SymbolExtensions.GetMethodInfo(() => CheckForRankUp(string.Empty)));
        Lua.RegisterFunction("AddSocialLinkPoints", this, SymbolExtensions.GetMethodInfo(() => AddPoints(string.Empty, 0)));
        Lua.RegisterFunction("SetSocialLinkState", this, SymbolExtensions.GetMethodInfo(() => ForceSetState(string.Empty, string.Empty)));
    }
    private void OnDisable()
    {
        Lua.UnregisterFunction("CheckForRankUp");
        Lua.UnregisterFunction("AddSocialLinkPoints");
        Lua.UnregisterFunction("SetSocialLinkState");
    }

    /// <summary>
    /// Bir karaktere Sosyal Bað puaný ekler. Her yerden çaðrýlabilir (diyalog, minigame vs.).
    /// </summary>
    public void AddPoints(string characterName, double pointsToAdd)
    {
        if (!configDictionary.ContainsKey(characterName)) return;

        double currentPoints = DialogueLua.GetActorField(characterName, "SocialLinkPoints").AsInt;
        currentPoints += pointsToAdd;
        DialogueLua.SetActorField(characterName, "SocialLinkPoints", currentPoints);

        Debug.Log($"{characterName} karakterine {pointsToAdd} puan eklendi. Yeni Toplam Puan: {currentPoints}");
    }

    /// <summary>
    /// Bir karakterin durumunu dýþarýdan zorla deðiþtirir. (Örn: Ana hikayedeki bir olay sonrasý)
    /// </summary>
    public void ForceSetState(string characterName, string state)
    {
        if (!configDictionary.ContainsKey(characterName)) return;
        DialogueLua.SetActorField(characterName, "SocialLinkState", state);
        Debug.LogWarning($"{characterName} durumu dýþarýdan {state} olarak ayarlandý.");
    }
    /// <summary>
    /// Bir diyalog baþladýðýnda Dialogue Manager tarafýndan çaðrýlýr.
    /// </summary>
    public void SetLastActorName(Transform conversant)
    {
        // Konuþulan NPC'nin kim olduðunu bul ve ismini kaydet.
        lastConversantActorName = DialogueActor.GetActorName(conversant);
    }

    /// <summary>
    /// Bir diyalog bittiðinde Dialogue Manager tarafýndan çaðrýlýr.
    /// </summary>
    public void CheckRankUpLastActor()
    {
        // Eðer son konuþulan bir karakter varsa, onun için rank atlama kontrolü yap.
        if (!string.IsNullOrEmpty(lastConversantActorName))
        {
            CheckForRankUp(lastConversantActorName);
            // Bir sonraki konuþma için ismi temizle (isteðe baðlý ama iyi bir pratik).
            lastConversantActorName = null;
        }
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
    /// NPC ile konuþma baþlatýldýðýnda çaðrýlan merkezi karar mekanizmasý.
    /// </summary>
    public void DecideAndStartConversation(string characterName)
    {
        // Karakterin mevcut durumunu Lua'dan oku.
        string currentState = DialogueLua.GetActorField(characterName, "SocialLinkState").AsString;
        int currentRank = DialogueLua.GetActorField(characterName, "SocialLinkRank").AsInt;
        string conversationName = "";

        // Duruma göre hangi konuþmanýn baþlayacaðýna karar ver.
        switch (currentState)
        {
            case "NotMet":
                conversationName = $"{characterName}_MeetingConversation";
                break;

            case "RankUpReady":
                // Rank atlama konuþmasý, bir sonraki rank için olanýdýr.
                conversationName = $"SL_{characterName}_RankUp_{currentRank + 1}";
                break;

            case "Standard":
            default:
                // Standart konuþma, mevcut rank için olanýdýr.
                conversationName = $"{characterName}_Rank_{currentRank}_StandardConversation";
                break;
        }
        if (!string.IsNullOrEmpty(conversationName))
        {
            DialogueManager.StartConversation(conversationName);
        }
        else
        {
            Debug.LogWarning($"{characterName} için {currentState} durumunda bir konuþma bulunamadý!");
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

            if (currentPoints >= pointsNeeded)
            {
                // RANK'I ARTIRMA! Sadece durumu "Rank Atlamaya Hazýr" olarak deðiþtir.
                DialogueLua.SetActorField(characterName, "SocialLinkState", "RankUpReady");
                Debug.LogWarning($"{characterName} rank atlamaya hazýr duruma geçti!");
            }
        }
        Debug.Log("31");
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