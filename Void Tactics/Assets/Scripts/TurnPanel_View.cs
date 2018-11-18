using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vdev.Messaging;

[RequireComponent(typeof(MessageAutoSubscriber))]
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

    [MessageHandler(typeof(MessageBus.NextTurn))]
    public void UpdateNumber()
    {
        text.text = currentTurn.Value.ToString();
    }

    [MessageHandler(typeof(MessageBus.RunningTurn))]
    public void UpdateButton(bool running)
    {
        button.interactable = !running;
    }

    public void SetReady()
    {
        MessageBus.PlayerReady.Broadcast(true);
    }

    #endregion Public methods

    #region Unity callbacks

    private void Start()
    {
        UpdateNumber();
    }

    #endregion Unity callbacks
}