﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    GameObject childPaddle1, childPaddle2;
    RectTransform tapAreaRect;
    Vector3[] corners; // corners of tapAreaRect in world space
    public float paddleMoveSpeedLimit; //should be around 1000 
    public float offset; //used to offset childPaddle1 in ClampedPos() so that the sprite does not appear in the wall
    BoxCollider2D paddleCollider;
    Rigidbody2D paddleRigidBody; // for continuous collisions
    Vector3 touch1Pos, touch2Pos;
    float paddleLength;
    float angle;
    public GameObject particlePrefab;
    Vector3 particleScale;
    GameObject particles;
    ParticleSystem.ShapeModule particleShape;
    Animator particleAnimator;
    Vector3[] pauseButtonCorners = new Vector3[4]; //used so that a paddle wont appear if the pause button is tapped
    public RectTransform pauseButtonRect;
    public static PaddleController Instance;
    public float maxPaddleLength; //subtracts length from the paddle;
    LayerMask background = 0;
    float rayDistance;
    Ray2D ray;
    Vector3 touchPos;
    Vector3 newTouchPos;
    EnemyBehavior ball;
    bool particlesActivated = false;
    CircleCollider2D endCollider1, endCollider2;
    float touch1Speed, touch2Speed;
    float clampedTouch1Speed, clampedTouch2Speed;

    private void Awake()
    {
        Instance = this;
        pauseButtonRect.GetWorldCorners(pauseButtonCorners);
        paddleCollider = new GameObject("paddleCollider").AddComponent<BoxCollider2D>();
        paddleCollider.gameObject.tag = "Paddle";
        paddleCollider.gameObject.layer = 12;
        paddleRigidBody = paddleCollider.gameObject.AddComponent<Rigidbody2D>();
        paddleRigidBody.freezeRotation = true;
        paddleRigidBody.bodyType = RigidbodyType2D.Kinematic;
        paddleRigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        childPaddle1 = transform.Find("Paddle1").gameObject;
        endCollider1 = childPaddle1.GetComponent<CircleCollider2D>();
        childPaddle2 = transform.Find("Paddle2").gameObject;
        endCollider2 = childPaddle2.GetComponent<CircleCollider2D>();

        GameObject tapArea = GameObject.Find("tapArea");

        // to use RectTransformUtility.RectangleContainsScreenPoint correctly the RectTransform parameter must be set to the transform of the game object not the rect tranfsorm components rect transform
        tapAreaRect = tapArea.transform as RectTransform;
        corners = new Vector3[4];
        tapAreaRect.GetWorldCorners(corners);

        particles = Instantiate(particlePrefab, Vector2.right * 900, paddleCollider.transform.rotation) as GameObject;        
        particleShape = particles.GetComponent<ParticleSystem>().shape; //to edit the shape of a particle system you must use a temp var (particlesyste.shapmodule) to store the the particlesytem.shape and then edit the temp var from there
        particleAnimator = particles.GetComponent<Animator>();
        particles.transform.parent = transform;
        particles.transform.localPosition = Vector2.zero;
    }

    private void Start()
    {
        ball = EnemyBehavior.Instance;
    }

    public float GetDistanceDifferenceForWalls()// used to set initial wall position in levelgenerator class
    {
        return 2.69159f - Vector3.Distance(new Vector3(0, 0, 0), new Vector3(corners[0].x, 0, 0));
    }

    private void OnEnable()
    {
        childPaddle1.SetActive(false);
        childPaddle2.SetActive(false);
        paddleCollider.gameObject.SetActive(false);
        particles.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        paddleCollider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (particles.activeInHierarchy)
        {
            particleAnimator.SetBool("particlesActivated", particlesActivated);
        }
        if (paddleCollider.gameObject.activeInHierarchy)
        {
            endCollider1.enabled = false;
            endCollider2.enabled = false;
        }
        else
        {
            endCollider1.enabled = true;
            endCollider2.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        //When using multi touch make sure to use fingerID's to track fingers seperately
        foreach (Touch t in Input.touches)
        {
            touchPos = Camera.main.ScreenToWorldPoint(t.position);
            newTouchPos = ClampedPos(touchPos);

            if (t.fingerId == 0)
            {
                touch1Pos = newTouchPos;
            }
            if (t.fingerId == 1)
            {
                rayDistance = Mathf.Clamp(Vector3.Distance(touch1Pos, newTouchPos), 0, maxPaddleLength);
                ray = new Ray2D(touch1Pos, newTouchPos - touch1Pos);
                touch2Pos = ray.GetPoint(rayDistance);
            }

            // this if block is used so that a paddle wont appear if the pause button is tapped
            if ((touchPos.x > pauseButtonCorners[0].x && touchPos.x < pauseButtonCorners[3].x) && (touchPos.y > pauseButtonCorners[0].y && touchPos.y < pauseButtonCorners[1].y))
            {
                return;
            }

            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (t.fingerId == 0)
                    {
                        if ((touchPos.x < pauseButtonCorners[0].x || touchPos.x > pauseButtonCorners[3].x) && (touchPos.y < pauseButtonCorners[0].y || touchPos.y > pauseButtonCorners[1].y))
                        {
                            childPaddle1.transform.position = new Vector3(newTouchPos.x, newTouchPos.y, 0); // this is necessary so that the childPaddle1.transform.position.z is not set to the same z value as the camera(this will cause it to be cut off by the camera's near clipping plane
                            childPaddle1.SetActive(true);
                        }
                    }

                    if (t.fingerId == 1)
                    {
                        if ((touchPos.x < pauseButtonCorners[0].x || touchPos.x > pauseButtonCorners[3].x) && (touchPos.y < pauseButtonCorners[0].y || touchPos.y > pauseButtonCorners[1].y))
                        {
                            childPaddle2.transform.position = new Vector3(touch2Pos.x, touch2Pos.y, 0);
                            childPaddle2.SetActive(true);
                        }
                    }
                    if(Input.touchCount > 1)
                    {
                        particlesActivated = true;
                    }
                    break;

                case TouchPhase.Moved:
                    if (t.fingerId == 0)
                    {
                        touch1Speed = t.deltaPosition.magnitude / t.deltaTime;
                        clampedTouch1Speed = Mathf.Clamp(touch1Speed, 0, paddleMoveSpeedLimit);
                        childPaddle1.transform.position = Vector2.Lerp(childPaddle1.transform.position, touch1Pos, clampedTouch1Speed * Time.deltaTime);
                        childPaddle1.SetActive(true);// might glitch if this is not true
                    }

                    if (t.fingerId == 1)
                    {
                        touch2Speed = t.deltaPosition.magnitude / t.deltaTime;
                        clampedTouch2Speed = Mathf.Clamp(touch2Speed, 0, paddleMoveSpeedLimit);
                        childPaddle2.transform.position = Vector2.Lerp(childPaddle2.transform.position, touch2Pos, clampedTouch2Speed * Time.deltaTime);
                        childPaddle2.SetActive(true); // might glitch if this is not true
                    }
                    break;

                case TouchPhase.Ended:
                    if (t.fingerId == 0)
                    {
                        childPaddle1.SetActive(false);
                    }

                    if (t.fingerId == 1)
                    {
                        childPaddle2.SetActive(false);
                    }
                    particlesActivated = false;
                    break;
            }
        }

        if (Input.touchCount > 1)
        {
            MakePaddle();
            paddleCollider.gameObject.SetActive(true);
        }
        else
        {
            paddleCollider.gameObject.SetActive(false);
        }

        if (Input.touchCount == 0)
        {
            childPaddle1.SetActive(false);
            childPaddle2.SetActive(false);
        }
    }

    public Vector2 ClampedPos(Vector2 touchPosition) // for first touch
    {
        float clampedX = Mathf.Clamp(touchPosition.x, corners[0].x + offset, corners[2].x - offset);
        float clampedY = Mathf.Clamp(touchPosition.y, corners[0].y + offset, corners[2].y - offset);

        return new Vector2(clampedX, clampedY);
    }

    void MakePaddle()
    {
        paddleLength = Vector2.Distance(childPaddle1.transform.position, childPaddle2.transform.position) + 0.35f;
        paddleCollider.size = new Vector2(paddleLength, 0.3f);
        paddleCollider.transform.position = (childPaddle1.transform.position + childPaddle2.transform.position) / 2;
        angle = Mathf.Atan2(Mathf.Abs(childPaddle2.transform.position.y - childPaddle1.transform.position.y), Mathf.Abs(childPaddle2.transform.position.x - childPaddle1.transform.position.x));

        if ((childPaddle1.transform.position.y < childPaddle2.transform.position.y && childPaddle1.transform.position.x > childPaddle2.transform.position.x) 
            || (childPaddle2.transform.position.y < childPaddle1.transform.position.y && childPaddle2.transform.position.x > childPaddle1.transform.position.x)) // for when the right finger is in the 2nd quadrant or when the left finger is in the 4th quadrant of the xy plane
        {
            angle *= -1;
        }

        if (childPaddle1.transform.position.x < paddleCollider.transform.position.x)
        {
            childPaddle2.transform.rotation = Quaternion.Euler(0, 0, 180);
            childPaddle1.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            childPaddle1.transform.rotation = Quaternion.Euler(0, 0, 180);
            childPaddle2.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        paddleCollider.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        AddParticleRenderer();

        paddleCollider.gameObject.SetActive(true);
    }

    void AddParticleRenderer()
    {
        particles.transform.rotation = paddleCollider.transform.rotation;
        particles.transform.position = paddleCollider.transform.position;
        particles.gameObject.SetActive(true);
        particleScale = new Vector3(paddleLength, 0, 0);
        particleShape.scale = particleScale;
    }
}
