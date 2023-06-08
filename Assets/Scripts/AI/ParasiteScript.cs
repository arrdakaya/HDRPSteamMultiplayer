
using UnityEngine;
using UnityEngine.AI;

public class ParasiteScript : MonoBehaviour
{

    public enum ParasiteState
    {
        Idle,
        Walking,
        Eating,
        Crawling,
        Die,
        RealDeath

    }


    private NavMeshAgent agent;
    public ParasiteState chooseState;
    private Animator anim;
    private GameObject[] targets;
    private AnimatorStateInfo animInfo;

    private int currentState;
    private float distanceToPlayer;
    public GameObject player;
    public float parasiteAlertRange = 10f;
    private bool awareOfPlayer = false;
    private bool adding = true;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.SetTrigger(chooseState.ToString());
        targets = GameObject.FindGameObjectsWithTag("AITarget");
        player = GameObject.Find("LocalGamePlayer");
        //agent.destination = targets[0].transform.position;
        anim.SetTrigger(chooseState.ToString());
        currentState = (int)chooseState;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        animInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (animInfo.IsTag("motion"))
        {
            if (anim.IsInTransition(0))
            {
                agent.isStopped = true;
            }
        }
        if (distanceToPlayer < parasiteAlertRange)
        {
            chooseState = ParasiteState.Walking;
            if(chooseState == ParasiteState.Walking)
            {
                agent.destination = player.transform.position;
                WalkOn();
                awareOfPlayer = true;
            }
        }
       
       
        if(distanceToPlayer > parasiteAlertRange)
        {
            awareOfPlayer = false;
            WalkOff();
        }
       
      
      
    }
    public void WalkOn()
    {
        
        anim.SetTrigger(chooseState.ToString());
        agent.isStopped = false;
    }
    public void WalkOff()
    {
        chooseState = ParasiteState.Idle;
        anim.SetTrigger(chooseState.ToString());
        agent.isStopped = true;
    }

}
