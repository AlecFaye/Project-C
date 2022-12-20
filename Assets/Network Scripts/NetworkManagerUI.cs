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

    private void Awake() {
        ServerBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        HostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        ClientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }
}
