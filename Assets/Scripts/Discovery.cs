using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class Discovery : MonoBehaviour
{
    [SerializeField] int BroadcastPort = 42069;
    [SerializeField] string Request = "Ola";
    [SerializeField] TMPro.TMP_Dropdown adaptersDropdown;

    public UnityEvent<UdpReceiveResult> serverFound;
    byte[] RequestData;
    
    // Start is called before the first frame update
    void Start()
    {
        RequestData = Encoding.ASCII.GetBytes(Request);
        ReadAdapters();
        adaptersDropdown.onValueChanged.AddListener(SetAdapter);

        BroadcastResponseListen();
    }

    private void ReadAdapters()
    {
        IPHostEntry hostEntry=Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList) {
            if (ip.AddressFamily==AddressFamily.InterNetwork) {
                UdpClient tmp = new UdpClient(new IPEndPoint(ip, BroadcastPort))
                {
                    EnableBroadcast = true,
                };
                tmp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                tmp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
                tmp.Client.ReceiveTimeout = 1000;

                Clients.adapterAddresses.Add(ip);
                byte[] ipBytes = ip.GetAddressBytes();
                ipBytes[3] = 255;
                IPEndPoint ep = new IPEndPoint(new IPAddress(ipBytes), BroadcastPort);
                Clients.adapters.Add(ip, new Tuple<UdpClient, IPEndPoint>(tmp, ep));
            }
        }

        adaptersDropdown.ClearOptions();
        adaptersDropdown.AddOptions(Clients.adapterAddresses.ConvertAll(ip => ip.ToString()));
        Clients.currentAdapter = Clients.adapters[Clients.adapterAddresses[0]];
    }

/// <summary> Called when the user selects a new network adapter </summary>
/// <param name="indx"> The index of the network adapter's IP address in the list of adapter IP's </param>
    public void SetAdapter(int indx)
    {
        IPAddress addr = Clients.adapterAddresses[indx];
        Clients.currentAdapter = Clients.adapters[addr];
    }

    public void BroadcastRequest()
    {
        BroadcastRequestAsync();
    }

    private async void BroadcastRequestAsync()
    {
        if(Clients.currentAdapter != null)
        {
            for(byte i = 1; i < 255; i++)
            {
                byte[] ipBytes = Clients.currentAdapter.Item2.Address.GetAddressBytes();
                ipBytes[3] = i;
                IPAddress addr = new(ipBytes);
                IPEndPoint ep = new IPEndPoint(addr, BroadcastPort);
                Debug.Log($"Sending broadcast to {ep} from {Clients.currentAdapter.Item1.Client.LocalEndPoint}");
                await Clients.currentAdapter.Item1.SendAsync(RequestData, RequestData.Length, ep);
            }
        }
        else
        {
            Debug.LogError("Client is not defined, pick an adapter");
        }
    }

/// <summary> Listens on the currently selected adapter for a response to a broadcast request. </summary>
    private async void BroadcastResponseListen()
    {
        while(true)
        {
            try
            {
                Debug.Log("Awaiting");
                UdpReceiveResult result = await Clients.currentAdapter.Item1.ReceiveAsync();
                string response = Encoding.ASCII.GetString(result.Buffer);

                Debug.Log($"Server: {result.RemoteEndPoint}");
                Debug.Log($"Data (The connection port): {response}");

                //Ignore own broadcast
                if(response == Request) continue;

                IPAddress ip = result.RemoteEndPoint.Address;
                int port = int.Parse(Encoding.ASCII.GetString(result.Buffer));

                if(!Clients.serverAddresses.Contains(ip))
                {
                    UdpClient serverClient = new UdpClient();
                    serverClient.Connect(ip, port);

                    Clients.serverAddresses.Add(ip);
                    Clients.servers.Add(ip, serverClient);
                    if(Clients.currentServer == null)
                        Clients.currentServer = serverClient;
                    
                    result.RemoteEndPoint.Port = port;
                    serverFound.Invoke(result);
                }

            } catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}