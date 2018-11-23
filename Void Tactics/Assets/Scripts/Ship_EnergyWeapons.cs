using UnityEngine;
using Vdev.Messaging;

[RequireComponent(typeof(MessageAutoSubscriber))]
public class Ship_EnergyWeapons : MonoBehaviour
{
    [SerializeField]
    private EnergyWeapon[] energyWeapons;

    [MessageHandler(typeof(MessageBus.RunningTurn))]
    private void HandleWeapons(bool running)
    {
        foreach (var w in energyWeapons)
        {
            if (running)
            {
                w.StartTrackingTargets();
            }
            else
            {
                w.StopTrackingTargets();
            }
        }
    }
}