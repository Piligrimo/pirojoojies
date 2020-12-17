using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task
{

    protected GameObject character;
    protected Queue<SubTask> subTasks;
    public SubTask currentSubtask;
    public bool isComplete = false;
    protected Animator animator;

    public abstract bool IsUnique();
    public abstract string GetName();

    public Task(GameObject character)
    {
        this.character = character;
        subTasks = new Queue<SubTask>();
        animator = character.GetComponent<Animator>();
    } 

    public void CheckSubtaskComplition ()
    {
        if (currentSubtask.IsComplete())
        {
            if (subTasks.Count > 0)
            {
                InitSubtask();
            }
            else
                isComplete = true;
        }
    }

    protected void InitSubtask ()
    {
        currentSubtask = subTasks.Dequeue();
        animator.SetTrigger(currentSubtask.AnimationTrigger());
        currentSubtask.Init();
    }
}

public  class Wandering : Task
{
    public Wandering (GameObject character) : base(character)
    {

        int cycleCount = Random.Range(1, 6);
        Vector2 charPosition = character.transform.position;
        for ( int i = 0; i < cycleCount; i++)
        {
            subTasks.Enqueue(new Waiting(character, Random.Range(0.2f, 7)));
            subTasks.Enqueue(new Walking(character, charPosition + Random.insideUnitCircle*10));
        }
        InitSubtask();
    }
    public override bool IsUnique(){  return true; }
    public override string GetName() { return "WANDER"; }
}

public class Idle : Task
{
    public Idle(GameObject character) : base(character)
    {
        subTasks.Enqueue(new Waiting(character, float.PositiveInfinity));
        InitSubtask();
    }
    public override bool IsUnique() { return false; }
    public override string GetName() { return "IDLE"; }
}

public class Dig : Task
{
    GameObject workStation;
    public Dig(GameObject character, GameObject workStation) : base(character)
    {
        this.workStation = workStation;
        Vector2 stationPosition = workStation.transform.position;
        subTasks.Enqueue(new Walking(character, stationPosition + new Vector2(0, -0.5f)));
        subTasks.Enqueue(new Work(character, workStation));
        InitSubtask();
    }
    public override bool IsUnique() { return true; }
    public override string GetName() { return "DIG"; }

}

public class Pick : Task
{
    GameObject workStation;
    public Pick(GameObject character, GameObject workStation) : base(character)
    {
        this.workStation = workStation;
        Vector2 stationPosition = workStation.transform.position;
        subTasks.Enqueue(new Walking(character, stationPosition + new Vector2(0, -0.5f)));
        subTasks.Enqueue(new Work(character, workStation));
        InitSubtask();
    }
    public override bool IsUnique() { return false; }
    public override string GetName() { return "PICK"; }

}

public abstract class SubTask
{
    public abstract bool IsComplete();
    public virtual void Init() { }
    public virtual void Tick() { }
    public abstract string AnimationTrigger();
    protected GameObject character;
    public SubTask (GameObject character)
    {
        this.character = character;
    }
}

public class Walking : SubTask
{
    Vector2 target;
    Rigidbody2D rigidbody;
    float speed = 3;

     public Walking (GameObject character, Vector2 target) : base(character)
    {
        this.target = target;
        rigidbody = character.GetComponent<Rigidbody2D>();
    }

    public override string AnimationTrigger()
    {
        return "walk";
    }

    public override bool IsComplete()
    {
        Vector2 position = new Vector2(character.transform.position.x, character.transform.position.y);
        return (position - target).magnitude < 0.1;
    }

    public override void Tick ()
    {
        Vector2 position = new Vector2(character.transform.position.x, character.transform.position.y);
        rigidbody.velocity = (target - position).normalized * speed;
    }
}

public class Waiting : SubTask
{
    float startTime;
    float duration;

    public Waiting(GameObject character, float duration) : base(character)
    {
        this.duration = duration;
    }

    public override string AnimationTrigger()
    {
        return "idle";
    }

    public override bool IsComplete()
    {
        return (Time.time - startTime) > duration;
    }

    public override void Init()
    {
        startTime = Time.time;
        character.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}

public class Work : SubTask
{
    bool isDone = false;
    GameObject workStation;

    public Work(GameObject character, GameObject workStation ) : base(character)
    {
        this.workStation = workStation;
    }

    public override string AnimationTrigger()
    {
        return "work";
    }

    public override bool IsComplete()
    {
        return isDone;
    }

    public override void Init()
    {
        if (workStation == null)
        {
            SetAsDone();
            return;
        }
        character.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        character.GetComponent<Control>().TurnTo(workStation);
        workStation.GetComponent<WorkStation>().station.StartUsage(this);
    }

    public void SetAsDone ()
    {
        isDone = true;
    }

    public GameObject GetCharacter()
    {
        return character;
    }
}