using TMPro;
using UnityEngine;

public class InputFieldManager : MonoBehaviour
{
    public TMP_InputField inputField;

    public int ReadLastInput()
    {
        if (int.TryParse(inputField.text, out int size))
        {
            return size;
        }
        return 5; // Varsayýlan deðer
    }
}