using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{

    Queue<Task> tasks = new Queue<Task>();
    public List<GameObject> visibleObjects = new List<GameObject>();
    public List<string> uniqueTasks = new List<string>();
    public SpriteRenderer arm;
    public SpriteRenderer hand;
    bool looksRight = true;
    public GameObject targetItem = null;
    // Start is called before the first frame update
    void Start()
    {
        Color color = Random.ColorHSV(0, 1, 0.5f, 1, 1, 1);
        GetComponent<SpriteRenderer>().color = color;
        arm.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        AlignTransform();
        ManageTasks();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag("WorkStation") || go.CompareTag("Item"))
        {
            visibleObjects.Add(go);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag("WorkStation") || go.CompareTag("Item"))
            {
                visibleObjects.Remove(go);
        }
    }

    void  AlignTransform()
    {
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, position.y, position.y);

        float velocityX = GetComponent<Rigidbody2D>().velocity.x;
        if (looksRight && velocityX < 0 || !looksRight && velocityX > 0)
        {
            Turn();
        }

    }

    void Turn ()
    {
        looksRight = !looksRight;
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
    }

    public void TurnTo (GameObject target)
    {
        if (target == null) return;
        float targetDirection = (transform.position - target.transform.position).x;
        if (targetDirection > 0 && looksRight || targetDirection < 0 && !looksRight)
            Turn();
    }

    bool CompareWorkStationName (GameObject go, string name)
    {
        return go.GetComponent<WorkStation>()?.GetName() == name;
    }

    void ManageTasks()
    {
        Task current = null;
        if (tasks.Count > 0 )
            current = tasks.Peek();
        if (current != null)
        {
            Debug.Log(current);
            if (!current.isComplete)
            {
                current.currentSubtask.Tick();
                current.CheckSubtaskComplition();
            }
            else
            {
                tasks.Dequeue();
            }
        }
        SearchForPile();
        SearchForItems();
    }

    void SearchForPile ()
    {
        GameObject pile = visibleObjects.Find(o => CompareWorkStationName(o, "PILE"));

        if (pile == null  || uniqueTasks.Contains("DIG")) return;
        AddTask(new Dig(gameObject, pile));
    }

    void SearchForItems ()
    {
        if (targetItem != null) return;
        targetItem = visibleObjects.Find(o => o.CompareTag("Item"));
        if (targetItem == null) return;
        AddTask(new Pick(gameObject, targetItem));

    }

    void AddTask (Task task)
    {
        tasks.Enqueue(task);
        Debug.Log(tasks.Count);
        if (task.IsUnique())
            uniqueTasks.Add(task.GetName());
    } 

    public void Give (Sprite sprite)
    {
        hand.sprite = sprite;
        GetComponent<Animator>().SetBool("has item", true);
        AddTask(new Wandering(gameObject));
    }
}
