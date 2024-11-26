using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoSingleton<MessageBox>
{
    public TMP_Text Text;
    public Button btn;
    public GameObject box;

    public void Show(string text)
    {
        box.SetActive(true);        
        Text.text = text;
    }
    public void Hide()
    {
        box.SetActive(false);
    }
}