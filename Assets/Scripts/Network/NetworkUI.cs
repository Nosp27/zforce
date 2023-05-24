using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : NetworkBehaviour
{
    [SerializeField] private Button startClient;
    [SerializeField] private InputField clientServerAddress;
    [SerializeField] private Button startHost;
    [SerializeField] private Text ping;

    private NetworkManager netManager;
    // Start is called before the first frame update
    void Start()
    {
        netManager = FindObjectOfType<NetworkManager>();
        startClient.onClick.AddListener(() =>
        {
            try
            {
                var (host, port) = ExtractAddressAndPort();
                print($"{host}, {port}");
                netManager.GetComponent<UnityTransport>().SetConnectionData(host, port);
                netManager.GetComponent<UnityTransport>().ConnectionData.Port = port;
                netManager.StartClient();
                DisableButtons();
            }
            catch (Exception e)
            {
                ColorBlock colors = startClient.colors;
                colors.normalColor = Color.red;
                startClient.colors = colors;
            }
        });
        startHost.onClick.AddListener(() => { netManager.StartHost(); DisableButtons();});
    }

    Tuple<string, ushort> ExtractAddressAndPort()
    {
        string raw = clientServerAddress.text;
        string[] parts = raw.Split(':');
        return new Tuple<string, ushort>(parts[0], ushort.Parse(parts[1]));
    }

    void DisableButtons()
    {
        startClient.gameObject.SetActive(false);
        startHost.gameObject.SetActive(false);
        clientServerAddress.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (IsClient)
        {
            ulong pingMs = netManager.GetComponent<NetworkTransport>().GetCurrentRtt(OwnerClientId);
            ping.text = pingMs.ToString();
        }
    }
}
