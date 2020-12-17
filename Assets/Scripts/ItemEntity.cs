using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : MonoBehaviour, iWorkStation
{
    Work pickingJob;
    float animationDuration = .8f;
    float start;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pickingJob!= null && Time.time - start > animationDuration)
        {
            Debug.Log(pickingJob.GetCharacter());
            pickingJob.GetCharacter().GetComponent<Control>().Give(GetComponent<SpriteRenderer>().sprite);
            pickingJob.SetAsDone();
            Destroy(gameObject);
        }
    }

    public string GetName()
    {
        return "ITEM_ENTITY";
    }

    public void StartUsage(Work job)
    {
        if (pickingJob != null)
            job.SetAsDone();
        else
        {
            pickingJob = job;
            start = Time.time;
        }
    }
}
