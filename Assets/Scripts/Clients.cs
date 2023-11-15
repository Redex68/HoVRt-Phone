using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public static class Clients
{
    public static List<IPAddress> adapterAddresses = new();
    public static List<IPAddress> serverAddresses = new();
    public static Dictionary<IPAddress, UdpClient> adapters = new();
    public static Dictionary<IPAddress, UdpClient> servers = new();
}
