using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, Photon.Pun.IPunObservable
{
    public float sensitivity;
    public GameObject model;
    public GameObject modelHeadless;
    private Rigidbody rb;
    private MiniGame game;
    private Animator anim;
    [SerializeField] private GameObject powerUpUiElement;
    [SerializeField] private Image powerUpIcon;
    [SerializeField] private Text powerUpName;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float jumpForwardVelocity;
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
    public ParticleSystem tagParticle;
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
    public GameObject menu;

    [Header("Interaction")]
    [SerializeField] private float tagDistance;
    public PowerUp powerUp;
    public Transform throwPos;
    public float forwardThrowForce;
    public float upwardsThrowForce;
    [Header("Multiplayer")]
    public int team;
    private List<PlayerController> players;
    public string nickName;
    public Transform nameTag;
    public Text nickNameText;
    private Transform nameTagTarget;
    public PhotonView pV;
    public bool isTagger;
    public bool canTag;
    public float tagCooldown;
    public bool invincible;
    public Image powerupImage;
    [Header("Animations")]
    private float animForwardSpeed;
    private bool animTag;
    private bool animJump;
    public bool animThrow;
    [Header("Custimization")]
    public Transform hatPos;
    public int currentHat;
    public List<GameObject> hats;
    GameObject currentHatObj;
    public SkinnedMeshRenderer meshFull;
    public SkinnedMeshRenderer meshHeadless;
    public Color team1Color;
    public Color team1ColorAccent;
    public Color team2Color;
    public Color team2ColorAccent;

    void Start()
    {
        pV = transform.GetComponent<PhotonView>();

        rb = GetComponent<Rigidbody>();
        game = FindObjectOfType<MiniGame>();

        speed = moveSpeed;
        jumpVel = jumpVelocity;

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

        players = FindObjectsOfType<PlayerController>().ToList();

        nickName = pV.Owner.NickName;
        UpdateNames();
    }

    public void UpdateNames()
    {
        if (!pV.IsMine)
        {
            nickName = pV.Owner.NickName;
            nickNameText.text = nickName;
            pV.RPC("UpdateNameColors", RpcTarget.All);
        }
    }

    [PunRPC]
    public void UpdateNameColors()
    {
        players = FindObjectsOfType<PlayerController>().ToList();

        foreach (PlayerController player in players)
        {
            player.nickNameText.color = player.isTagger ? taggerColor : Color.white;
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
            stream.SendNext(nickName);
            stream.SendNext(currentHat);
            stream.SendNext(team);
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
            nickName = (string)stream.ReceiveNext();
            currentHat = (int)stream.ReceiveNext();
            team = (int)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (pV.IsMine)
        {
            if (stunned) return;

            if (!menu.active)
            {
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

            if (Input.GetButtonDown("Cancel"))
            {
                TurnMenuOnOff();
            }
        }
        else
        {
            if (currentHatObj == null && currentHat > 0)
            {
                CustomizationUpdate();

            }
        }

        if (jumping)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, head.transform.position.y, head.position.z);
        }
        else
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, camDefaultPos, 5);
        }

        AnimationUpdate();

        foreach (PlayerController player in players)
        {
            if (player != null && pV.IsMine)
            {
                players = FindObjectsOfType<PlayerController>().ToList();
                Vector3 newDir = Vector3.RotateTowards(player.nameTag.forward, new Vector3(transform.position.x, player.nameTag.position.y, transform.position.z) - player.nameTag.position, Time.deltaTime * 100, 0);
                player.nameTag.rotation = Quaternion.LookRotation(newDir);
            }
        }
    }

    void FixedUpdate()
    {
        if (pV.IsMine)
        {
            if (stunned) return;
            if (!menu.active)
            {
                Move();

            }
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
            animJump = true;
            pV.RPC("PlayJump", RpcTarget.All);

            if (powerJumps > 0)
            {
                pV.RPC("PlaySpeed", RpcTarget.All, true);
                jumpVel *= powerJumpMultiplier;
                powerJumps--;
                powerJumped = true;
            }

            rb.velocity += Vector3.up * jumpVel + input * (jumpVel * jumpDirMultiplier);
            rb.AddForce(Vector3.forward * jumpForwardVelocity);
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
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, tagDistance))
        {
            if (hit.transform.tag == "Player")
            {
                PlayerController player = hit.transform.GetComponent<PlayerController>();

                if (isTagger && !player.isTagger && !player.invincible)
                {
                    isTagger = false;
                    player.PhotonTag(transform.position, 1);
                    SetTagger(false);
                    tagParticle.transform.position = hit.point;
                    tagParticle.Play();
                    StartCoroutine(TagCooldown());
                }
            }
        }
    }

    private IEnumerator TagCooldown()
    {
        canTag = false;
        yield return new WaitForSeconds(tagCooldown);
        canTag = true;
    }

    public void PhotonTag(Vector3 taggerPos, float knockback)
    {
        pV.RPC("Tagged", RpcTarget.All, taggerPos, knockback);
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

        UpdateNames();
        roleText.color = isTagger ? taggerColor : runnerColor;
        roleText.text = isTagger ? "Tagger" : "Runner";
    }

    [PunRPC]
    public void photonEliminate()
    {
        Camera newCam = FindObjectOfType<Camera>();
        newCam.gameObject.SetActive(true);
        if (roleText == null) return;
        roleText.enabled = false;
        timerText.enabled = false;
        Destroy(gameObject);
    }

    public void Eliminate()
    {
        pV.RPC("photonEliminate", RpcTarget.All);
    }

    public float GetTagKnockBack()
    {
        TagManager tag = FindObjectOfType<TagManager>();
        float timer = tag.timer;
        if (timer != 0)
        {
            return GameSettings.tagKnockBack * (tag.roundTime / timer);
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
        anim.SetBool("Jump", animJump);
        anim.SetFloat("Speed", animForwardSpeed);
    }


    public void CustomizationUpdate()
    {
        for (int i = 0; i < hatPos.childCount; i++)
        {
            Destroy(hatPos.GetChild(i).gameObject);
        }
        if (currentHat > 0)
        {
            currentHatObj = Instantiate(hats[currentHat], hatPos);
        }
    }

    public void TurnMenuOnOff()
    {
        menu.SetActive(!menu.active);
        Cursor.visible = menu.active;
        if (menu.active)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void TeleportPlayer(Vector3 teleportLoc)
    {
        pV.RPC("PUNTeleportPlayer", RpcTarget.All, teleportLoc);
    }

    [PunRPC]
    public void PUNTeleportPlayer(Vector3 teleportLoc)
    {
        transform.position = teleportLoc;
    }

    public void SetTeam(int teamToSet)
    {
        pV.RPC("PUNSetTeam", RpcTarget.All, teamToSet);
    }

    [PunRPC]
    public void PUNSetTeam(int teamToSet)
    {
        team = teamToSet;
        if(team == 1)
        {
            meshFull.materials[0].color = team1Color;
            meshFull.materials[1].color = team1ColorAccent;
            meshHeadless.materials[0].color = team1Color;
            meshHeadless.materials[1].color = team1ColorAccent;
        }
        else if(team == 2)
        {
            meshFull.materials[0].color = team2Color;
            meshFull.materials[1].color = team2ColorAccent;
            meshHeadless.materials[0].color = team2Color;
            meshHeadless.materials[1].color = team2ColorAccent;
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (otherPlayer.IsMasterClient || PhotonNetwork.PlayerList.Length < 2)
        {
            LeaveRoom();
        }
    }

    public void LeaveRoom()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.DestroyPlayerObjects(pV.Owner);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);

    }
    private void OnTriggerStay(Collider c)
    {
        jumping = false;
        animJump = false;
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
