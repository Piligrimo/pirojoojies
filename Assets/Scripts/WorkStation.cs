using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iWorkStation
{
    string GetName();
    void StartUsage(Work job);
}


public class WorkStation : MonoBehaviour
{
    public iWorkStation station;

    public MonoBehaviour ws;

  
    // Start is called before the first frame update
    void Start()
    {
        station = ws as iWorkStation;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public string GetName()
    {
        return station.GetName();
    }
}
