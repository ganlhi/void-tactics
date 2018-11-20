using UnityEngine;
using Vdev.Messaging;

[RequireComponent(typeof(MessageAutoSubscriber))]
public class Command_Plotting : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private AxisPlotting yaw;

    [SerializeField]
    private AxisPlotting pitch;

    [SerializeField]
    private AxisPlotting roll;

    [SerializeField]
    private ThrustPlotting thrust;

    #endregion Editor customization

    #region Private methods

    [MessageHandler(typeof(MessageBus.NextTurn))]
    [MessageHandler(typeof(MessageBus.SelectShip))]
    private void SetValues()
    {
        if (selectedShip.Value == null)
        {
            return;
        }

        var ship = selectedShip.Value.GetComponent<Ship_Data>().Data;
        var shipMov = selectedShip.Value.GetComponent<Ship_Movement>();

        yaw.SetValues(ship.MaxYaw, shipMov.PlottedYaw);
        pitch.SetValues(ship.MaxPitch, shipMov.PlottedPitch);
        roll.SetValues(ship.MaxRoll, shipMov.PlottedRoll);
        thrust.SetValues(ship.MaxThrust, shipMov.Thrust);
    }

    #endregion Private methods
}