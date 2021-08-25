using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Tag
{
    Player,
    Enemy,
    Object,
    Map,
    Wall,
    DeadZone,

}

public class DataManager : MonoBehaviour
{
    static public DataManager ins = null;

    private void Awake()
    {
        ins = this;
    }
}
