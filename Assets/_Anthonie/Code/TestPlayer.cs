using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float speed;
    public float sencitivity;
    public Camera camera;
    private PhotonView PV;

    void Start()
    {
        PV = transform.GetComponent<PhotonView>();
        camera.gameObject.SetActive(true);
        if (!PV.IsMine)
        {
            Destroy(camera);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            MousesMove();

        }
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            Move();

        }
    }

    void Move()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed * Time.deltaTime);
    }

    void MousesMove()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * sencitivity * Time.deltaTime, 0);
        camera.transform.Rotate(-Input.GetAxis("Mouse Y") * sencitivity * Time.deltaTime, 0, 0);
    }
}
