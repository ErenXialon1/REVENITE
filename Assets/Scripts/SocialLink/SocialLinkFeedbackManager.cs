using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween'i kullanmak için bu satýr þart!
using System.Collections.Generic;
using PixelCrushers.DialogueSystem; // Lua'ya fonksiyon kaydetmek için gerekli.

public class SocialLinkFeedbackManager : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private GameObject pointIconPrefab; // Project'ten oluþturduðumuz prefab
    [SerializeField] private RectTransform targetUI; // Sol üstteki hedefimiz
    [SerializeField] private Transform spawnPoint; // Ýkonlarýn doðacaðý nokta (Diyalog kutusu olabilir)
    //[SerializeField] private AudioClip pointSound; // Puan kazanma sesi

    //private AudioSource audioSource;
    public List<Image> iconPool = new List<Image>(); // Performans için object pooling

    private void Awake()
    {
        // TODO Ses için AudioSource component'ini al veya ekle.
        
    }

    private void OnEnable()
    {
        // C# fonksiyonumuzu Lua ortamýna kaydediyoruz. Böylece diyalog içinden doðrudan çaðýrabileceðiz.
        // Lua, sayýlarý double olarak kullandýðý için parametre türü double olmalý.
        Lua.RegisterFunction("PlayPointFeedback", this, SymbolExtensions.GetMethodInfo(() => PlayPointFeedback(default(double))));
    }

    private void OnDisable()
    {
        // Unutmamak için, obje pasif olduðunda kaydý silelim.
        Lua.UnregisterFunction("PlayPointFeedback");
    }

    // Lua'dan çaðrýlacak olan ana fonksiyonumuz.
    public void PlayPointFeedback(double amount)
    {
        int iconCount = (int)amount;
        StartCoroutine(SpawnIconsRoutine(iconCount));
    }

    private System.Collections.IEnumerator SpawnIconsRoutine(int count)
    {
        // Belirtilen sayýda ikonu, aralarýnda küçük bir gecikmeyle oluþtur ve hareket ettir.
        for (int i = 0; i < count; i++)
        {
            Image icon = GetIconFromPool();
            Color iconColor = icon.color;
            iconColor.a = 1f;
            icon.color = iconColor;
            icon.gameObject.SetActive(true);
            icon.transform.position = spawnPoint.position;
            icon.transform.localScale = Vector3.zero; // Baþlangýçta görünmez olsun

            // DOTween ile animasyon zinciri oluþturuyoruz.
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(icon.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack)); // Ekranda belirme efekti
            mySequence.Append(icon.transform.DOMove(targetUI.position, 1.2f).SetEase(Ease.InOutCubic)); // Hedefe doðru hareket
            mySequence.Join(icon.DOFade(0f, 0.5f).SetDelay(0.7f)); // Hedefe yaklaþýrken yavaþça kaybolma
            mySequence.OnComplete(() =>
            {
                // Animasyon bitince ikonu havuza geri gönder.
                icon.gameObject.SetActive(false);
            });

            // TODO Ses efektini çal.
            

            // Bir sonraki ikonu oluþturmadan önce kýsa bir süre bekle.
            yield return new WaitForSeconds(0.15f);
        }
    }

    // Havuzdan kullanýlabilir bir ikon alýr veya yenisini yaratýr.
    private Image GetIconFromPool()
    {
        foreach (var icon in iconPool)
        {
            if (!icon.gameObject.activeInHierarchy)
            {
                return icon;
            }

        }

        // Havuzda uygun ikon yoksa, yenisini oluþtur.
        GameObject newIconObj = Instantiate(pointIconPrefab, this.transform);
        Image newIcon = newIconObj.GetComponent<Image>();
        iconPool.Add(newIcon);
        return newIcon;
    }
}