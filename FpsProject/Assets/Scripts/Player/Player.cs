using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerWeaponSystem weaponSystem;
    public Camera myCamera;
    public Transform myEyeTransform;
    public Transform myFeetTransform;
    public Vector3 lookingdir;

    float mouseMoveX = 0;
    float mouseMoveY = 0;

    public Rigidbody myRigidbody;

    public KeyCode forwardMoveKey;
    public KeyCode backMoveKey;
    public KeyCode leftMoveKey;
    public KeyCode rightMoveKey;
    public KeyCode sprintKey;
    public KeyCode jumpKey;
    public float moveSpeed;
    public float sprintIncreaseValue;
    public float jumpPower;
    public bool isGrond;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        CheckFeet();
        if (isGrond && Input.GetKeyDown(jumpKey))
        {
            Jump();
        }

        MouseMove();

        if (Input.GetMouseButtonDown(0))
        {
            weaponSystem.Trigger(true);
        }
    }

    void CheckFeet()
    {
        Ray ray = new Ray(myFeetTransform.position, Vector3.down);
        RaycastHit hit;
        int mask = 1 << LayerMask.NameToLayer("Ground");

        if (Physics.Raycast(ray, out hit, 0.1f, mask))
        {
            isGrond = true;
            return;
        }

        isGrond = false;
    }

    void MouseMove()
    {
        mouseMoveX += Input.GetAxis("Mouse X");
        mouseMoveY += -Input.GetAxis("Mouse Y");
        mouseMoveY = Mathf.Clamp(mouseMoveY, -60f, 60f);
        if (mouseMoveX > 360f)
        {
            mouseMoveX -= 360f;
        }
        if (mouseMoveX < -360f)
        {
            mouseMoveX += 360f;
        }

        Vector3 mouseMoveVector = new Vector3(mouseMoveY, mouseMoveX, 0);

        myCamera.transform.localEulerAngles = mouseMoveVector;
        transform.localRotation = new Quaternion(0, myCamera.transform.localRotation.y, 0, myCamera.transform.localRotation.w);

        lookingdir = myCamera.transform.localRotation * Vector3.forward;
        myCamera.transform.position = myEyeTransform.position;

        Quaternion newQua = Quaternion.LookRotation(lookingdir.normalized);
        weaponSystem.inUseWeapon.gameObject.transform.rotation = newQua;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(myEyeTransform.position, lookingdir * 10);
    }

    void Move()
    {
        float moveX = 0;
        float moveZ = 0;
        if (Input.GetKey(forwardMoveKey)) moveZ++;
        if (Input.GetKey(backMoveKey)) moveZ--;
        if (Input.GetKey(rightMoveKey)) moveX++;
        if (Input.GetKey(leftMoveKey)) moveX--;

        if (moveX != 0 && moveZ != 0)
        {
            moveX *= 0.75f;
            moveZ *= 0.75f;
        }

        float speed = moveSpeed;
        if (Input.GetKey(sprintKey))
        {
            speed *= sprintIncreaseValue;
        }

        Vector3 locVel = myRigidbody.velocity;
        locVel.x = moveX * speed;
        locVel.z = moveZ * speed;
        myRigidbody.velocity = transform.TransformDirection(locVel);
    }

    void Jump()
    {
        Vector3 vector = myRigidbody.velocity;
        vector.y = jumpPower;
        myRigidbody.velocity = vector;
    }
}
