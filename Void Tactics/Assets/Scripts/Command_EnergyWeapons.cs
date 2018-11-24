using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vdev.Messaging;

public class Command_EnergyWeapons : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private Transform content;

    [SerializeField]
    private GameObject listEntryPrefab;

    #endregion Editor customization

    #region Private methods

    [MessageHandler(typeof(MessageBus.NextTurn))]
    [MessageHandler(typeof(MessageBus.SelectShip), AllowPartialParameters = true)]
    [MessageHandler(typeof(MessageBus.AddEnergyWeaponTarget), AllowPartialParameters = true)]
    [MessageHandler(typeof(MessageBus.RemoveEnergyWeaponTarget), AllowPartialParameters = true)]
    private void SetValues()
    {
        if (selectedShip.Value == null)
        {
            return;
        }

        var energyWeapons = selectedShip.Value.GetComponent<Ship_EnergyWeapons>();

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (var target in energyWeapons.Targets)
        {
            switch (target.tag)
            {
                case "Ship":
                    var name = target.GetComponent<Ship_Data>().Data.Name;
                    var entry = Instantiate(listEntryPrefab);
                    entry.GetComponentInChildren<TMP_Text>().text = name;
                    entry.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        MessageBus.RemoveEnergyWeaponTarget.Broadcast(target);
                    });
                    entry.transform.SetParent(content);
                    break;
            }
        }
    }

    #endregion Private methods
}