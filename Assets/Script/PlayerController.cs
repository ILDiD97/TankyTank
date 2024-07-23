using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IShootable
{
    [SerializeField]
    private PlayerInput input;

    [SerializeField] 
    private GameObject cameraPivot;

    [SerializeField] 
    private GameObject projectile;

    [SerializeField] 
    private Transform turret;

    [SerializeField] 
    private Transform cannon;

    [SerializeField]
    private Transform tank;

    [SerializeField]
    private AudioSource shooting;

    [SerializeField]
    private LayerMask mask;
    
    private bool dead;

    [SerializeField]
    [Range(0,90)]
    private float rotationSpeed;

    [SerializeField]
    [Range(0, 10)]
    private float speed;

    [SerializeField]
    [Range(0, 10)]
    private float accelleration;

    [SerializeField]
    [Range(0,10)]
    private float accellerationLerpSpeed;

    private float accellerationLerp;


    [SerializeField]
    private float projectileAccelleration;

    [SerializeField]
    private float cameraRotationSpeed;

    [SerializeField]
    private int health;

    public bool Dead { get => dead; set => dead = value; }
    public int Health { get => health; set => health = value; }

    private void Awake()
    {
        transform.position = new Vector3(500, 0, 0);
        input.actions["Fire"].started += Shooting;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead)
        {
            //if (Input.GetMouseButtonDown(0))
            //{
                //Shoot();
                //shooting.Play();
            //}
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, mask))
            {
                Vector3 lookPoint = new Vector3(hit.point.x, turret.position.y, hit.point.z);
                turret.LookAt(lookPoint);
            }
            Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveVector = cameraPivot.transform.TransformDirection(moveVector);
            PlayerMoving(moveVector);
            TankMoving(moveVector);
            RotateCamera();
            //Floating();
        }
    }

    private void Shooting(InputAction.CallbackContext context)
    {
        if (!Dead)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        GameObject ammo = Instantiate(projectile, cannon.position, transform.localRotation);
        ammo.GetComponent<Rigidbody>().AddRelativeForce(cannon.forward * projectileAccelleration, ForceMode.Impulse);
        ammo.GetComponent<Projectile>().spawnerName = gameObject.name;
        shooting.Play();
    }

    private void PlayerMoving(Vector3 move)
    {
        float accelleration;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            accelleration = Accelleration(true);
        }
        else
        {
            accelleration = Accelleration(false);
        }
        if ((move.x > 0 || move.x < 0) && (move.z > 0 || move.z < 0))
        {
            transform.position = new Vector3(transform.position.x  + move.x * (speed * accelleration / 2) * Time.deltaTime, 
                0, transform.position.z + move.z * (speed * accelleration / 2) * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + move.x * speed * accelleration * Time.deltaTime, 
                0, transform.position.z + move.z * speed * accelleration * Time.deltaTime);
        }
    }
    private void TankMoving(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            tank.forward = Vector3.Lerp(tank.forward, direction, rotationSpeed * Time.deltaTime);
        }
    }

    private float Accelleration(bool accellerating)
    {
        if (accellerating)
        {
            if(accellerationLerp >= 1)
            {
                accellerationLerp = 1;
            }
            else
            {
                accellerationLerp += accellerationLerpSpeed * Time.deltaTime;
            }
        }
        else
        {
            if(accellerationLerp <= 0)
            {
                accellerationLerp = 0;
            }
            else
            {
                accellerationLerp -= accellerationLerpSpeed * Time.deltaTime;
            }
        }
        
        return Mathf.Lerp(1, accelleration, accellerationLerp);
    }

    private void RotateCamera()
    {
        float angle = Time.deltaTime * cameraRotationSpeed;
        if (Input.GetKey(KeyCode.E))
        {
            cameraPivot.transform.Rotate(new Vector3(0, angle, 0));
        }

        if (Input.GetKey(KeyCode.Q))
        {
            cameraPivot.transform.Rotate(new Vector3(0, -angle, 0));
        }
    
    }

    //public void Floating()
    //{
    //    tank.localPosition = new Vector3(0, Mathf.Sin(Time.time * floatingSpeed) * escurtion, 0);
    //}
}
