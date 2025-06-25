using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween'i kullanmak i�in bu sat�r �art!
using System.Collections.Generic;
using PixelCrushers.DialogueSystem; // Lua'ya fonksiyon kaydetmek i�in gerekli.

public class SocialLinkFeedbackManager : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private GameObject pointIconPrefab; // Project'ten olu�turdu�umuz prefab
    [SerializeField] private RectTransform targetUI; // Sol �stteki hedefimiz
    [SerializeField] private Transform spawnPoint; // �konlar�n do�aca�� nokta (Diyalog kutusu olabilir)
    //[SerializeField] private AudioClip pointSound; // Puan kazanma sesi

    //private AudioSource audioSource;
    public List<Image> iconPool = new List<Image>(); // Performans i�in object pooling

    private void Awake()
    {
        // TODO Ses i�in AudioSource component'ini al veya ekle.
        
    }

    private void OnEnable()
    {
        // C# fonksiyonumuzu Lua ortam�na kaydediyoruz. B�ylece diyalog i�inden do�rudan �a��rabilece�iz.
        // Lua, say�lar� double olarak kulland��� i�in parametre t�r� double olmal�.
        Lua.RegisterFunction("PlayPointFeedback", this, SymbolExtensions.GetMethodInfo(() => PlayPointFeedback(default(double))));
    }

    private void OnDisable()
    {
        // Unutmamak i�in, obje pasif oldu�unda kayd� silelim.
        Lua.UnregisterFunction("PlayPointFeedback");
    }

    // Lua'dan �a�r�lacak olan ana fonksiyonumuz.
    public void PlayPointFeedback(double amount)
    {
        int iconCount = (int)amount;
        StartCoroutine(SpawnIconsRoutine(iconCount));
    }

    private System.Collections.IEnumerator SpawnIconsRoutine(int count)
    {
        // Belirtilen say�da ikonu, aralar�nda k���k bir gecikmeyle olu�tur ve hareket ettir.
        for (int i = 0; i < count; i++)
        {
            Image icon = GetIconFromPool();
            Color iconColor = icon.color;
            iconColor.a = 1f;
            icon.color = iconColor;
            icon.gameObject.SetActive(true);
            icon.transform.position = spawnPoint.position;
            icon.transform.localScale = Vector3.zero; // Ba�lang��ta g�r�nmez olsun

            // DOTween ile animasyon zinciri olu�turuyoruz.
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(icon.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack)); // Ekranda belirme efekti
            mySequence.Append(icon.transform.DOMove(targetUI.position, 1.2f).SetEase(Ease.InOutCubic)); // Hedefe do�ru hareket
            mySequence.Join(icon.DOFade(0f, 0.5f).SetDelay(0.7f)); // Hedefe yakla��rken yava��a kaybolma
            mySequence.OnComplete(() =>
            {
                // Animasyon bitince ikonu havuza geri g�nder.
                icon.gameObject.SetActive(false);
            });

            // TODO Ses efektini �al.
            

            // Bir sonraki ikonu olu�turmadan �nce k�sa bir s�re bekle.
            yield return new WaitForSeconds(0.15f);
        }
    }

    // Havuzdan kullan�labilir bir ikon al�r veya yenisini yarat�r.
    private Image GetIconFromPool()
    {
        foreach (var icon in iconPool)
        {
            if (!icon.gameObject.activeInHierarchy)
            {
                return icon;
            }

        }

        // Havuzda uygun ikon yoksa, yenisini olu�tur.
        GameObject newIconObj = Instantiate(pointIconPrefab, this.transform);
        Image newIcon = newIconObj.GetComponent<Image>();
        iconPool.Add(newIcon);
        return newIcon;
    }
}