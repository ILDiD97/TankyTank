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
    private Camera mainCameraobj;

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

    [SerializeField]
    private Vector3 moveVector;

    private bool dead;

    [SerializeField]
    [Range(0,90)]
    private float rotationSpeed;

    [SerializeField]
    [Range(0, 90)]
    private float turretRotationSpeed;

    [SerializeField]
    [Range(0, 50)]
    private float speed;

    [SerializeField]
    [Range(0, 5)]
    private float accelleration;

    [SerializeField]
    [Range(0,10)]
    private float accellerationLerpSpeed;

    //[SerializeField]
    private float accellerationLerp;

    //[SerializeField]
    private bool canAccellerate;

    private bool canShoot;

    [SerializeField]
    private float projectileAccelleration;

    [SerializeField]
    private float cameraRotationSpeed;

    [SerializeField]
    private float cameraRotateDirection;

    [SerializeField]
    private int health;

    [SerializeField]
    private Vector2 inputTurret;

    public bool Dead { get => dead; set => dead = value; }
    public int Health { get => health; set => health = value; }

    private void Awake()
    {
        transform.position = new Vector3(500, 0, 0);
        SetUpInput();
    }

    private void SetUpInput()
    {
        input = FindFirstObjectByType<PlayerInput>();
        input.actions["Move"].started += Move;
        input.actions["Move"].performed += Move;
        input.actions["Move"].canceled += Move;
        input.actions["Accelleration"].started += Accellerate;
        input.actions["Accelleration"].canceled += Accellerate;
        input.actions["RotateTurret"].started += RotateTurret;
        input.actions["RotateTurret"].performed += RotateTurret;
        input.actions["RotateTurret"].canceled += RotateTurret;
        input.actions["Look"].started += CameraDirection;
        input.actions["Look"].performed += CameraDirection;
        input.actions["Look"].canceled += CameraDirection;
        input.actions["FirePrincipal"].started += Shooting;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead && GameStatus.Instance.IsGameOn)
        {
            //if (Physics.Raycast(mainCameraobj.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, mask))
            //{
            //    //print(Input.mousePosition);
            //    Vector3 lookPoint = new Vector3(hit.point.x, turret.position.y, hit.point.z);
            //    turret.LookAt(lookPoint);
            //}

            Vector3 direction = mainCameraobj.transform.TransformDirection(moveVector);
            PlayerMoving(direction);
            TankMoving(direction);
            //TurretRotation();
            RotateCamera();
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        if (!Dead && GameStatus.Instance.IsGameOn)
        {
            if (Physics.Raycast(mainCameraobj.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, mask))
            {
                //print(Input.mousePosition);
                Vector3 lookPoint = new Vector3(hit.point.x, turret.position.y, hit.point.z);
                turret.LookAt(lookPoint);
            }
        }
    }

    private void Shooting(InputAction.CallbackContext context)
    {
        if (!Dead && GameStatus.Instance.IsGameOn)
        {
            canShoot = true;
        }
    }

    public void Shoot()
    {
        if (canShoot)
        {
            GameObject ammo = Instantiate(projectile, cannon.position, transform.localRotation);
            ammo.GetComponent<Rigidbody>().AddRelativeForce(cannon.forward * projectileAccelleration, ForceMode.Impulse);
            ammo.GetComponent<Projectile>().spawnerName = gameObject.name;
            shooting.Play();
            canShoot = false;
        }
    }

    private void Move(InputAction.CallbackContext context)
    {
        Vector2 inputContext = context.ReadValue<Vector2>();
        moveVector = new Vector3(inputContext.x, 0, inputContext.y);
    }

    private void Accellerate(InputAction.CallbackContext context)
    {
        canAccellerate = !canAccellerate;
    }

    private void RotateTurret(InputAction.CallbackContext context)
    {
        Vector2 input  = context.ReadValue<Vector2>();
        
        inputTurret = input;
            //new Vector2(input.x + Screen.width, input.y + Screen.height);
    }

    private void TurretRotation()
    {
        //if (Physics.Raycast(mainCameraobj.ScreenPointToRay(inputTurret), out RaycastHit hit, 100, mask))
        //{
        //    Vector3 lookPoint = new Vector3(hit.point.x, turret.position.y, hit.point.z);
        //    turret.LookAt(lookPoint);
        //}
        if(input.actions["RotateTurret"].phase == InputActionPhase.Started
            || input.actions["RotateTurret"].phase == InputActionPhase.Performed)
        {
            // Calculate the direction from the tank to the mouse position
            Vector2 direction = (inputTurret - (Vector2)transform.position).normalized;
            Debug.Log("Direction: " + direction);
            // Rotate the turret to face the direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(0, angle, 0);
        }

    }

    private void PlayerMoving(Vector3 move)
    {
        float accelleration = Accelleration(canAccellerate);
        //Vector3 mover;
        //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        //{
        //    accelleration = Accelleration(true);
        //}
        //else
        //{
        //    accelleration = Accelleration(false);
        //}

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
        Vector3 rightDirection = new Vector3(direction.x, 0, direction.z);
        if (direction != Vector3.zero)
        {
            tank.forward = Vector3.Lerp(tank.forward, rightDirection, rotationSpeed * Time.deltaTime);
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

    private void CameraDirection(InputAction.CallbackContext context)
    {
        cameraRotateDirection = context.ReadValue<float>();
    }

    private void RotateCamera()
    {
        float angle = Time.deltaTime * cameraRotationSpeed * cameraRotateDirection;
        cameraPivot.transform.Rotate(new Vector3(0, angle, 0));
    }

}
