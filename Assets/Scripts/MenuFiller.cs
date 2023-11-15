using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class MenuFiller : MonoBehaviour
{
    [SerializeField] Discovery discovery;
    [SerializeField] TMPro.TMP_Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        discovery?.serverFound?.AddListener(AddToList);
    }

    void AddToList(UdpReceiveResult result)
    {
        dropdown.AddOptions(new List<string> { result.RemoteEndPoint.ToString() } );
    }
}
