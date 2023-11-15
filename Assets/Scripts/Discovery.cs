using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class Discovery : MonoBehaviour
{
    [SerializeField] int Port = 42069;
    [SerializeField] string Request = "Ola";
    [SerializeField] TMPro.TMP_Dropdown adaptersDropdown;

    public UnityEvent<UdpReceiveResult> serverFound;
    public static List<UdpClient> clients = new();
    //Currently selected client
    UdpClient client = null;
    IPEndPoint BroadcastEp;
    byte[] RequestData;
    
    // Start is called before the first frame update
    void Start()
    {
        BroadcastEp = new IPEndPoint(IPAddress.Broadcast, Port);
        RequestData = Encoding.ASCII.GetBytes(Request);
        ReadAdapters();
        adaptersDropdown.onValueChanged.AddListener(SetAdapter);
    }

    private void ReadAdapters()
    {
        List<string> addresses = new();
        IPHostEntry hostEntry=Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList) {
            if (ip.AddressFamily==AddressFamily.InterNetwork) {
                addresses.Add(ip.ToString());
                Debug.Log(ip);
                UdpClient tmp = new UdpClient(new IPEndPoint(IPAddress.Parse(ip.ToString()), Port))
                {
                    EnableBroadcast = true
                };
                tmp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                tmp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
                clients.Add(tmp);
            }
        }

        adaptersDropdown.ClearOptions();
        adaptersDropdown.AddOptions(addresses);
        client = clients[0];
    }

/// <summary>
/// Called when the user selects a new network adapter
/// </summary>
/// <param name="ipAddr">The IP address of the adapter</param>
    public void SetAdapter(int indx)
    {
        client = clients[indx];
    }

    public void FindServer()
    {
        FindServerAsync();
    }

    private async void FindServerAsync()
    {
        if(client != null)
        {
            Debug.Log($"Sending broadcast to port {Port}");
            await client.SendAsync(RequestData, RequestData.Length, BroadcastEp);

            UdpReceiveResult result = await client.ReceiveAsync();

            Debug.Log($"Server: {result.RemoteEndPoint}");
            Debug.Log($"Data: {Encoding.ASCII.GetString(result.Buffer)}");
            serverFound.Invoke(result);
        }
        else
        {
            Debug.LogError("Client is not defined, pick an adapter");
        }
    }
}