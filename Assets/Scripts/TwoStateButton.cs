using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TwoStateButton : MonoBehaviour
{
    [SerializeField] private Color _normalColor = Color.yellow;
    [SerializeField] private string _normalText = "Claim";
    [Space(10)]
    [SerializeField] private Color _negativeColor = Color.gray;
    [SerializeField] private string _negativeText = "Claimed";

    public void SetState(bool normal)
    {
        var button = GetComponent<Button>();
        var text = button.GetComponentInChildren<TextMeshProUGUI>();

        if (normal)
        {
            button.image.color = _normalColor;
            text.text = _normalText;
        }
        else
        {
            button.image.color = _negativeColor;
            text.text = _negativeText;
        }
    }

}
