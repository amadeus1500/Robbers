using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MRoom : MonoBehaviour
{
    public string roomname_;
    public string Count;
    [SerializeField] private TextMeshProUGUI roomtext;
    [SerializeField] private TextMeshProUGUI MaxCount;
    void Start()
    {
        roomtext.text = roomname_;
        MaxCount.text = Count;
    }
}
