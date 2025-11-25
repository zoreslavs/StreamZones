using UnityEngine;
using TMPro;

public class LoadingIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;

    public void Show(string message)
    {
        if (_label)
            _label.text = message;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (_label)
            _label.text = string.Empty;

        gameObject.SetActive(false);
    }
}