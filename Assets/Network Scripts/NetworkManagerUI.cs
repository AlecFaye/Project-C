using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button ServerBtn;
    [SerializeField] private Button HostBtn;
    [SerializeField] private Button ClientBtn;

    [SerializeField] private GameTimer GameTimer;

    private void Awake() {
        ServerBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            GameTimer.IsTimerStarted = true;
        });
        HostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            GameTimer.IsTimerStarted = true;
        });
        ClientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            GameTimer.IsTimerStarted = true;
        });
    }
}
