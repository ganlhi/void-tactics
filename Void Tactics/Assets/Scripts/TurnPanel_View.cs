using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanel_View : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private IntVariable currentTurn;

    [SerializeField]
    private Button button;

    [SerializeField]
    private TMP_Text text;

    #endregion Editor customization

    #region Public methods

    public void UpdateNumber()
    {
        text.text = currentTurn.Value.ToString();
    }

    public void UpdateButton(bool running)
    {
        button.interactable = !running;
    }

    #endregion Public methods

    #region Unity callbacks

    private void Start()
    {
        UpdateNumber();
    }

    #endregion Unity callbacks
}