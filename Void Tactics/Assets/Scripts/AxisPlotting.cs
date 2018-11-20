using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AxisPlotting : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private ManeuverAxis axis;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Image needle;

    [SerializeField]
    private Image positiveDirection;

    [SerializeField]
    private Image negativeDirection;

    [SerializeField]
    private TMP_Text value;

    [SerializeField]
    private bool clockwiseIsNegative;

    #endregion Editor customization

    #region Public methods

    public void SetValues(int max, int cur)
    {
        slider.minValue = -1 * max;
        slider.maxValue = max;
        slider.value = cur;

        HandleValue(true);
    }

    #endregion Public methods

    #region Private methods

    public void HandleValue(bool silent = false)
    {
        var amount = Mathf.Abs(slider.value);
        needle.fillAmount = amount / 360;
        needle.fillClockwise = clockwiseIsNegative ? slider.value < 0 : slider.value > 0;
        value.text = amount.ToString();
        value.enabled = slider.value != 0;
        positiveDirection.enabled = slider.value > 0;
        negativeDirection.enabled = slider.value < 0;

        if (!silent)
        {
            MessageBus.Plot.Broadcast(axis, (int)slider.value);
        }
    }

    #endregion Private methods
}