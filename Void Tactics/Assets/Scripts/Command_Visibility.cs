using UnityEngine;
using Vdev.Messaging;

[RequireComponent(typeof(MessageAutoSubscriber))]
public class Command_Visibility : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private BoolVariable runningTurn;

    #endregion Editor customization

    #region Private methods

    [MessageHandler(typeof(MessageBus.SelectShip), AllowPartialParameters = true)]
    [MessageHandler(typeof(MessageBus.RunningTurn), AllowPartialParameters = true)]
    public void UpdateVisibility()
    {
        var rect = GetComponent<RectTransform>();
        if (selectedShip.Value != null && runningTurn.Value == false)
        {
            rect.position = new Vector3(0, 0, 0);
        }
        else
        {
            rect.position = new Vector3(-9999, 0, 0);
        }
    }

    #endregion Private methods

    #region Unity callbacks

    private void Start()
    {
        UpdateVisibility();
    }

    #endregion Unity callbacks
}