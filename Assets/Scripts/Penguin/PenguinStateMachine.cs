using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class PenguinStateMachine : MonoBehaviour {
    #region FSM
    public enum State
    {
        Null,
        Normal,
        Sliding,
        SlidingOnSlope,
        SlidingJumping,
        Jumping,
        WatingToStart
    }
    public enum StateInput
    {
        Enter, Excute, Exit
    }
    public Dictionary<State, Dictionary<StateInput, Action<float>>> rule = new Dictionary<State, Dictionary<StateInput, Action<float>>>();
    State currentState = State.Null;
    public void ChangeState(State state)
    {
        if (currentState != State.Null)
        {
            rule[currentState][StateInput.Exit](0);
        }
        currentState = state;
        rule[currentState][StateInput.Enter](0);
    }
    public void StateUpdate(float time)
    {
        if (currentState != State.Null)
            rule[currentState][StateInput.Excute](time);
    }
    public void RefreshState()
    {
        if (currentState != State.Null)
        {
            rule[currentState][StateInput.Exit](0);
            rule[currentState][StateInput.Enter](0);
        }
    }
    void FsmRuleInit()
    {
        State state = State.Normal;
        rule[state] = new Dictionary<StateInput, Action<float>>();
        rule[state][StateInput.Enter] = (time) =>
        {
            forces = Vector2.zero;
            velocity.y = 0;
        };
        rule[state][StateInput.Excute] = (time) =>
        {
            if (!isOnGround)
            {
                ChangeState(State.Jumping);
                return;
            }
            if (platformAngle != 0)
            {
                float sign = platformAngle > 0 ? 1 : -1;
                float velocityXSign = velocity.x > 0 ? 1 : -1;
                velocity = velocity.magnitude * platformDirection * velocityXSign * sign;
                ChangeState(State.SlidingOnSlope);
                return;
            }
            if (velocity.x!=0)
            {
                animator.Play("Walking");

            }else
            {
                animator.Play("Idle");
            }
            forces = Vector2.zero;
            velocity.x = Input.GetAxis("Horizontal") * maxSpeed;
            velocity.y = 0;
            if (Input.GetKey(KeyCode.Space))
            {
                velocity.y = jumpSpeed;
            }
        };
        rule[state][StateInput.Exit] = (time) =>
        {
            return;
        };
        state = State.Jumping;
        rule[state] = new Dictionary<StateInput, Action<float>>();
        rule[state][StateInput.Enter] = (time) =>
        {

        };
        rule[state][StateInput.Excute] = (time) =>
        {
            if (isOnGround)
            {
                ChangeState(State.Normal);
                return;
            }
            forces = Vector2.zero;
            velocity.x = Input.GetAxisRaw("Horizontal") * maxSpeed;
            forces += new Vector2(0, -gravity);

        };
        rule[state][StateInput.Exit] = (time) =>
        {
            return;
        };
        state = State.Sliding;
        rule[state] = new Dictionary<StateInput, Action<float>>();
        rule[state][StateInput.Enter] = (time) =>
        {
            
        };
        rule[state][StateInput.Excute] = (time) =>
        {
            if (bodyTemp < tooHotTemp)
            {
                animator.Play("Sliding");
            }
            else
            {
                animator.Play("HotSliding");

            }
            if (!isOnGround)
            {
                ChangeState(State.SlidingJumping);
                return;
            }
            if (velocity.magnitude < 0.2f && bodyTemp < tooHotTemp)
            {
                ChangeState(State.Normal);
                return;
            }
            if (platformAngle != 0)
            {
                ChangeState(State.SlidingOnSlope);
                return;
            }
            
            float velocityXSign = velocity.x > 0 ? 1 : -1;
            velocity = Vector2.right * velocityXSign * velocity.magnitude;
            forces = Vector2.zero;
            forces += -velocity.normalized * platform.drag * velocity.magnitude / maxSpeed;
            slidingRotate();

        };
        rule[state][StateInput.Exit] = (time) =>
        {
            return;
        };
        state = State.SlidingOnSlope;
        rule[state] = new Dictionary<StateInput, Action<float>>();
        rule[state][StateInput.Enter] = (time) =>
        {
            
        };
        rule[state][StateInput.Excute] = (time) =>
        {
            if (bodyTemp < tooHotTemp)
            {
                animator.Play("Sliding");
            }
            else
            {
                animator.Play("HotSliding");

            }

            if (!isOnGround)
            {
                ChangeState(State.SlidingJumping);
                return;
            }
            if (platformAngle == 0)
            {
                
                ChangeState(State.Sliding);
                return;
            }
            
            float sign = platformAngle > 0 ? 1 : -1;
            float velocityXSign = velocity.x > 0 ? 1 : -1;
            velocity = velocity.magnitude * platformDirection * velocityXSign * sign;
            forces = Vector2.zero;
            forces += (gravity * Mathf.Sin(Mathf.Abs(platformAngle) * Mathf.Deg2Rad) - (downSlope ? 1 : -1) * platform.drag * Mathf.Cos(Mathf.Abs(platformAngle) * Mathf.Deg2Rad)) * platformDirection;
            slidingRotate();

        };
        rule[state][StateInput.Exit] = (time) =>
        {
            return;
        };
        state = State.SlidingJumping;
        rule[state] = new Dictionary<StateInput, Action<float>>();
        rule[state][StateInput.Enter] = (time) =>
        {
           
        };
        rule[state][StateInput.Excute] = (time) =>
        {
            if (bodyTemp < tooHotTemp)
            {
                animator.Play("Sliding");
            }
            else
            {
                animator.Play("HotSliding");

            }
            if (isOnGround)
            {
                if (platformAngle == 0)
                {
                    velocity.y = 0;
                    ChangeState(State.Sliding);
                    return;

                }
                else
                {
                    velocity -= Vector2.Dot(velocity, platformNormal) * platformNormal;
                    ChangeState(State.SlidingOnSlope);
                    return;
                }
            }
            forces = Vector2.zero;
            forces += new Vector2(0, -gravity);
            slidingRotate();

        };
        rule[state][StateInput.Exit] = (time) =>
        {
            return;
        };
        state = State.WatingToStart;
        rule[state] = new Dictionary<StateInput, Action<float>>();
        rule[state][StateInput.Enter] = (time) =>
        {
            increasingTemp = false;
            GetComponentInChildren<SpriteRenderer>().material.color = new Color(1, 1, 1, 0.5f);
            collider.enabled = false;
            Vector2 coo = new Vector2((int)rigidbody.position.x - 1, (int)rigidbody.position.y + 1);
            GameMap gameMap = GameObject.Find("GameMap").GetComponent<GameMap>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    gameMap.penguinPosList.Add(new Vector2(coo.x - 1 + i, coo.y + j) - gameMap.originPoint);
                }
            }
        };
        rule[state][StateInput.Excute] = (time) =>
        {
            
            velocity = Vector2.zero;
            forces = Vector2.zero;
        };
        rule[state][StateInput.Exit] = (time) =>
        {
            increasingTemp = true;
            GetComponentInChildren<SpriteRenderer>().material.color = new Color(1, 1, 1, 1f);
            collider.enabled = true;
            GameMap gameMap = GameObject.Find("GameMap").GetComponent<GameMap>();
            gameMap.penguinPosList.Clear();
            return;
        };
    }
    #endregion
    #region Monobehaviour
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Freezer")
        {
            SceneManager.LoadScene(1);
        }
        foreach(ContactPoint2D point in collision.contacts)
        {
            Vector2 normal = point.normal;
            float speed = velocity.magnitude;
            velocity -= Vector2.Dot(velocity, normal) * normal;
            velocity = velocity.normalized * speed;
        }
    }
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        sprite = transform.GetChild(0).gameObject;
        animator = sprite.GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        FsmRuleInit();
    }
    // Use this for initialization
    void Start()
    {
        ChangeState(State.WatingToStart);

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (bodyTemp < 1 && increasingTemp)
        {
            Collider2D[] results = Physics2D.OverlapCircleAll(rigidbody.position, 1.25f, LayerMask.GetMask("Platform", "Ice"));
            if (results.Length>0)
            {
                bool flag = false;
                foreach(Collider2D result in results)
                {
                    Platform platform = result.gameObject.GetComponent<Platform>();
                    if (platform != null)
                    {
                        if (platform.type == Platform.PlatformType.ice)
                        {
                            flag = true;
                            platform.health -= iceDeclineSpeed * Time.fixedDeltaTime;
                        }
                    }
                }
                if (flag)
                {
                    if (bodyTemp > 0)
                    {
                        bodyTemp -= declineSpeed * Time.fixedDeltaTime;
                    }
                }else
                {
                    bodyTemp += increasingSpeed * Time.fixedDeltaTime;

                }
            }
            else
            {
                bodyTemp += increasingSpeed * Time.fixedDeltaTime;

            }
        }
        if(bodyTemp> tooHotTemp)
        {
            if(currentState== State.Normal || currentState == State.Jumping)
            ChangeState(State.SlidingJumping);
        }
        setPlatform();
        StateUpdate(Time.fixedDeltaTime);
        velocity += forces * Time.fixedDeltaTime;
        moveFlip();
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
    void slidingRotate()
    {
        if (velocity != Vector2.zero)
        {
            float degree = Vector2.Angle(Vector2.right, velocity) * (velocity.y > 0 ? 1 : -1);
            sprite.transform.eulerAngles = velocity.x >= 0 ? new Vector3(0, 0, degree) : new Vector3(0, 0, degree + 180);
        }
    }
    void moveFlip()
    {
        if(velocity.x != 0)
        sprite.transform.localScale = new Vector3(Mathf.Abs(sprite.transform.localScale.x) * (velocity.x > 0 ? 1 : -1), sprite.transform.localScale.y, sprite.transform.localScale.z);

    }
    private void OnDrawGizmos()
    {
        if (rigidbody != null)
        {
            Gizmos.color = Color.red;
            Vector2 foot = new Vector2(0, -1f * GetComponent<CircleCollider2D>().radius * transform.localScale.x * Mathf.Pow(2, 0.4f) + skinWidth);
            Gizmos.DrawLine(rigidbody.position + foot, rigidbody.position + foot + Vector2.down * rayCastDistance);
        }
    }
    #endregion
    #region movement
    public float maxSpeed = 3;
    public Vector2 velocity = new Vector2(0, 0);
    public Vector2 forces = new Vector2(0, 0);
    public float gravity = 5;
    public float rayCastDistance = 0.2f;
    public float slidingCheckRayDistance = 1f;
    public float skinWidth = 0.1f;
    public float platformAngle = 0;
    public float jumpSpeed = 1;
    public bool increasingTemp = false;
    [Range(0,1)]
    public float bodyTemp = 0;
    [Range(0,1)]
    public float tooHotTemp = 0.5f;
    public float increasingSpeed = 0.15f;
    public float declineSpeed = 0.15f;
    public float iceDeclineSpeed = 0.1f;
    Collider2D collider;
    public Vector2 platformDirection
    {
        get
        {
            float sign = platformAngle > 0 ? 1 : -1;
            float angle = Mathf.Abs(platformAngle);
            return new Vector2(sign * Mathf.Cos(angle * Mathf.Deg2Rad), -1 * Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        }
    }
    public bool downSlope
    {
        get
        {
            return velocity.x * platformAngle > 0;
        }
    }
    public Vector2 platformNormal = Vector2.zero;
    Rigidbody2D rigidbody;
    Platform platform = null;
    public bool isOnGround
    {
        get
        {
            return platform != null;
        }
    }
    void setPlatform()
    {
        Vector2 foot = new Vector2 (0,-1f * GetComponent<CircleCollider2D>().radius * transform.localScale.x * Mathf.Pow(2,0.5f) + skinWidth);
        RaycastHit2D result = Physics2D.Raycast(rigidbody.position + foot, Vector2.down, rayCastDistance, LayerMask.GetMask("Platform", "Ice"));
        if (result)
        {
            platform = result.collider.gameObject.GetComponent<Platform>();
            platformAngle = Vector2.Angle(Vector2.up, result.normal) * (result.normal.x > 0 ? 1 : -1);
            platformNormal = result.normal;
            if (Mathf.Abs(platformAngle) < 3)
            {
                platformAngle = 0;
            }
        }
        else
        {
            platform = null;
        }
    }
    #endregion

    #region spriteAndAnimator
    GameObject sprite;
    Animator animator;
    #endregion
}
