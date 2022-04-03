using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AnimalNeeds
{
    public bool hasWashed;
    public bool hasEaten;
    public bool hasBeenPet;

    public AnimalNeeds(bool wash, bool eat, bool pet)
    {
        hasWashed = wash;
        hasEaten = eat;
        hasBeenPet = pet;
    }
}


public class AnimalScript : MonoBehaviour
{
    delegate void AnimalRandomAction();

    private List<AnimalRandomAction> actions;

    public int satisfaction;
    private int totalSatisfaction = 10;
    private readonly int satisfactionPerDay = 5;

    public AnimatorOverrideController hurtController;
    public AnimatorOverrideController hurterController;


    private Animator animator;
    private Rigidbody2D rb2D;
    private ParticleSystem heartParticles;
    private SpriteRenderer spriteRenderer;

    public AnimalNeeds needs;
    private bool isActing;

    private readonly float actionTime = 4f;
    private readonly float walkSpeed = 4f;
    private readonly float runSpeed = 8f;

    public AudioClip dogSound;
    public AudioClip happySound;
    public AudioClip hurtSound;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        heartParticles = GetComponent<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        CreateList();

        if (GameManager.Instance.animalHealth < 5)
            animator.runtimeAnimatorController = hurterController;
        else if (GameManager.Instance.animalHealth < 10)
            animator.runtimeAnimatorController = hurtController;

        needs = GameManager.Instance.needs;

        satisfaction = GameManager.Instance.satisfaction;

        Debug.Log(needs.hasBeenPet);
        Debug.Log(needs.hasWashed);
        Debug.Log(needs.hasEaten);

        totalSatisfaction = 10 + GameManager.Instance.days * satisfactionPerDay;
    }

    void CreateList()
    {
        actions = new List<AnimalRandomAction>
        {
            MoveAround,
            DoNothing,
            Sit,
            Sleep
        };

        if (GameManager.Instance.isHappy)
            actions.Add(Happy);

        if (GameManager.Instance.isSad)
            actions.Add(Sad);
    }

    public bool IsSatisfied()
    {
        return satisfaction >= totalSatisfaction;
    }

    //Triggers are objects to put on the animal to make him happy (or unhappy) with animations
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Broom does nothing to doggo
        if (collision.CompareTag("Broom"))
            return;
        //Any other trigger is more important than anything else
        if (isActing)
            StopAllCoroutines();
        animator.SetTrigger("ReturnIdle");
        animator.SetFloat("Speed", 0f);
        SoundManager.Instance.RandomizeSfx(happySound);
        if (collision.CompareTag("Washing") && !needs.hasWashed)
        {
            animator.SetTrigger("Wash");
            needs.hasWashed = true;
            satisfaction += 10;
        }
        else if (collision.CompareTag("Food") && !needs.hasEaten)
        {
            animator.SetTrigger("Eat");
            needs.hasEaten = true;
            satisfaction += 10;
        }

        GameManager.Instance.hasSpawnedObject = false;
        Destroy(collision.gameObject);
        StartCoroutine(DoNothingRoutine());

    }

    //Dog can collide with hurtful objects on the screen, which reduce his satisfaction and make his health decrease more
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActing)
            StopAllCoroutines();
        SoundManager.Instance.RandomizeSfx(hurtSound);
        animator.SetTrigger("ReturnIdle");
        animator.SetFloat("Speed", 0f);
        Destroy(collision.gameObject);
        animator.SetTrigger("Hurt");
        GameManager.Instance.animalHealth -= 2;

        StartCoroutine(DoNothingRoutine());
    }

    private void MoveAround()
    {
        float x = Random.Range(GameManager.Instance.xMin, GameManager.Instance.xMax);
        float y = Random.Range(GameManager.Instance.yMin, GameManager.Instance.yMax);

        StartCoroutine(MoveToRoutine(x, y));
    }

    private void DoNothing()
    {
        StartCoroutine(DoNothingRoutine());
    }

    private void Sit()
    {
        animator.SetTrigger("Sit");
        StartCoroutine(DoNothingRoutine());
    }

    private void Happy()
    {
        animator.SetTrigger("Happy");
        StartCoroutine(DoNothingRoutine());
    }

    private void Sad()
    {
        animator.SetTrigger("Sad");
        StartCoroutine(DoNothingRoutine());
    }

    private void Sleep()
    {
        animator.SetTrigger("Sleep");
        StartCoroutine(DoNothingRoutine());
    }

    private void Update()
    {
        if (isActing || GameManager.Instance.doingSetup)
            return;

        RandomDogSound();

        int actionIndex = Random.Range(0, actions.Count);
        actions[actionIndex]();
    }

    private void RandomDogSound()
    {
        float rand = Random.Range(0f, 1f);
        if (rand < 0.5f)
            SoundManager.Instance.RandomizeSfx(dogSound);
            
    }

    private void OnMouseDown()
    {
        heartParticles.Play();
        SoundManager.Instance.RandomizeSfx(happySound);
        if (!needs.hasBeenPet)
        {
            needs.hasBeenPet = true;
            satisfaction += 10;
        }
    }

    IEnumerator MoveToRoutine(float x, float y)
    {
        isActing = true;

        float speed;
        Vector3 endPoint = new Vector3(x, y, 0f);

        if (spriteRenderer.flipX && x < transform.position.x)
            spriteRenderer.flipX = false;
        else if (!spriteRenderer.flipX && x > transform.position.x)
            spriteRenderer.flipX = true;

        float sqrDistance = (transform.position - endPoint).sqrMagnitude;
        if (sqrDistance > 25)
        {
            speed = runSpeed;
            animator.SetFloat("Speed", 1.1f);
        }
        else
        {
            speed = walkSpeed;
            animator.SetFloat("Speed", 0.6f);
        }

        while (sqrDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, endPoint, speed * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrDistance = (transform.position - endPoint).sqrMagnitude;
            yield return null;
        }
        rb2D.MovePosition(endPoint);
        animator.SetFloat("Speed", 0f);
        isActing = false;
    }

    IEnumerator DoNothingRoutine()
    {
        isActing = true;
        yield return new WaitForSeconds(actionTime);
        isActing = false;
    }
}
