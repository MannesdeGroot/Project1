using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, Photon.Pun.IPunObservable
{
    public float sensitivity;
    private Rigidbody rb;
    private MiniGame game;
    private Animator anim;
    private int timer;
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
    [SerializeField] private float jumpMoveMultiplier;
    private float moveInputMultiplier;
    public bool stunned;
    private Vector3 input;
    [SerializeField] private float jumpDirMultiplier;

    [Header("View")]
    [SerializeField] private Transform camTransform;
    [SerializeField] private float viewClamp;
    [SerializeField] private Text roleText;
    public Text timerText;
    [SerializeField] private Color runnerColor, taggerColor;
    public Camera cam;
    private float mInputVert;

    [Header("Interaction")]
    [SerializeField] private float tagDistance;
    public PowerUp powerUp;
    public Transform throwPos;
    public float forwardThrowForce;
    public float upwardsThrowForce;
    [Header("Multiplayer")]
    public PhotonView pV;
    public bool isTagger;
    public bool invincible;
    public Image powerupImage;
    [Header("Animations")]
    private float animForwardSpeed;
    private bool animTag;
    public bool animThrow;

    void Start()
    {
        pV = transform.GetComponent<PhotonView>();

        rb = GetComponent<Rigidbody>();
        game = FindObjectOfType<MiniGame>();
        anim = GetComponentInChildren<Animator>();

        speed = moveSpeed;
        jumpVel = jumpVelocity;

        //Temp
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isTagger);
            stream.SendNext(invincible);
            stream.SendNext(powerupImage);
            stream.SendNext(animForwardSpeed);
            stream.SendNext(animTag);
            stream.SendNext(animThrow);
            stream.SendNext(jumping);
        }
        else if (stream.IsReading)
        {
            isTagger = (bool)stream.ReceiveNext();
            invincible = (bool)stream.ReceiveNext();
            powerupImage = (Image)stream.ReceiveNext();
            animForwardSpeed = (float)stream.ReceiveNext();
            animTag = (bool)stream.ReceiveNext();
            animThrow = (bool)stream.ReceiveNext();
            jumping = (bool)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (pV.IsMine)
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
                    if (powerUp is DodgeballItem)
                        animThrow = true;

                    powerUp.Use();
                    powerUp = null;
                    powerUpUiElement.SetActive(false);
                }
            }
        }
        else
        {
            cam.gameObject.SetActive(false);
        }
        AnimationUpdate();
    }

    void FixedUpdate()
    {
        if (pV.IsMine)
        {
            if (stunned) return;

            Move();
        }
    }

    private void Move()
    {
        input.x = Input.GetAxis("Horizontal") * moveInputMultiplier * Time.deltaTime * speed;
        input.z = Input.GetAxis("Vertical") * moveInputMultiplier * Time.deltaTime * speed;

        animForwardSpeed = input.magnitude;
        pV.RPC("SpeedAnim", RpcTarget.All);

        transform.Translate(input);
    }

    private void Rotate()
    {
        float rotHor = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        mInputVert -= Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

        mInputVert = Mathf.Clamp(mInputVert, -viewClamp, viewClamp);

        transform.Rotate(0, rotHor, 0);
        camTransform.localRotation = Quaternion.Euler(mInputVert, 0, 0);
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

            rb.velocity += Vector3.up * jumpVel + input * (jumpVel * jumpDirMultiplier);
            jumpVel = jumpVelocity;
            pV.RPC("AnimationUpdate", RpcTarget.All);

            if (rb.velocity.y < 0)
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    PlayerController player;
    private void Tag()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, tagDistance))
        {
            if (hit.transform.tag == "Player")
            {
                player = hit.transform.GetComponent<PlayerController>();

                if (/*game is TagGame &&*/ isTagger && !player.isTagger)
                {
                    //tag = (TagGame)game;

                    //tag.TagPlayer(pV.ViewID.ToString(), player.pV.ViewID.ToString());

                    isTagger = false;
                    player.PhotonTag(transform.position, 1);
                    pV.RPC("AnimationUpdate", RpcTarget.All);
                }
            }

        }
    }

    public void PhotonTag(Vector3 taggerPos, float knockback)
    {
        pV.RPC("Tagged", RpcTarget.All, taggerPos, knockback);
    }

    [PunRPC]
    public void Tagged(Vector3 taggerPos, float knockBackMultiplier)
    {
        print("punrpcTagged");
        print(taggerPos + "   " + knockBackMultiplier);
        SetTagger(true);
        rb.AddForce((transform.position - taggerPos) * GetTagKnockBack() * knockBackMultiplier);
    }

    public void SetTagger(bool value)
    {
        print("setTagger");
        isTagger = value;

        if (roleText == null) return;
        roleText.color = isTagger ? taggerColor : runnerColor;
        roleText.text = isTagger ? "Tagger" : "Runner";

        if (isTagger)
            pV.RPC("AnimationUpdate", RpcTarget.All);
    }

    public void Eliminate()
    {
        Camera newCam = FindObjectOfType<Camera>();
        newCam.gameObject.SetActive(true);
        Destroy(gameObject);
    }

    public float GetTagKnockBack()
    {
        if (timer != 0)
        {
            return GameSettings.tagKnockBack * (GameSettings.eliminationTime / timer);
        }
        return 0;
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

    [PunRPC]
    public void AnimationUpdate()
    {
        anim.SetBool("Tagger", isTagger);
        anim.SetBool("Tag", animTag);
        anim.SetBool("Throw", animThrow);
        anim.SetBool("Jump", jumping);
    }

    [PunRPC]
    public void SpeedAnim()
    {
        anim.SetFloat("Speed", animForwardSpeed);
    }

    private void OnTriggerEnter(Collider c)
    {
        jumping = false;
        moveInputMultiplier = 1;
    }

    private void OnTriggerExit(Collider c)
    {
        jumping = true;
        moveInputMultiplier = jumpMoveMultiplier;
    }
}
