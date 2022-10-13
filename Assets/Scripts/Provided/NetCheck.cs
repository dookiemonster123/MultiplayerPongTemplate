using System.Net;
using UnityEngine;

public class NetCheck : MonoBehaviour
{
    void Awake()
    {
        PrintNetwork();
    }

    public static void PrintNetwork()
    {
        string hostName = Dns.GetHostName();
        string ipAddr = Dns.GetHostAddresses(hostName)[0].ToString();
        Debug.Log($"Host ({hostName}) IP Address: {ipAddr}");
    }
}
