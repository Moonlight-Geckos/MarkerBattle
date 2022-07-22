using System;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private List<SkinItem> allSkins;

    [SerializeField]
    private List<Player> playersList;

    #endregion Public

    private static DataHolder _instance;
    private static int _uniqueNum = 0;

    public static DataHolder Instance
    {
        get { return _instance; }
    }
    public static int UniqueNum
    {
        get {
            if (_uniqueNum > 1000000)
                _uniqueNum = 0;
            return _uniqueNum++;
        }
    }
    public List<SkinItem> AllSkins
    {
        get { return allSkins; }
    }
    public List<Player> Players
    {
        get { return playersList; }
    }

    #region Methods

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            allSkins?.Sort((x, y) => x.skinNumber.CompareTo(y.skinNumber));
            playersList?.Sort((x, y) => x.number.CompareTo(y.number));
        }
    }

    #endregion
}
