using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
public class IconManager : MonoBehaviour
{
    public Image interactionIcon;
    public Vector3 offset = new Vector3(0, 2f, 0);
    private Transform currentTarget;

    void Start()
    {
        if (interactionIcon != null) interactionIcon.enabled = false;
    }

    void LateUpdate()
    {
        if (currentTarget != null && interactionIcon.enabled)
        {
            interactionIcon.transform.position = Camera.main.WorldToScreenPoint(currentTarget.position + offset);
        }
    }

    public void ShowIcon(Usable selectedUsable)
    {
        if (selectedUsable != null)
        {
            currentTarget = selectedUsable.transform;
            interactionIcon.enabled = true;

        }
    }

    public void HideIcon()
    {
        currentTarget = null;
        if (interactionIcon != null) interactionIcon.enabled = false;
    }
}
