using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
public static LevelManager Singletone;
    public GameObject BulletHole;
    public LayerMask BulletIgnore;
    public TextMeshProUGUI ammotext;
    public TextMeshProUGUI HpText;
    public Image Hpbar;
    private void Awake()
    {
        if(Singletone == null)
        {
            Singletone = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
