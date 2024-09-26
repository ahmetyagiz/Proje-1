using TMPro;
using UnityEngine;

/// <summary>
/// Bu kod InputField alan�n� y�netir.
/// </summary>
public class InputFieldManager : MonoBehaviour
{
    public TMP_InputField inputField;

    public int ReadLastInput()
    {
        if (int.TryParse(inputField.text, out int size))
        {
            return size;
        }
        return 5; // Varsay�lan de�er
    }
}