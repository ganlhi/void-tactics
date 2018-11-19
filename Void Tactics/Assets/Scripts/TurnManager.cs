using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vdev.Messaging;

[RequireComponent(typeof(PhotonView)), RequireComponent(typeof(MessageAutoSubscriber))]
public class TurnManager : MonoBehaviourPunCallbacks
{
    #region Private variables

    private Dictionary<int, bool> playersReady = new Dictionary<int, bool>();

    [SerializeField]
    private IntVariable currentTurn;

    [SerializeField]
    private BoolVariable runningTurn;

    [SerializeField]
    private IntVariable turnDuration;

    [SerializeField]
    private IntVariable speedFactor;

    private int currentTurnDuration
    {
        get
        {
            return turnDuration / speedFactor;
        }
    }

    #endregion Private variables

    #region Private methods

    [MessageHandler(typeof(MessageBus.PlayerReady))]
    private void OnPlayerReady(bool ready)
    {
        Debug.Log("Local Player Ready: " + ready);
        photonView.RPC("RPC_SetPlayerReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, ready);
    }

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
            photonView.RPC("RPC_RunTurn", RpcTarget.All, !runningTurn.Value);
        }
    }

    [PunRPC]
    private void RPC_RunTurn(bool run)
    {
        runningTurn.Value = run;
        MessageBus.RunningTurn.Broadcast(run);

        if (run)
        {
            StartCoroutine("WaitTurnEnd");
        }
        else
        {
            currentTurn.Value++;
            MessageBus.NextTurn.Broadcast();
        }
    }

    private IEnumerator WaitTurnEnd()
    {
        OnPlayerReady(false);

        var duration = currentTurnDuration + .5f;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        OnPlayerReady(true);
    }

    [PunRPC]
    private void RPC_SetTurnNumber(int num)
    {
        currentTurn.Value = num;
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

    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_SetTurnNumber", RpcTarget.All, 1);
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