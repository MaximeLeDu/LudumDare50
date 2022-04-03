using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalWalk : MonoBehaviour
{
    private bool isOnGround = true;
    private float jumpForce = 6.4f;
    private Animator animator;
    private Rigidbody2D rb2D;
    public AnimatorOverrideController hurtAnimator;
    public AnimatorOverrideController hurterAnimator;

    public AudioClip hurtSound;
    public AudioClip jumpSound;
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (GameManager.Instance.animalHealth < 5)
            animator.runtimeAnimatorController = hurterAnimator;
        else if (GameManager.Instance.animalHealth < 10)
            animator.runtimeAnimatorController = hurtAnimator;
        animator.SetFloat("Speed", 0.6f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            isOnGround = false;
            animator.SetBool("IsOnGround", false);
            SoundManager.Instance.RandomizeSfx(jumpSound);
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Touching");
        if (collision.gameObject.CompareTag("Poop"))
        {
            SoundManager.Instance.RandomizeSfx(hurtSound);
            Debug.Log("Bad guy");
            Destroy(collision.gameObject);
            animator.SetFloat("Speed", 0);
            animator.SetTrigger("ReturnIdle");
            animator.SetTrigger("Hurt");
            GameManager.Instance.animalHealth -= 1;
            StartCoroutine(RunningAgain());
            return;
        }

        Debug.Log("Ground");
        //If it is not an enemy it is the ground
        isOnGround = true;
        animator.SetBool("IsOnGround", true);
    }

    IEnumerator RunningAgain()
    {
        yield return new WaitForSeconds(1);
        animator.SetFloat("Speed", 0.6f);
    }
}
