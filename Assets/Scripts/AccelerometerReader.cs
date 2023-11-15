using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AccelerometerReader : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Dropdown serverDropdown;
    
    void Start()
    {
        serverDropdown.onValueChanged.AddListener(ServerSelected);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ServerSelected(int indx)
    {
        
    }
}
