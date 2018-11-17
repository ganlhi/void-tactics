using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayersManager : MonoBehaviourPunCallbacks
{
    #region Private variables

    private Dictionary<int, bool> playersReady = new Dictionary<int, bool>();

    [SerializeField]
    private BoolGameEvent runningTurnEvent;

    #endregion Private variables

    #region Public methods

    public void OnPlayerReady(bool ready)
    {
        Debug.Log("Local Player Ready: " + ready);
        photonView.RPC("RPC_SetPlayerReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, ready);
    }

    #endregion Public methods

    #region Private methods

    [PunRPC]
    private void RPC_SetPlayerReady(int actorNum, bool ready)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Debug.LogFormat("Player {0} Ready: {1}", actorNum, ready);
        playersReady[actorNum] = ready;
        CheckPlayersReady();
    }

    private void CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (playersReady.Values.All(ready => ready))
        {
            runningTurnEvent.Raise(true);
        }
    }

    #endregion Private methods

    #region Unity callbacks

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true;
        }
    }

    private void Update()
    {
    }

    #endregion Unity callbacks

    #region Photon callbacks

    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.CreateRoom("OfflineRoom");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Player in room: " + PhotonNetwork.LocalPlayer.ActorNumber);
        if (PhotonNetwork.IsMasterClient)
        {
            playersReady.Add(PhotonNetwork.LocalPlayer.ActorNumber, false);
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log("Player Left: " + player.ActorNumber);
        if (PhotonNetwork.IsMasterClient)
        {
            playersReady.Add(player.ActorNumber, false);
        }
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        Debug.Log("Player Entered: " + player.ActorNumber);
        if (PhotonNetwork.IsMasterClient)
        {
            playersReady.Remove(player.ActorNumber);
            CheckPlayersReady();
        }
    }

    #endregion Photon callbacks
}