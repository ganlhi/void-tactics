using UnityEngine;
using UnityEngine.UI;

public class Toolbar_MarkerPos : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private IntVariable turnDuration;

    [SerializeField]
    private IntVariable markerPosIndex;

    [SerializeField]
    private Slider slider;

    #endregion Editor customization

    #region Private methods

    private void SetMarkerPos(float value)
    {
        markerPosIndex.Value = Mathf.Clamp((int)value, 0, turnDuration - 1);
    }

    #endregion Private methods

    #region Unity callbacks

    private void Start()
    {
        slider.maxValue = turnDuration - 1;
        slider.value = Mathf.Clamp(markerPosIndex.Value, 0, turnDuration - 1);
        slider.onValueChanged.AddListener(SetMarkerPos);
    }

    #endregion Unity callbacks
}