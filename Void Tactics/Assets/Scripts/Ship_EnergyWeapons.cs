using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Vdev.Messaging;

[RequireComponent(typeof(MessageAutoSubscriber))]
public class Ship_EnergyWeapons : MonoBehaviour
{
    private List<GameObject> targets = new List<GameObject>();
    private bool isSelected = false;

    public ReadOnlyCollection<GameObject> Targets { get { return targets.AsReadOnly(); } }

    [SerializeField]
    private EnergyWeapon[] energyWeapons;

    [MessageHandler(typeof(MessageBus.SelectShip))]
    private void OnSelectShip(GameObject selected)
    {
        isSelected = selected == gameObject;
    }

    [MessageHandler(typeof(MessageBus.AddEnergyWeaponTarget))]
    private void OnAddTarget(GameObject target)
    {
        Debug.LogFormat("[{0}] is{1} selected.", gameObject.name, isSelected ? "" : " not");
        if (isSelected && !targets.Contains(target))
        {
            Debug.LogFormat("[{0}] will target {1}.", gameObject.name, target.name);
            targets.Add(target);
            foreach (var w in energyWeapons)
            {
                w.Targets.Add(target);
            }
        }
    }

    [MessageHandler(typeof(MessageBus.RemoveEnergyWeaponTarget))]
    private void OnRemoveTarget(GameObject target)
    {
        if (isSelected)
        {
            targets.Remove(target);
            foreach (var w in energyWeapons)
            {
                w.Targets.Remove(target);
            }
        }
    }

    [MessageHandler(typeof(MessageBus.NextTurn))]
    private void ClearTargets()
    {
        targets.Clear();
        foreach (var w in energyWeapons)
        {
            w.Targets.Clear();
        }
    }

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