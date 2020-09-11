using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, Photon.Pun.IPunObservable
{
    private Rigidbody rb;
    private MiniGame game;
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
    public bool stunned;

    [Header("View")]
    [SerializeField] private Transform cam;
    [SerializeField] private float viewClamp;
    [SerializeField] private Text roleText;
    [SerializeField] private Text timerText;
    [SerializeField] private Color runnerColor, taggerColor;
    public Camera camera;
    private float mInputVert;

    [Header("Interaction")]
    [SerializeField] private float tagDistance;
    public PowerUp powerUp;
    public Transform throwPos;
    public float throwForce;
    [Header("Multiplayer")]
    public PhotonView pV;
    public bool isTagger;
    public bool invinceble;
    public Image powerupImage;
    [Header("Animations")]
    public bool idle;

    void Start()
    {
        pV = transform.GetComponent<PhotonView>();

        rb = GetComponent<Rigidbody>();
        game = FindObjectOfType<MiniGame>();

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
            stream.SendNext(invinceble);
            stream.SendNext(powerupImage);
            stream.SendNext(idle);

        }
        else if (stream.IsReading)
        {
            isTagger = (bool)stream.ReceiveNext();
            invinceble = (bool)stream.ReceiveNext();
            powerupImage = (Image)stream.ReceiveNext();
            idle = (bool)stream.ReceiveNext();
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
                    powerUp.Use();
                    powerUp = null;
                    powerUpUiElement.SetActive(false);
                }
            }
        }
        else
        {
            camera.gameObject.SetActive(false);
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

    TagGame tag;
    PlayerController player;
    private void Tag()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, tagDistance))
        {
            if (hit.transform.tag == "Player")
            {
                player = hit.transform.GetComponent<PlayerController>();

                if (game is TagGame && isTagger && !player.isTagger)
                {
                    tag = (TagGame)game;

                    //de line hieronder moet nog in "Tagged", maar weet niet hoe, pls fix.
                    tag.TagPlayer(this, player);

                    isTagger = false;
                    player.pV.RPC("Tagged", RpcTarget.All, transform.position, 1);
                }
            }

        }
    }

    [PunRPC]
    public void Tagged(Vector3 taggerPos, float knockBackMultiplier)
    {
        SetTagger(true);
        rb.AddForce((transform.position - taggerPos) * GetTagKnockBack() * knockBackMultiplier);
    }

    public void SetTagger(bool value)
    {
        isTagger = value;

        if (roleText == null) return;
        roleText.color = isTagger ? taggerColor : runnerColor;
        roleText.text = isTagger ? "Tagger" : "Runner";

        timerText.gameObject.SetActive(isTagger);

        if (isTagger)
        {
            timer = GameSettings.eliminationTime;
            StartCoroutine(CountDown());
        }
        else
        {
            StopCoroutine(CountDown());
        }
    }

    private void EliminatePlayer()
    {

    }

    private IEnumerator CountDown()
    {
        if (timer <= 0)
            EliminatePlayer();

        if (timer >= 0)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            string secondsText = seconds < 10 ? $"0{seconds}" : seconds.ToString();
            timerText.text = $"{minutes}:{secondsText}";
        }

        yield return new WaitForSeconds(1);
        timer--;

        StartCoroutine(CountDown());
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

    void AnimationUpdate()
    {

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
