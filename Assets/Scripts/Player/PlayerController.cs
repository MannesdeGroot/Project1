using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, Photon.Pun.IPunObservable
{
    public float sensitivity;
    public GameObject model;
    public GameObject modelHeadless;
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
    private bool powerJumped;
    private float jumpVel;
    [SerializeField] private float fallMultiplier;
    private bool jumping;
    [SerializeField] private float jumpMoveMultiplier;
    private float moveInputMultiplier;
    public bool stunned;
    private Vector3 input;
    [SerializeField] private float jumpDirMultiplier;
    public float normalFov;
    public float speedFov;
    public ParticleSystem jumpParticle;
    public ParticleSystem speedBoostParticle;
    public GameObject stunParticle;

    [Header("View")]
    [SerializeField] private Transform camTransform;
    [SerializeField] private float viewClamp;
    private Vector3 camDefaultPos;
    public Text pregameTimer;
    public Text roleText;
    public Text timerText;
    [SerializeField] private Color runnerColor, taggerColor;
    public Camera cam;
    private float mInputVert;
    public GameObject voteScreen;
    public Transform head;

    [Header("Interaction")]
    [SerializeField] private float tagDistance;
    public PowerUp powerUp;
    public Transform throwPos;
    public float forwardThrowForce;
    public float upwardsThrowForce;
    [Header("Multiplayer")]
    public string nickName;
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

        speed = moveSpeed;
        jumpVel = jumpVelocity;

        nickName = PhotonNetwork.NickName;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        camDefaultPos = cam.transform.localPosition;

        if (!pV.IsMine)
        {
            cam.gameObject.SetActive(false);
            modelHeadless.SetActive(false);
            anim = model.GetComponent<Animator>();
        }
        else
        {
            model.SetActive(false);
            anim = modelHeadless.GetComponent<Animator>();
        }
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

        if (jumping)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, head.position.z);
        }
        else
        {
            cam.transform.localPosition = camDefaultPos;
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

        animForwardSpeed = Mathf.Abs(Input.GetAxis("Horizontal") + Input.GetAxis("Vertical"));

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
            pV.RPC("PlayJump", RpcTarget.All);

            if (powerJumps > 0)
            {
                pV.RPC("PlaySpeed", RpcTarget.All, true);
                jumpVel *= powerJumpMultiplier;
                powerJumps--;
                powerJumped = true;
            }

            rb.velocity += Vector3.up * jumpVel + input * (jumpVel * jumpDirMultiplier);
            jumpVel = jumpVelocity;

            if (rb.velocity.y < 0)
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    [PunRPC]
    public void PlayJump()
    {
        jumpParticle.Play();
    }

    private void Tag()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, tagDistance))
        {
            if (hit.transform.tag == "Player")
            {
                PlayerController player = hit.transform.GetComponent<PlayerController>();

                if (isTagger && !player.isTagger && !player.invincible)
                {
                    isTagger = false;
                    player.PhotonTag(transform.position, 1);
                    SetTagger(false);
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
    }

    public void Eliminate()
    {
        Camera newCam = FindObjectOfType<Camera>();
        newCam.gameObject.SetActive(true);
        roleText.enabled = false;
        timerText.enabled = false;
        Destroy(gameObject);
    }

    public float GetTagKnockBack()
    {
        if (timer != 0)
        {
            return GameSettings.tagKnockBack * (GameSettings.roundTime / timer);
        }
        return 0;
    }

    public void ApplySpeedBoost(float multiplier, float duration) => StartCoroutine(ApplyBoost(multiplier, duration));

    private IEnumerator ApplyBoost(float multiplier, float duration)
    {
        speed *= multiplier;
        pV.RPC("PlaySpeed", RpcTarget.All, true);

        yield return new WaitForSeconds(duration);
        speed = moveSpeed;
        pV.RPC("PlaySpeed", RpcTarget.All, false);
    }

    [PunRPC]
    public void PlaySpeed(bool play)
    {
        if (play)
        {
            speedBoostParticle.Play();
        }
        else
        {
            speedBoostParticle.Stop();
        }
    }

    public IEnumerator Stun(float duration)
    {
        stunned = true;
        pV.RPC("PlayStun", RpcTarget.All, true);

        yield return new WaitForSeconds(duration);

        stunned = false;
        pV.RPC("PlayStun", RpcTarget.All, false);
    }

    [PunRPC]
    public void PlayStun(bool play)
    {
        stunParticle.SetActive(play);
    }

    public IEnumerator SetInvincible(float duration)
    {
        invincible = true;

        yield return new WaitForSeconds(duration);

        invincible = false;
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

    public void AnimationUpdate()
    {
        anim.SetBool("Tagger", isTagger);
        anim.SetBool("Tag", animTag);
        anim.SetBool("Throw", animThrow);
        anim.SetBool("Jump", jumping);
        anim.SetFloat("Speed", animForwardSpeed);
    }

    private void OnTriggerStay(Collider c)
    {
        jumping = false;
        moveInputMultiplier = 1;
        if (powerJumped)
        {
            powerJumped = false;
            pV.RPC("PlaySpeed", RpcTarget.All, false);
        }
    }

    private void OnTriggerExit(Collider c)
    {
        jumping = true;
        moveInputMultiplier = jumpMoveMultiplier;
    }
}
