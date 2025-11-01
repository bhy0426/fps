using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject[] bullet; // 0 : 권총, 1 : 라이플
    public Transform[] bulletPos; // 0 : 권총, 1 : 라이플
    public Weapon equipWeapon; // 현재 사용 중인 무기 정보(type, damage, ..)

    public float sens;
    public float speed;
    public float[] attackRate;
    public float bulletSpeed;

    bool mouseLDown; // 마우스 왼쪽 클릭
    bool isFire;

    float mouseX = 0;
    float mouseY = 0;


    Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CameraRotation();
        Attack();


        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 100f, Color.green);
        // Debug.DrawRay(transform.position, transform.forward * 20f, Color.red);
    }

    private void FixedUpdate()
    {
        Move();
    }

    void CameraRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;

        mouseX += Input.GetAxis("Mouse X") * sens;
        mouseY -= Input.GetAxis("Mouse Y") * sens;

        mouseY = Mathf.Clamp(mouseY, -90, 90);

        transform.rotation  = Quaternion.Euler(0, mouseX, 0);
        mainCamera.transform.rotation  = Quaternion.Euler(mouseY, mouseX, 0);
        //transform.localRotation = Quaternion.Euler(0, mouseX, 0);
    }

    void Move()
    {
        float moveX;
        float moveZ;

        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveVec = (transform.forward * moveZ + transform.right * moveX).normalized;

        // rigid.velocity += moveVec * 5f * Time.deltaTime;
        // transform.position += moveVec.normalized * 5f *  Time.deltaTime;

        if (rigid.velocity.magnitude > 10f)
            rigid.velocity = rigid.velocity.normalized * 10f;
        else
            rigid.AddForce(moveVec * speed * Time.deltaTime, ForceMode.Impulse);     
    }

    void Attack()
    {
        mouseLDown = Input.GetButton("Fire1");

        if(mouseLDown && !isFire)
        {
            StopCoroutine(Fire());
            StartCoroutine(Fire());
        }
    }

    IEnumerator Fire()
    {
        RaycastHit rayHit;
        Vector3 dirVec;

        GameObject instanceBullet = Instantiate(bullet[((int)equipWeapon.type)], bulletPos[((int)equipWeapon.type)].position, bulletPos[((int)equipWeapon.type)].rotation);
        Rigidbody rigidBullet = instanceBullet.GetComponent<Rigidbody>();

        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out rayHit, Mathf.Infinity))
        {
            if(rayHit.collider != null)
            {
                Debug.Log(rayHit.point);
                dirVec = rayHit.point - bulletPos[((int)equipWeapon.type)].transform.position;
                rigidBullet.AddForce(dirVec.normalized * bulletSpeed, ForceMode.Impulse);
            }
        }

        isFire = true;
        yield return new WaitForSeconds(attackRate[((int)equipWeapon.type)]);
        isFire = false;
    }
}
