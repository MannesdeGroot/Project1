﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private MiniGame game;
    private Player self;
    [SerializeField] private GameObject powerUpUiElement;
    [SerializeField] private Image powerUpIcon;
    [SerializeField] private Text powerUpName;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float powerJumpMultiplier;
    public int powerJumps;
    private float jumpVel;
    [SerializeField] private float fallMultiplier;
    private bool jumping;
    public bool stunned;

    [Header("View")]
    [SerializeField] private Transform cam;
    [SerializeField] private float viewClamp;
    private float mInputVert;

    [Header("Interaction")]
    [SerializeField] private float tagDistance;
    public PowerUp powerUp;
    public Transform throwPos;
    public float throwForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        game = FindObjectOfType<MiniGame>();
        self = GetComponent<Player>();

        speed = moveSpeed;
        jumpVel = jumpVelocity;

        //Temp
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (stunned) return;

        Rotate();

        if (Input.GetButtonDown("Fire1"))
            Tag();
        if (Input.GetButtonDown("Jump"))
            Jump();
        if (Input.GetButtonDown("Use"))
        {
            if (powerUp != null)
            {
                powerUp.Use();
                powerUp = null;
                powerUpUiElement.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        if (stunned) return;

        Move();
    }

    private void Move()
    {
        float inputX = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float inputZ = Input.GetAxis("Vertical") * Time.deltaTime * speed;

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
            if (powerJumps > 0)
            {
                jumpVel *= powerJumpMultiplier;
                powerJumps--;
            }

            rb.velocity += Vector3.up * jumpVel;
            jumpVel = jumpVelocity;

            if (rb.velocity.y < 0)
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void Tag()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, tagDistance))
        {
            Player player = hit.transform.GetComponent<Player>();

            if (game is TagGame && self.isTagger)
            {
                TagGame tag = (TagGame)game;
                tag.TagPlayer(self, player, self.GetTagKnockBack());
            }
        }
    }

    public void ApplySpeedBoost(float multiplier, float duration) => StartCoroutine(ApplyBoost(multiplier, duration));

    private IEnumerator ApplyBoost(float multiplier, float duration)
    {
        speed *= multiplier;

        yield return new WaitForSeconds(duration);
        speed = moveSpeed;
    }

    public IEnumerator Stun(float duration)
    {
        stunned = true;

        yield return new WaitForSeconds(duration);

        stunned = false;
    }

    public void AddPowerUp(PowerUp type)
    {
        if (powerUp == null)
        {
            powerUp = type;
            powerUpName.text = powerUp.powerUpName;
            powerUpIcon.sprite = powerUp.icon;
            powerUpUiElement.SetActive(true);
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