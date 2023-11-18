using System.Net;
using System.Text;
using UnityEngine;

public class AccelerometerReader : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Dropdown serverDropdown;
    
    public bool connected {get; private set;} = false;
    
    void Start()
    {
        serverDropdown.onValueChanged.AddListener(ServerSelected);
    }

    // Update is called once per frame
    void Update()
    {
        if(connected)
        {
            byte[] data = Encoding.ASCII.GetBytes($"{Input.acceleration.x} {Input.acceleration.y} {Input.acceleration.z}");
            Clients.currentServer.SendAsync(data, data.Length);
        }
    }

    public void ServerSelected(int indx)
    {
        IPAddress serverIP = Clients.serverAddresses[indx];
        Clients.currentServer = Clients.servers[serverIP];
    }

    public void StartSend()
    {
        connected = true;
    }

    public void StopSend()
    {
        connected = false;
    }
}
