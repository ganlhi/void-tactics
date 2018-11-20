using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThrustPlotting : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TMP_Text value;

    #endregion Editor customization

    #region Public methods

    public void SetValues(float max, float cur)
    {
        slider.minValue = 0;
        slider.maxValue = max;
        slider.value = cur;

        HandleValue(true);
    }

    #endregion Public methods

    #region Private methods

    public void HandleValue(bool silent = false)
    {
        var amount = Mathf.Round(slider.value * 10) / 10;
        slider.value = amount;
        value.text = amount.ToString() + " g";

        if (!silent)
        {
            MessageBus.PlotThrust.Broadcast(amount);
        }
    }

    #endregion Private methods
}