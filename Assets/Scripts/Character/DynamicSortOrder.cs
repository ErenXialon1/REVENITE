using UnityEngine;
using System.Collections; // Coroutine'ler için bu using ifadesi gereklidir!

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSortOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Sorting Settings")]
    [Tooltip("Y pozisyonunu Order in Layer'a çevirirken kullanýlacak çarpan.")]
    [SerializeField] private int sortingOrderMultiplier = -100;

    [Tooltip("Sprite'ýn pivot noktasýna göre dikey bir ofset ekler.")]
    [SerializeField] private float yOffset = 0;

    [Header("Performance")]
    [Tooltip("Sýralamanýn saniyede kaç kez güncelleneceði. 10, çoðu durum için akýcý bir deðerdir.")]
    [SerializeField] private float updatesPerSecond = 10f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Bu component aktif olduðunda Coroutine'i baþlat.
    private void OnEnable()
    {
        StartCoroutine(UpdateSortingOrder());
    }
    private void OnDisable()
    {
        StopCoroutine(UpdateSortingOrder());
    }



    /// <summary>
    /// Belirlenen aralýklarla kendini tekrar eden ve sortingOrder'ý güncelleyen ana döngü.
    /// </summary>
    private IEnumerator UpdateSortingOrder()
    {
        // Sonsuz bir döngü baþlatýyoruz. Script pasif hale gelince veya yok edilince döngü otomatik olarak durur.
        while (true)
        {
            // Ana sýralama mantýðýmýz burada.
            spriteRenderer.sortingOrder = (int)((transform.position.y + yOffset) * sortingOrderMultiplier);

            // Bir sonraki güncellemeye kadar bekle.
            // Örneðin, updatesPerSecond=10 ise, 1/10 = 0.1 saniye bekler.
            yield return new WaitForSeconds(1f / updatesPerSecond);
        }
    }
}