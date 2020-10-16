using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.ComponentModel;

public class PlayerControl : MonoBehaviour
{
    // Start is called before the first frame update
    private string Horizontal;
    private string Vertical;

    private float moveHorizontal;
    private float moveVertical;
    private float CameraDistance;
    private float CameraSide;

    private float CurrentHorizontal;
    private float CurrentVertical;
    private float Current_speed;
    private float Current_CameraDistance;
    private float Current_CameraSide;

    public float RunScale;
    public float WalkSpeed;
    public float TurnSpeed;
    public float TimeScale;

    public bool run;
    public bool focus;
    public bool movement;

    private float TimeInterpolation;

    private Animator PlayerAnimator;
    private Rigidbody PlayerRigidbody;
    public CinemachineVirtualCamera cam;

    private float Rotation_x;
    private float Rotation_y;
    private float CurrentRotation_x;
    private float CurrentRotation_y;

    private float CurrentDamping;

    private float RotateScale;

    private Quaternion CurrentEular;

    private Transform FollowTarge;

    private void Awake()
    {
        if (!PlayerAnimator) { PlayerAnimator = GetComponent<Animator>(); }
        if (!PlayerRigidbody) { PlayerRigidbody = GetComponent<Rigidbody>(); }
    }
    void Start()
    {
        Horizontal = "Horizontal";
        Vertical = "Vertical";
        TimeInterpolation = 10;
        Current_speed = WalkSpeed;
        PlayerAnimator.SetBool("Grounded", true);
        CameraDistance = 1.75f;
        CameraSide = 0.65f;

        TimeScale = 0.75f;
        RotateScale = 1.0f;
        TimeInterpolation = 10;

        FollowTarge = transform.Find("Follow Target");
    }

    // Update is called once per frame
    void Update()
    {
        moveHorizontal = Input.GetAxis(Horizontal);
        moveVertical = Input.GetAxis(Vertical);

        run = Input.GetKey(KeyCode.LeftShift);
        focus = Input.GetKey(KeyCode.Mouse1);
        Rotateview();
        Focus();

        CurrentVertical = Mathf.Lerp(CurrentVertical, moveVertical, Time.deltaTime*TimeInterpolation);
        CurrentHorizontal = Mathf.Lerp(CurrentHorizontal, moveHorizontal, Time.deltaTime * TimeInterpolation);

        PlayerAnimator.SetFloat("MoveSpeed", moveVertical*Current_speed*8);
        transform.position += CurrentVertical * transform.forward * Current_speed ;
        transform.Rotate(0, CurrentHorizontal * TurnSpeed * Time.deltaTime, 0);
    }

    private void Rotateview()
    {
        if (moveHorizontal != 0 || moveVertical != 0) { movement = true; }
        else { movement = false; }

        if (focus) { RotateScale = 0.25f; }
        else { RotateScale = 2.0f; }

        if (!movement)
        {
            Rotation_x += Input.GetAxis("Mouse X"); 
            Rotation_y += Input.GetAxis("Mouse Y"); 
            CurrentRotation_x = Mathf.Lerp(CurrentRotation_x, Rotation_x, Time.deltaTime * TimeInterpolation * RotateScale);
            CurrentRotation_y = Mathf.Lerp(CurrentRotation_y, Rotation_y, Time.deltaTime * TimeInterpolation * RotateScale);
            
                if (CurrentRotation_x > 180)
                {
                    Rotation_x += -360;
                    CurrentRotation_x = Rotation_x;
                }
                if (CurrentRotation_x < -180)
                {
                    Rotation_x += 360;
                    CurrentRotation_x = Rotation_x;
                }
                if (focus)
                {
                    if (CurrentRotation_x > 30) { Rotation_x = 30; }
                    if (CurrentRotation_x < -30) { Rotation_x = -30; }
                    if (CurrentRotation_y > 13) { Rotation_y = 13; }
                    if (CurrentRotation_y < -11) { Rotation_y = -11; }
                }
                else
                {
                    if (CurrentRotation_y > 9) { Rotation_y = 9; }
                    if (CurrentRotation_y < -9) { Rotation_y = -9; }
                 }
            FollowTarge.localEulerAngles = new Vector3(CurrentRotation_y, CurrentRotation_x,FollowTarge.transform.localEulerAngles.z);
        }
        else
        { 
            CurrentEular = FollowTarge.transform.localRotation;
            if (CurrentEular != new Quaternion(0,0,0,1))
            {
               CurrentEular =Quaternion.Lerp(CurrentEular,new Quaternion(0,0,0,1), Time.deltaTime);
               FollowTarge.transform.localRotation = CurrentEular;
            }
            Rotation_x = FollowTarge.transform.localEulerAngles.y;
            CurrentRotation_x = FollowTarge.transform.localEulerAngles.y;
            Rotation_y = FollowTarge.transform.localEulerAngles.x;
            CurrentRotation_y = FollowTarge.transform.localEulerAngles.x;
        }
    }
    private void Focus()
    {
        Current_CameraDistance = cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance;
        Current_CameraSide = cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraSide;
        if (run && !focus)
        {
            Current_speed = WalkSpeed * RunScale;
            if (CurrentDamping != 0.05f) { CurrentDamping = Mathf.Lerp(CurrentDamping, 0.05f, Time.deltaTime); }
            if (Current_CameraDistance != 3f) { CameraDistance = Mathf.Lerp(CameraDistance, 3f, Time.deltaTime * TimeScale); }
            if (Current_CameraSide != 1.0f) { CameraSide = Mathf.Lerp(CameraSide, 1.0f, Time.deltaTime * TimeScale); }
        }
        else if (!focus)
        {
            Current_speed = WalkSpeed;
            if (CurrentDamping != 0.1f) { CurrentDamping = Mathf.Lerp(CurrentDamping, 0.1f, Time.deltaTime); }
            if (Current_CameraDistance != 1.25f) { CameraDistance = Mathf.Lerp(CameraDistance, 1.25f, Time.deltaTime * TimeScale); }
            if (Current_CameraSide != 0.60f) { CameraSide = Mathf.Lerp(CameraSide, 0.60f, Time.deltaTime * TimeScale); }
        }
        if (focus)
        {
            CameraDistance = Mathf.Lerp(CameraDistance, 0.55f, Time.deltaTime * TimeScale * 2);
            CameraSide = Mathf.Lerp(CameraSide, 0.85f, Time.deltaTime * TimeScale * 2);
        }
        cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().Damping = new Vector3(0.1f, 0.5f, CurrentDamping);
        cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = CameraDistance;
        cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraSide = CameraSide;
    }
}
