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
        Lua.RegisterFunction("CheckForRankUp", this, SymbolExtensions.GetMethodInfo(() => CheckForRankUp(default(string))));
    }
    private void OnDisable()
    {
        Lua.UnregisterFunction("CheckForRankUp");
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

            // Rank atlama ko�ulu sa�lan�yor mu?
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
        Debug.LogWarning(characterName + " ile sosyal ba��n Rank " + newRank + " seviyesine Y�KSELD�!");

        
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