using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] points;
    public float moveDelay = 2f;
    public float jumpHeight = 0.05f;

    private NavMeshAgent agent;
    private float timer;
    private bool isWaiting = false;
    private bool isSeeAnimal = false;
    private bool isJumping = false;
    private Vector3 startPosition;
    private const float JUMP_TIME = 0.4f;
    private Vector3 basePosition;
    private WaitForSeconds jumpWait = new WaitForSeconds(0.2f);
    [SerializeField] private AudioSource exitGiveMoney_sound;

    private void Start()
    {
        InitializeNPC();
    }

    private void InitializeNPC()
    {
        agent = GetComponent<NavMeshAgent>();
        if (points != null && points.Length > 0)
        {
            MoveToRandomPoint();
        }
        else
        {
            Debug.LogError("Points array is not set for NPCMove.");
        }
    }

    private void Update()
    {
        if (!isJumping)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        if (!isWaiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartWaiting();
        }

        if (isWaiting)
        {
            UpdateWaitingTimer();
        }
    }

    private void StartWaiting()
    {
        isWaiting = true;
        timer = 0f;
    }

    private void UpdateWaitingTimer()
    {
        timer += Time.deltaTime;
        if (timer >= moveDelay)
        {
            MoveToRandomPoint();
            isWaiting = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zoo"))
        {
            HandleZooVisit(other);
        }
        else if (other.CompareTag("ExitPoint"))
        {
            HandleExit();
        }
    }

    private void HandleZooVisit(Collider other)
    {
        if (!isJumping && other.TryGetComponent<PrefabID>(out var prefabId))
        {
            GameController.playerMoney += prefabId.visitPrise;
            PlayerPrefs.SetInt("Money", GameController.playerMoney);
            isSeeAnimal = true;
            StartCoroutine(JumpRoutine());
        }
    }

    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        agent.enabled = false;
        basePosition = transform.position;
        Vector3 jumpPosition = basePosition + Vector3.up * jumpHeight;

        for (int i = 0; i < 3; i++)
        {
            transform.position = jumpPosition;
            yield return jumpWait;
            transform.position = basePosition;
            yield return jumpWait;
        }

        isJumping = false;
        agent.enabled = true;
    }

    private void HandleExit()
    {
        exitGiveMoney_sound.Play();
        agent.isStopped = true;
        if (isSeeAnimal)
        {
            NpcStat npcStat = GetComponent<NpcStat>();
            if (npcStat != null)
            {
                
                int payment = CalculatePayment(npcStat.GetHappyCount());
                GameController.playerMoney += payment;
                PlayerPrefs.SetInt("Money", GameController.playerMoney);
                Debug.Log($"Посетитель заплатил {payment} монет (Уровень счастья: {npcStat.GetHappyCount()})");
            }
        }
        Destroy(gameObject, 1f);
    }

    private int CalculatePayment(int happyCount)
    {
        // Если счастье 0, то оплата 0
        if (happyCount <= 0) return 0;
        
        // Оплата пропорциональна уровню счастья
        // При 100% счастья оплата будет 30 монет
        return Mathf.RoundToInt((happyCount / 100f) * 30f);
    }

    private void MoveToRandomPoint()
    {
        if (points == null || points.Length == 0) return;

        int randomIndex = Random.Range(0, points.Length);
        agent.SetDestination(points[randomIndex].position);
    }
}