using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MRoom : MonoBehaviour
{
    public string roomname_;
    [SerializeField] private TextMeshProUGUI roomtext;
    void Start()
    {
        roomtext.text = roomname_;
    }
}
