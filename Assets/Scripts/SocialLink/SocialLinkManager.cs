using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class SocialLinkManager : MonoBehaviour
{
    // --- Singleton ---
    public static SocialLinkManager Instance { get; private set; }

    // Bu liste karakterlerin "de�i�meyen de�erlerini" tutuyor.
    [SerializeField] private List<SocialLinkData> socialLinkConfigs;


    
    public Dictionary<string, SocialLinkData> configDictionary = new Dictionary<string, SocialLinkData>();
    private string lastConversantActorName;

    private void Awake()
    {
        // --- Singleton Kurulumu ---
        // Sahnede zaten bir SocialLinkManager varsa, bu yenisini yok et. Sadece bir tane olmal�.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Bu manager'�n yeni bir sahne y�klendi�inde yok olmas�n� engeller.
            DontDestroyOnLoad(gameObject);

            // S�zl��� haz�rlayarak sistemi ba�lat.
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
    /// Bir karaktere Sosyal Ba� puan� ekler. Her yerden �a�r�labilir (diyalog, minigame vs.).
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
    /// Bir karakterin durumunu d��ar�dan zorla de�i�tirir. (�rn: Ana hikayedeki bir olay sonras�)
    /// </summary>
    public void ForceSetState(string characterName, string state)
    {
        if (!configDictionary.ContainsKey(characterName)) return;
        DialogueLua.SetActorField(characterName, "SocialLinkState", state);
        Debug.LogWarning($"{characterName} durumu d��ar�dan {state} olarak ayarland�.");
    }
    /// <summary>
    /// Bir diyalog ba�lad���nda Dialogue Manager taraf�ndan �a�r�l�r.
    /// </summary>
    public void SetLastActorName(Transform conversant)
    {
        // Konu�ulan NPC'nin kim oldu�unu bul ve ismini kaydet.
        lastConversantActorName = DialogueActor.GetActorName(conversant);
    }

    /// <summary>
    /// Bir diyalog bitti�inde Dialogue Manager taraf�ndan �a�r�l�r.
    /// </summary>
    public void CheckRankUpLastActor()
    {
        // E�er son konu�ulan bir karakter varsa, onun i�in rank atlama kontrol� yap.
        if (!string.IsNullOrEmpty(lastConversantActorName))
        {
            CheckForRankUp(lastConversantActorName);
            // Bir sonraki konu�ma i�in ismi temizle (iste�e ba�l� ama iyi bir pratik).
            lastConversantActorName = null;
        }
    }
    /// <summary>
    /// Edit�rden atanan listeyi, h�zl� eri�im i�in bir s�zl��e d�n��t�r�r.
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
    /// NPC ile konu�ma ba�lat�ld���nda �a�r�lan merkezi karar mekanizmas�.
    /// </summary>
    public void DecideAndStartConversation(string characterName)
    {
        // Karakterin mevcut durumunu Lua'dan oku.
        string currentState = DialogueLua.GetActorField(characterName, "SocialLinkState").AsString;
        int currentRank = DialogueLua.GetActorField(characterName, "SocialLinkRank").AsInt;
        string conversationName = "";

        // Duruma g�re hangi konu�man�n ba�layaca��na karar ver.
        switch (currentState)
        {
            case "NotMet":
                conversationName = $"{characterName}_MeetingConversation";
                break;

            case "RankUpReady":
                // Rank atlama konu�mas�, bir sonraki rank i�in olan�d�r.
                conversationName = $"SL_{characterName}_RankUp_{currentRank + 1}";
                break;

            case "Standard":
            default:
                // Standart konu�ma, mevcut rank i�in olan�d�r.
                conversationName = $"{characterName}_Rank_{currentRank}_StandardConversation";
                break;
        }
        if (!string.IsNullOrEmpty(conversationName))
        {
            DialogueManager.StartConversation(conversationName);
        }
        else
        {
            Debug.LogWarning($"{characterName} i�in {currentState} durumunda bir konu�ma bulunamad�!");
        }
    }

        /// <summary>
        /// Bir karakterin rank atlay�p atlamad���n� kontrol eder. 
        /// Bu fonksiyon art�k diyalog i�erisinden CSharp.Run() ile �a�r�lacak.
        /// </summary>
        /// <param name="characterName">Kontrol edilecek karakterin ad�.</param>
        public void CheckForRankUp(string characterName)
    {
        // Karakterin ayar dosyas�n� s�zl�kten bul.
        if (configDictionary.TryGetValue(characterName, out SocialLinkData config))
        {
            // G�ncel Puan ve Rank de�erlerini do�rudan Dialogue System'in haf�zas�ndan oku.
            int currentPoints = DialogueLua.GetActorField(characterName, "SocialLinkPoints").AsInt;
            int currentRank = DialogueLua.GetActorField(characterName, "SocialLinkRank").AsInt;

            // Gerekli puan dizisinin s�n�rlar�n� a��p a�mad���m�z� kontrol et.
            if (currentRank >= config.rankUpPointsNeeded.Length)
            {
                return; // Zaten maksimum rank'ta.
            }

            // Gerekli puan� ayar dosyas�ndan al.
            int pointsNeeded = config.rankUpPointsNeeded[currentRank];

            if (currentPoints >= pointsNeeded)
            {
                // RANK'I ARTIRMA! Sadece durumu "Rank Atlamaya Haz�r" olarak de�i�tir.
                DialogueLua.SetActorField(characterName, "SocialLinkState", "RankUpReady");
                Debug.LogWarning($"{characterName} rank atlamaya haz�r duruma ge�ti!");
            }
        }
        Debug.Log("31");
    }

    
    public void RankUpConversationStart(string characterName, int newRank)
    {
        // Rank atlama diyalo�unun ad�n� dinamik olarak olu�tur.
        string rankUpConversationName = $"SL_{characterName}_RankUp_{newRank}";

        // Dinamik olarak olu�turulan isimdeki konu�may� ba�lat.
        DialogueManager.StartConversation(rankUpConversationName);
    }
    public void MeetingConversationStart(string characterName)
    {
        // Rank atlama diyalo�unun ad�n� dinamik olarak olu�tur.
        string meetingConversationName = $"{characterName}_MeetingConversation";

        // Dinamik olarak olu�turulan isimdeki konu�may� ba�lat.
        DialogueManager.StartConversation(meetingConversationName);
    }
    public void StandardConversationStart(string characterName, int socialLinkRank)
    {
        // Rank atlama diyalo�unun ad�n� dinamik olarak olu�tur.
        string standardConversationName = $"{characterName}_Rank_{socialLinkRank}_StandardConversation";

        // Dinamik olarak olu�turulan isimdeki konu�may� ba�lat.
        DialogueManager.StartConversation(standardConversationName);
    }
    public int GetCharSocialLinkRank(string characterName)
    {
        int currentRank = DialogueLua.GetActorField(characterName, "SocialLinkRank").AsInt;
        return currentRank;
    }

}