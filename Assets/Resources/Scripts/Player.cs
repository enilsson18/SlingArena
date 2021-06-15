using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool controllable = false;

    //variables set by the dev at runtime
    public float slingSpeed = 0.03f;
    public float minLaunchSpeed = 1.0f;
    public float aimBrake = 0.1f;
    public float deceleration = 0.02f;

    //public GameObject tempEnemy

    //get the variables
    private Rigidbody rb;
    public TimeManager timeManager;

    enum InputType {
        PC,
        Iphone
    };

    private InputType inputType;

    //finger drag position for when it is released
    private Vector2 startDragPos;
    private Vector2 endDragPos;

    //status variable saying if the player is currently firing
    bool firing;

    //status about living
    public bool isAlive;

    // Start is called before the first frame update
    void Start()
    {
        //controllable = false;

        //init child vars
        rb = transform.GetComponent<Rigidbody>();

        //set speed vars with the new 
        
        //adjust speeds to match the change in fixed update
        slingSpeed /= Time.fixedDeltaTime / 0.015f;
        aimBrake /= Time.fixedDeltaTime / 0.015f;
        deceleration /= Time.fixedDeltaTime / 0.015f;

        //init normal variables
        firing = false;
        isAlive = true;

        //determine input type
        //Iphone
        #if UNITY_IOS
            inputType = InputType.Iphone;
        #endif

        //Computer
        #if UNITY_EDITOR_WIN
            inputType = InputType.PC;
        #endif
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateTime();
        updatePlayer();

        if (controllable)
        {
            //GetComponent<TrailRenderer>().enabled = true;
            controls();
        }
        else
        {
            //GetComponent<TrailRenderer>().enabled = false;
        }
    }

    //gather input
    void controls()
    {
        //sling mechanics
        //Computer
        if (inputType == InputType.PC)
        {
            //get leftclick to determine drag
            if (Input.GetMouseButton(0))
            {
                if (!firing)
                {
                    startDragPos = Input.mousePosition;
                    firing = true;
                }

                endDragPos = Input.mousePosition;
            }
            else if (!Input.GetMouseButton(0))
            {
                if (firing)
                {
                    Launch(startDragPos, endDragPos, slingSpeed);

                    firing = false;
                }
            }

            if (Input.GetMouseButton(1))
            {
                timeManager.setTimeScale(0.2f, 0.1f);
            }
        }

        //Iphone
        if (inputType == InputType.Iphone)
        {
            Touch touch = Input.GetTouch(0);

            //get begining and end of touch
            if (touch.phase == TouchPhase.Began)
            {
                startDragPos = touch.position;
                firing = true;
            }
            //end touch
            else if (touch.phase == TouchPhase.Ended)
            {
                if (firing)
                {
                    Launch(startDragPos, touch.position, slingSpeed);

                    firing = false;
                }
            }
        }
    }

    void updatePlayer()
    {
        //normal drag
        rb.velocity *= 1 - deceleration;

        //aim drag
        if (firing)
        {
            rb.velocity *= 1 - aimBrake;
        }

        //normal drag
        if (rb.velocity.magnitude < 0.00001)
        {
            rb.velocity *= 0;
        }
    }

    //collisions
    private void OnCollisionEnter(Collision collision)
    {
        //use collider because otherwise it will react to the main body and not the weapon
        if (collision.collider.gameObject.CompareTag("enemy_weapon"))
        {
            print("dead");
            isAlive = false;
            //this.gameObject.SetActive(false);
        }
    }

    //control player based on input
    //get direction and power vector from the negative difference of the two inputed and scale down the power based on the speed multiplyer given by the dev is
    void Launch(Vector2 startPos, Vector2 endPos, float speed)
    {
        //print("Fire");
        Vector2 newVelocity = endPos - startPos;
        newVelocity *= speed;
        //if the launch speed is lower than minimum then don't launch
        if (newVelocity.magnitude > minLaunchSpeed)
        {
            //rb.velocity += new Vector3(speed, speed, 0);
            rb.velocity += new Vector3(newVelocity.x, 0, newVelocity.y);
        }
    }

    //time alterations
    void updateTime()
    {
        float cvt = timeManager.cvtTime(1);
        deceleration = deceleration * cvt;
        aimBrake = aimBrake * cvt;
        slingSpeed = slingSpeed * cvt;

        //decelerate when time slows
        rb.velocity *= cvt;
    }
}
