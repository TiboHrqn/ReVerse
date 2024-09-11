using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{

    // Référence au composant TextMeshPro
    public TextMeshProUGUI textMeshPro;

    public void DisplayTime( float time)
    {
        textMeshPro.text = time.ToString("F3");
    }
}
