using UnityEngine;
using System.Collections; // Coroutine'ler i�in bu using ifadesi gereklidir!

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSortOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Sorting Settings")]
    [Tooltip("Y pozisyonunu Order in Layer'a �evirirken kullan�lacak �arpan.")]
    [SerializeField] private int sortingOrderMultiplier = -100;

    [Tooltip("Sprite'�n pivot noktas�na g�re dikey bir ofset ekler.")]
    [SerializeField] private float yOffset = 0;

    [Header("Performance")]
    [Tooltip("S�ralaman�n saniyede ka� kez g�ncellenece�i. 10, �o�u durum i�in ak�c� bir de�erdir.")]
    [SerializeField] private float updatesPerSecond = 10f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Bu component aktif oldu�unda Coroutine'i ba�lat.
    private void OnEnable()
    {
        StartCoroutine(UpdateSortingOrder());
    }
    private void OnDisable()
    {
        StopCoroutine(UpdateSortingOrder());
    }



    /// <summary>
    /// Belirlenen aral�klarla kendini tekrar eden ve sortingOrder'� g�ncelleyen ana d�ng�.
    /// </summary>
    private IEnumerator UpdateSortingOrder()
    {
        // Sonsuz bir d�ng� ba�lat�yoruz. Script pasif hale gelince veya yok edilince d�ng� otomatik olarak durur.
        while (true)
        {
            // Ana s�ralama mant���m�z burada.
            spriteRenderer.sortingOrder = (int)((transform.position.y + yOffset) * sortingOrderMultiplier);

            // Bir sonraki g�ncellemeye kadar bekle.
            // �rne�in, updatesPerSecond=10 ise, 1/10 = 0.1 saniye bekler.
            yield return new WaitForSeconds(1f / updatesPerSecond);
        }
    }
}