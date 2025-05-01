using System.Collections.Generic;
using UnityEngine;

public class NetworkProperties : MonoBehaviour
{
    public static NetworkProperties instance;


    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetMonsterStates(string _name)
    {
        foreach (var name in monsterNames)
        {
            if (name == _name)
                return true;
        }
        return false;
    }

    public string GetMonsterStatesName(string _name)
    {
        foreach (var name in monsterNames)
        {
            if (name == _name)
                return _name;
        }
        return "";
    }

    public bool GetPlayerStates(int _name)
    {
        foreach (var name in ProfileNames)
        {
            if (name == _name)
                return true;
        }
        return false;
    }

    [SerializeField]
    List<string> monsterNames = new List<string>();


    [SerializeField]
    List<int> ProfileNames = new List<int>();



}
