using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;
    public FloatingJoystick joystick;
    Vector3 normalizedMoveVector;
    Vector3 lastNormalized;
    [Range(1f, 100f), SerializeField]
    float maxDonusDerecesi;


    [Range(1f, 100f), SerializeField]
    float speed;
    [Range(10f, 1000f), SerializeField]
    float jumpSpeed;


    float currentMode;
    bool kickMode;
    float kickModeCounter;

    public float tapSayaci;
    public bool kickTrigger;
    bool kickAnim;


    CapsuleCollider playerTrigger;
    Touch touch;
    Vector3 firstTouchPos;
    float deltaSwipeX;
    bool speedDecrease;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
        playerTrigger = gameObject.transform.GetChild(1).GetComponent<CapsuleCollider>();
        speed = 2.5f;
        kickModeCounter = 0;
        currentMode = 1f;
        tapSayaci = 0;
        kickTrigger = false;
        maxDonusDerecesi = 50;
        speedDecrease = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SwipeControl();
        MoveForward();
        //MoveWithJoystick();
        KickModeStarter();
        speed = Mathf.Clamp(speed, 2f, 20f);
    }

    void MoveWithJoystick()
    {
        normalizedMoveVector = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;
        rb.velocity = (normalizedMoveVector * speed);

        if (touch.phase == TouchPhase.Moved)
        {
            lastNormalized = normalizedMoveVector;
        }
        if (touch.phase == TouchPhase.Ended)
        {
            rb.velocity = (lastNormalized*speed);
            
        }

        if (normalizedMoveVector != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(normalizedMoveVector), maxDonusDerecesi * Time.deltaTime);
            
        }

        //if (normalizedMoveVector == Vector3.zero)
        //{
        //    rb.velocity = (normalizedMoveVector * speed);
        //}

        // Running to Walking
        if (speed < 3f)
        {
            animator.SetBool("isRunning", false);
        }
        // Walking to Sprinting
        if (10f >= speed && speed >= 3f)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isSprinting", false);
        }
        // Running to Sprinting
        if (speed >= 15f)
        {
            animator.SetBool("isSprinting", true);
            animator.SetBool("isRunning", false);
        }
    }
    void SwipeControl()
    {
        if (Input.touchCount>0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase== TouchPhase.Began)
            {
                firstTouchPos = touch.position;
            }
            deltaSwipeX = touch.position.x - firstTouchPos.x;
        }
    }
    void MoveForward()
    {
        rb.velocity = new Vector3(rb.velocity.x/speed, rb.velocity.y / speed, 1) * speed;
        transform.position = new Vector3(Mathf.Clamp(deltaSwipeX /200,-1.5f,1.5f), transform.position.y, transform.position.z);

        // Running to Walking
        if (speed < 3f)
        {            
            animator.SetBool("isRunning", false);
        }
        // Walking to Sprinting
        if (10f >= speed && speed >= 3f)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isSprinting", false);
        }
        // Running to Sprinting
        if (speed >= 15f)
        {
            animator.SetBool("isSprinting", true);
            animator.SetBool("isRunning", false);
        }
    }
    void TapSayaci()
    {
        if (Input.GetMouseButtonDown(0))
        {
            tapSayaci += 1;
        }
        tapSayaci = Mathf.Clamp(tapSayaci, 0f, 100f);
        tapSayaci -= Time.fixedDeltaTime;
        
        // Deneme
        
    }
    // Kick mode'a geçiş yapılarak gerekli dövüş işlemleri tamamlanacak
    // Yavaş yavaş hız azaltılacak
    // KickMode olmadan çarpışma olursa fail

    void KickModeStarter()
    {
        if (Input.touchCount > 0 && touch.phase != TouchPhase.Ended || Input.GetMouseButton(0))
        {
            kickTrigger = true;
            animator.SetBool("isKicking", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isSprinting", false);
            playerTrigger.radius = 1f;
            speedDecrease = true;

            //rb.AddForce(new Vector3(0, jumpSpeed, 0));

        }
        else
        {
            speedDecrease = false;
            kickTrigger = false;
            animator.SetBool("isKicking", false);
            playerTrigger.radius = 0.3f;
            speed += Time.fixedDeltaTime * 5;
            

        }
        if (speedDecrease == true) 
        { 
        speed -= Time.fixedDeltaTime * 5;
            speedDecrease = false;

        }
        
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 && kickMode == false)

        {
            SceneManager.LoadScene(0);

        }


        if (other.gameObject.tag =="Finish")
        {
            // Finish Ekranı
            // Kazanma sesi
            // Kazanma animasyonu
            // Player Dans Animasyonu
            SceneManager.LoadScene(0);
        }
       
    }
}



