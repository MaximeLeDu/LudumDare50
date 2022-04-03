using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScript : MonoBehaviour
{
    private bool hasStarted = false;
    private bool positionSet = false;
    private bool startEnding = false;

    private readonly float walkSpeed = 2f;
    private readonly float lossOfSpeed = 0.003f;
    private float minSpeed = 0.02f;

    private Animator animator;
    private ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.doingSetup && !hasStarted)
        {
            hasStarted = true;
            StartCoroutine(GoToCenterRoutine());
        }
        if (positionSet)
        {
            animator.SetTrigger("Sleep");
            animator.SetBool("CanWakeUp", false);
            animator.speed /= 10f;
            particles.Play();
            StartCoroutine(WaitForEnd());
            positionSet = !positionSet;
        }

        if (startEnding)
        {
            GameManager.Instance.EndGame();
            startEnding = !startEnding;
        }
    }

    IEnumerator GoToCenterRoutine()
    {
        animator.SetFloat("Speed", 0.6f);
        float speed = walkSpeed;
        Vector3 endPoint = Vector3.zero;

        float sqrDistance = (transform.position - endPoint).sqrMagnitude;

        while (sqrDistance > float.Epsilon && speed > minSpeed)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);
            transform.position = newPosition;
            sqrDistance = (transform.position - endPoint).sqrMagnitude;
            //Loss of 0.2% speed and animation speed per frame
            speed -= lossOfSpeed * speed;
            animator.speed -= lossOfSpeed * animator.speed / 5f;
            yield return null;
        }
        animator.SetFloat("Speed", 0f);
        positionSet = true;
    }

    IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(5);
        startEnding = true;
    }
}
