// Gives mouse hover and keyboard navigation one shared button highlight.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class UIButtonFocus : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        ColorBlock colors = button.colors;
        colors.selectedColor = colors.highlightedColor;
        colors.highlightedColor = colors.normalColor;
        colors.fadeDuration = 0f;
        button.colors = colors;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        FocusButton();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.delta.sqrMagnitude > 0f) FocusButton();
    }

    private void FocusButton()
    {
        if (button.IsActive() && button.IsInteractable()) button.Select();
    }
}