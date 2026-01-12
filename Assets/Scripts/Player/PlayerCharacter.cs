using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    [HideInInspector] public CharacterController controller;

    private float xRotation;
    
    private float lockedYPosition; 

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lookSensitivity = 0.2f;

    [Header("Inputs")]
    [SerializeField] private InputActionReference moveActionRef;
    [SerializeField] private InputActionReference lookActionRef;
    [SerializeField] private InputActionReference punchActionRef;
    [SerializeField] private InputActionReference interactActionRef;
    [SerializeField] private float punchRange = 2f;

    [Header("Camera")]
    [SerializeField] private Transform cam;

    [Header("Player Stock")]
    public string stock;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction punchAction;
    private InputAction interactAction;

    private bool brokeSomething = false;
    private bool killedSomeone = false;

    [SerializeField] private AudioPlayerPool audioPool;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        moveAction = moveActionRef.action;
        lookAction = lookActionRef.action;
        punchAction = punchActionRef.action;
        interactAction = interactActionRef.action;
        
        lockedYPosition = transform.position.y;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        punchAction.Enable();
        interactAction.Enable();
        punchAction.performed += ctx => Punch();
        interactAction.performed += ctx => Interact();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        punchAction.Disable();
        interactAction.Disable();
        punchAction.performed -= ctx => Punch();
        interactAction.performed -= ctx => Interact();
    }

    private void Update()
    {
        if (!controller.enabled)
            return;

        RotateCamera();
        MovePlayer();
        CheckCursorState();
    }

    private void CheckCursorState()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, punchRange))
        {
            if (hit.collider.GetComponent<IInteractible>() != null)
            {
                Finder.LevelManager.inGameUI.SwitchToFistTalkCursor();
                return;
            }
            else if (hit.collider.GetComponent<IBreakable>() != null)
            {
                Finder.LevelManager.inGameUI.SwitchToFistCursor();
                return;
            }
        }
        Finder.LevelManager.inGameUI.SwitchToNormalCursor();
    }

    private void Punch()
    {
        if (!controller.enabled)
            return;
        
        Finder.LevelManager.inGameUI.PlayPunchAnimation();
        audioPool.PlaySound();
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, punchRange))
        {
            var breakable = hit.collider.GetComponent<IBreakable>();
            if (breakable != null)
            {
                breakable.Break();
                if (breakable is Character && !killedSomeone)
                {
                    Finder.SongManager.PlayTrack(2);
                    brokeSomething = true;
                    killedSomeone = true;
                }
                if (!brokeSomething)
                {
                    Finder.SongManager.PlayNext();
                    brokeSomething = true;
                }
            }
        }
    }

    private void Interact()
    {
        if (!controller.enabled)
            return;

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, punchRange))
        {
            hit.collider.GetComponent<IInteractible>()?.Interact(this);
        }
    }

    private void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up);

        Vector3 moveDir = forward * input.y + right * input.x;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        controller.Move(moveDir * speed * Time.deltaTime);

        controller.transform.position = new Vector3(
            controller.transform.position.x,
            lockedYPosition,
            controller.transform.position.z
        );
    }

    private void RotateCamera()
    {
        Vector2 look = lookAction.ReadValue<Vector2>() * lookSensitivity;

        xRotation -= look.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * look.x, Space.World);
    }
}