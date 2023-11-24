using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class CustomIP : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField customIp;
    [SerializeField] TMPro.TMP_InputField customPort;
    [SerializeField] Discovery discovery;
    
    public bool connected {get; private set;} = false;

    public void AddServer()
    {
        IPAddress ip;
        int port;
        if(!IPAddress.TryParse(customIp.text, out ip) || !int.TryParse(customPort.text, out port))
            return;

        if(Clients.serverAddresses.Contains(ip))
        {
            Clients.currentServer = Clients.servers[ip];
        }
        else
        {
            UdpClient serverClient = new();
            serverClient.Connect(ip, port);

            Clients.serverAddresses.Add(ip);
            Clients.servers.Add(ip, serverClient);
            Clients.currentServer ??= serverClient;
            
            UdpReceiveResult result = new(new byte[1] {0}, new IPEndPoint(ip, port));
            discovery.serverFound.Invoke(result);
        }
    }
}
