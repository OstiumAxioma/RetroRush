using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIFSM : MonoBehaviour
{

    public float enemySpeed = 5.0f;
    public enum FSMStates
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead
    }
    public float attackDistance = 6.0f;
    public float chaseDistance = 10.0f;
    public float shootRate = 2.5f;
    public GameObject player;
    public GameObject deadVFX;

    public FSMStates currentState;
    public GameObject[] spawnProjectiles;
    public GameObject ShotPoint;
    GameObject[] PatrolPoints;
    Vector3 nextDestination;
    Animator anim;
    int currentDestinationIndex = 0;
    float distanceToPlayer;
    float elapsedTime = 0;
    EnemyHealth enemyHealth;
    float health;
    Transform deadTransform;
    bool isDead;

    private Vector3 originalScale;
    private Vector3 targetScale;
    public GameObject electricField;
    public float damageAmount = -0.1f;
    float damageTime = 0;

    public int scoreAmount = 2;
    public static int TotalScoreAmount;

    NavMeshAgent agent;

    public Transform enemyEyes;
    public float fieldOfView = 90.0f;

    public int type = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            distanceToPlayer = 100.0f;
            return;
        }
        else {
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        }
        health = enemyHealth.currentHealth;

        switch(currentState)
        {
            case FSMStates.Patrol:
                UpdatePatrolState();
                break;
            case FSMStates.Chase:
                UpdateChaseState();
                break;
            case FSMStates.Attack:
                UpdateAttackState();
                break;
            case FSMStates.Dead:
                UpdateDeadState();
                break;
        }
        //damageTime += Time.deltaTime;

        if (health <= 0)
        {
            currentState = FSMStates.Dead;
        }

    }

    void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PatrolPoints = new GameObject[1];
        GameObject newObj = new GameObject("Patrol1");
        newObj.transform.position = this.transform.position; 
        PatrolPoints[0] = newObj;
        //PatrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoints");
        /*
        PatrolPoints = new List<GameObject>();
        GameObject newTransform1 = new GameObject("Patrol1");
        newTransform1.transform.position = this.gameObject.transform.position + new Vector3(1, 0, 0);
        GameObject newTransform2 = new GameObject("Patrol2");
        newTransform1.transform.position = this.gameObject.transform.position - new Vector3(1, 0, 0);
        PatrolPoints.Add(newTransform1);
        PatrolPoints.Add(newTransform2);
        */
        anim = GetComponent<Animator>();
        ShotPoint = GameObject.FindGameObjectWithTag("ShotPoint");
        enemyHealth = GetComponent<EnemyHealth>();
        health = enemyHealth.currentHealth;
        isDead = false;
        deadTransform = gameObject.transform;
        originalScale = electricField.transform.localScale;
        targetScale = originalScale * 2;
        currentState = FSMStates.Patrol;
        //nextDestination = this.transform.position;
        FindNextPoint();
    }

    void UpdatePatrolState()
    {
        print("Patroling");
        anim.SetInteger("animState", 1);
        if(Vector3.Distance(transform.position, nextDestination) < 1.0f)
        {
            FindNextPoint();
        }
        if (IsPlayerInClearFOV() || distanceToPlayer <= chaseDistance)
        {
            currentState = FSMStates.Chase;
        }
        //FaceTarget(nextDestination);
        EnemyExpendElectricField(false);
        //agent.SetDestination(nextDestination);
        //transform.position = Vector3.MoveTowards(transform.position, nextDestination, enemySpeed * Time.deltaTime);
    }

    void UpdateChaseState()
    {
        print("Chasing");
        anim.SetInteger("animState", 2);
        nextDestination = player.transform.position;
        if(distanceToPlayer < attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if(distanceToPlayer > chaseDistance)
        {
            FindNextPoint();
            currentState = FSMStates.Patrol;
        }
        FaceTarget(nextDestination);
        EnemyExpendElectricField(false);
        
        agent.SetDestination(nextDestination);
        //transform.position = Vector3.MoveTowards(transform.position, nextDestination, enemySpeed * Time.deltaTime); 
    }

    void UpdateAttackState()
    {
        //print("Attacking");
        nextDestination = player.transform.position;
        if(distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if(distanceToPlayer > attackDistance && distanceToPlayer <= chaseDistance)
        {
            currentState = FSMStates.Chase;
        }
        else if(distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }
        FaceTarget(nextDestination);
        //anim.SetInteger("animState", 3);
        //EnemySpellCast();
        //player.GetComponent<Health>().
        //DealDamage();
        //electricField.DealDamage();
        EnemyExpendElectricField(true);
    }

    void UpdateDeadState()
    {
        isDead = true;
        anim.SetInteger("animState", 4);
        deadTransform = gameObject.transform;
        Instantiate(deadVFX, deadTransform.position, deadTransform.rotation);
        Destroy(gameObject, 0.5f);
    }

    void FindNextPoint()
    {
        if (PatrolPoints.Length == 0) {
            return;
        }
        nextDestination = PatrolPoints[currentDestinationIndex].transform.position;

        currentDestinationIndex = (currentDestinationIndex + 1) % PatrolPoints.Length;
    }

    void FaceTarget(Vector3 target)
    {

        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;


        float angle = 90f;
        Vector3 rotatedDirection = Quaternion.Euler(0, angle, 0) * directionToTarget;


        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(rotatedDirection.x, 0, rotatedDirection.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    void DealDamage() {
        player.GetComponent<Health>().ReceiveHealth(damageAmount, gameObject);
        Debug.Log("Damage dealed, player health:" + player.GetComponent<Health>().CurrentHealth);
    }

    void EnemyExpendElectricField(bool isExpend) {
        if (elapsedTime < 0) {
            elapsedTime = 0;
        } else if (elapsedTime > 1) {
            elapsedTime = 1;
        }
        if (isExpend && elapsedTime < 1)
        {
            electricField.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / 1.0f);
            elapsedTime = elapsedTime + Time.deltaTime;
        }
        else if(!isExpend && elapsedTime > 0)
        {
            electricField.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / 1.0f);
            elapsedTime = elapsedTime - Time.deltaTime;
        }
    }

    void EnemySpellCast()
    {
        if(isDead != true)
        {
            if(elapsedTime > shootRate)
            {
                var animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
                Invoke("SpellCasting", animDuration);
                elapsedTime = 0.0f;

            }
        }
    }

    void SpellCasting()
    {
        GameObject spellProjectile = spawnProjectiles[Random.Range(0, spawnProjectiles.Length)];
        Instantiate(spellProjectile, ShotPoint.transform.position, ShotPoint.transform.rotation);
    }

    private void OnDestroy() 
    {
        Debug.Log("Enemy Destroyed");
        Debug.Log("deadVFX"+deadVFX);
        Debug.Log("deadTransform"+deadTransform);
        TotalScoreAmount += scoreAmount;
        //Instantiate(deadVFX, deadTransform.position, deadTransform.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Vector3 frontRayPoint = enemyEyes.position + (enemyEyes.forward * chaseDistance);

        Vector3 leftRayPoint = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * frontRayPoint;
        Vector3 rightRayPoint = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * frontRayPoint;

        Debug.DrawLine(enemyEyes.position, frontRayPoint, Color.yellow);
        Debug.DrawLine(enemyEyes.position, leftRayPoint, Color.green);
        Debug.DrawLine(enemyEyes.position, rightRayPoint, Color.blue);
    }

    bool IsPlayerInClearFOV()
    {
        RaycastHit hit;
        Vector3 direction = player.transform.position - enemyEyes.position;
        if (Vector3.Angle(direction, enemyEyes.forward) <= fieldOfView)
        {
            if (Physics.Raycast(enemyEyes.position, direction.normalized, out hit, chaseDistance))
            {
                //Debug.Log(hit.collider.name);
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }

}
