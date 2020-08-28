using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    private MiniGame game;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpVelocity;
    private bool jumping;
    public bool stunned;

    [Header("View")]
    [SerializeField] private Transform cam;
    [SerializeField] private float viewClamp;
    private float mInputVert;

    [Header("Interaction")]
    [SerializeField] private float tagDistance;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        game = FindObjectOfType<MiniGame>();

        //Temp
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        if (stunned) return;

        Move();
        Rotate();

        if (Input.GetButtonDown("Jump"))
            Jump();
        if (Input.GetButtonDown("Fire1"))
            Tag();
    }

    private void Move()
    {
        float inputX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float inputZ = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        transform.Translate(inputX, 0, inputZ);
    }

    private void Rotate()
    {
        float rotHor = Input.GetAxis("Mouse X") * Time.deltaTime * Settings.sensitivity;
        mInputVert -= Input.GetAxis("Mouse Y") * Time.deltaTime * Settings.sensitivity;

        mInputVert = Mathf.Clamp(mInputVert, -viewClamp, viewClamp);

        transform.Rotate(0, rotHor, 0);
        cam.localRotation = Quaternion.Euler(mInputVert, 0, 0);
    }

    private void Jump()
    {
        if (!jumping)
        {
            rb.velocity += new Vector3(0, jumpVelocity, 0);
        }
    }

    private void Throw()
    {

    }

    private void Tag()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, tagDistance))
        {
            Player player = hit.transform.GetComponent<Player>();
            
            if(game is TagGame)
            {
                TagGame tag = (TagGame)game;
                tag.TagPlayer(GetComponent<Player>(), player);
            }
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        jumping = false;
    }

    private void OnTriggerExit(Collider c)
    {
        jumping = true;
    }
}
