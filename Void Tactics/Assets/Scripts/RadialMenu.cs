using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Vdev.Messaging;

[RequireComponent(typeof(MessageAutoSubscriber))]
public class RadialMenu : MonoBehaviour
{
    #region Private variables

    private GameObject menu;
    private GameObject menuTarget;
    private new CameraOrbit camera;

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private Button viewButton;

    [SerializeField]
    private Button targetButton;

    [SerializeField]
    private Button missileButton;

    #endregion Private variables

    #region Private methods

    [MessageHandler(typeof(MessageBus.RadialMenu))]
    private void OnRadialMenu(GameObject target)
    {
        menuTarget = target;
        menu.SetActive(target != null);

        RectTransform rt = menu.GetComponent<RectTransform>();

        if (target != null)
        {
            rt.position = Camera.main.WorldToScreenPoint(target.transform.position);
            SetupButtons();
        }
    }

    private void SetupButtons()
    {
        // View button
        var isCamTarget = menuTarget.transform == camera.Target;
        var isSelectedShip = menuTarget == selectedShip.Value;
        var viewNegImage = viewButton.transform.Find("NegImage").gameObject;

        viewButton.interactable = !isSelectedShip || (isSelectedShip && !isCamTarget);
        viewNegImage.SetActive(isCamTarget && viewButton.interactable);

        // Target and Missile buttons
        var tgtNegImage = targetButton.transform.Find("NegImage").gameObject;
        var mslNegImage = missileButton.transform.Find("NegImage").gameObject;

        targetButton.interactable = false;
        missileButton.interactable = false;
        tgtNegImage.SetActive(false);
        mslNegImage.SetActive(false);

        if (menuTarget.tag == "Ship")
        {
            var shipData = menuTarget.GetComponent<Ship_Data>().Data;
            var energyWeapons = selectedShip.Value.GetComponent<Ship_EnergyWeapons>();
            var isEnnemy = shipData.Owner != PhotonNetwork.LocalPlayer.ActorNumber;
            var isEnergyWeaponsTarget = energyWeapons.Targets.Contains(menuTarget);

            // Target button
            targetButton.interactable = isEnnemy;
            tgtNegImage.SetActive(isEnergyWeaponsTarget);

            // Missile button
            // TODO
        }
    }

    private void OnViewButton()
    {
    }

    private void OnTargetButton()
    {
        if (menuTarget.tag == "Ship")
        {
            var energyWeapons = selectedShip.Value.GetComponent<Ship_EnergyWeapons>();
            var isEnergyWeaponsTarget = energyWeapons.Targets.Contains(menuTarget);

            if (isEnergyWeaponsTarget)
            {
                MessageBus.RemoveEnergyWeaponTarget.Broadcast(menuTarget);
            }
            else
            {
                MessageBus.AddEnergyWeaponTarget.Broadcast(menuTarget);
            }

            MessageBus.RadialMenu.Broadcast(null);
        }
    }

    private void OnMissileButton()
    {
    }

    #endregion Private methods

    #region Unity callbacks

    private void Start()
    {
        menu = transform.GetChild(0).gameObject;
        menu.SetActive(false);
        camera = Camera.main.GetComponent<CameraOrbit>();

        viewButton.onClick.AddListener(OnViewButton);
        targetButton.onClick.AddListener(OnTargetButton);
        missileButton.onClick.AddListener(OnMissileButton);
    }

    private void Update()
    {
    }

    #endregion Unity callbacks
}