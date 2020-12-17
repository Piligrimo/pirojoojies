using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pile : MonoBehaviour, iWorkStation
{

    List<DiggingJob> jobs = new List<DiggingJob>();
    public GameObject seed;

    class DiggingJob
    {
        float start;
        float duration;
        Work work;

        public DiggingJob(Work work, float duration)
        {
            start = Time.time;
            this.work = work;
            this.duration = duration;
        }

        public bool IsDone()
        {
            return ((Time.time - start) > duration);
        }

       public void SetAsDone ()
        {
            work.SetAsDone();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        List<DiggingJob> doneJobs = new List<DiggingJob>();

        foreach (DiggingJob job in jobs)
        {
            if (job.IsDone())
            {
                job.SetAsDone();
                doneJobs.Add(job);
                Instantiate(seed, transform.position + new Vector3(Random.Range(-5, 5), Random.Range(0, 5)), transform.rotation);
            }
        }
        foreach (DiggingJob  job in doneJobs)
        {
            jobs.Remove(job);
        }
    }

    public void StartUsage(Work work)
    {
        jobs.Add(new DiggingJob(work, Random.Range(3, 10)));
    }

    public string GetName()
    {
        return "PILE";
    }
}
