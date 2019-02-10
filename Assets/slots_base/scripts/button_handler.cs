using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class button_handler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public event Action<button_handler> OnClick = delegate { };
    public event Action<button_handler> OnEnter = delegate { };
    public event Action<button_handler> OnExit = delegate { };

    private Image image;
    private AudioSource audioComponent;

    public Sprite Normal, Highlight, Click;

    void Awake()
    {
        image = GetComponent<Image>();
        audioComponent = GetComponent<AudioSource>();
    }
    void OnEnable()
    {
        image.sprite = Normal;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = Highlight;
        OnEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = Normal;
        OnExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioComponent != null)
            audioComponent.Play();
        image.sprite = Click;
        OnClick(this);
    }
}
