using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextUpdater : MonoBehaviour
{
    private TMP_Text textObj;

    void Awake()
    {
        textObj = GetComponent<TMP_Text>();
    }

    public void ShowInt(int value)
    {
        textObj.text = value.ToString();
    }

    public void ShowFloat(float value)
    {
        textObj.text = value.ToString();
    }

    public void ShowString(string value)
    {
        textObj.text = value;
    }
}
