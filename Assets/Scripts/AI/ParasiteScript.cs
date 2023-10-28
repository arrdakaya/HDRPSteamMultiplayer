
using UnityEngine;
using UnityEngine.AI;

public class ParasiteScript : MonoBehaviour
{
    public static ParasiteScript instance;

    public enum ParasiteState
    {
        Idle,
        Walking,


    }
    public LayerMask PlayerLayer;

    private NavMeshAgent agent;
    public ParasiteState chooseState;
    private Animator anim;
    private AnimatorStateInfo animInfo;

    public GameObject player;
    public float parasiteAlertRange = 10f;
    private float attackDistance = 1.2f;

    [SerializeField] private AudioSource chaseMusic;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.SetTrigger(chooseState.ToString());
    }

    // Update is called once per frame
    void Update()
    {

        animInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (animInfo.IsTag("motion"))
        {
            if (anim.IsInTransition(0))
            {
                agent.isStopped = true;
            }
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, parasiteAlertRange, PlayerLayer);

        float closestDistance = Mathf.Infinity;

        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        player = collider.gameObject;
                        closestDistance = distance;
                        if (closestDistance <= attackDistance)
                        {
                            agent.isStopped = true;
                            anim.SetBool("Attack", true);

                            Vector3 pos = (player.transform.position - transform.position).normalized;
                            Quaternion posRotation = Quaternion.LookRotation(new Vector3(pos.x, 0, pos.z));
                            transform.rotation = Quaternion.Slerp(transform.rotation, posRotation, 5f * Time.deltaTime);
                        }
                        else
                        {
                            if (player != null)
                            {
                                anim.SetBool("Attack", false);

                                Debug.Log("Closest Player: " + player.name);
                                chooseState = ParasiteState.Walking;
                                if (chooseState == ParasiteState.Walking)
                                {

                                    agent.destination = player.transform.position;
                                    if (chaseMusic.volume < 0.6f)
                                    {
                                        if (chaseMusic.isPlaying == false)
                                        {
                                            chaseMusic.Play();

                                        }
                                        chaseMusic.volume += 0.5f * Time.deltaTime;
                                    }
                                }
                                WalkOn();

                            }
                        }
                    }
                }
            }
        }
        else if (colliders.Length < 1)
        {
            player = null;

            if (chaseMusic.volume > 0)
            {
                chaseMusic.volume -= 0.5f * Time.deltaTime;
            }
            if (chaseMusic.volume == 0)
            {
                chaseMusic.Stop();
            }

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
