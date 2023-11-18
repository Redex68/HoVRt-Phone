using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System;

public static class Clients
{
    public static List<IPAddress> adapterAddresses = new();
    public static List<IPAddress> serverAddresses = new();
    public static Dictionary<IPAddress, Tuple<UdpClient, IPEndPoint>> adapters = new();
    public static Dictionary<IPAddress, UdpClient> servers = new();
    public static Tuple<UdpClient, IPEndPoint> currentAdapter = null;
    public static UdpClient currentServer = null;
}
