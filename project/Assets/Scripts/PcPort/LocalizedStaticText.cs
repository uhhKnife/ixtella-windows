// Only swaps the text while it's still the original string, so runtime-set text is left alone.
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedStaticText : MonoBehaviour
{
    [SerializeField]
    private string japanese_;

    [SerializeField]
    private string english_;

    public void Setup(string japanese, string english)
    {
        japanese_ = japanese;
        english_ = english;
    }

    private void Awake()
    {
        Apply();
    }

    private void OnEnable()
    {
        Apply();
    }

    private void Start()
    {
        Apply();
    }

    public void Apply()
    {
        Text text = GetComponent<Text>();
        if (text == null || string.IsNullOrEmpty(english_))
        {
            return;
        }
        if (Localization.IsEnglish)
        {
            if (text.text == japanese_)
            {
                text.text = english_;
            }
        }
        else if (text.text == english_)
        {
            text.text = japanese_;
        }
    }
}
