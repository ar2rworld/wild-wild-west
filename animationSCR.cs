using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class animationSCR : MonoBehaviour
{
    public GameObject gameManager;

    private Animation a;

    private float yaw = 0f;
    private float pitch = 0f;

    private Rigidbody rb;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    private Vector3 forward_;
    public float force = 10;
    public float runIndex = 20;
    public float jumpForce = 1000;
    public float maxDistanceDown = 1;

    //camera positionning
    public GameObject point1;
    public GameObject point2;
    public GameObject camera;

    public GameObject weaponPos;
    public GameObject weaponPos2;

    private float reloadTime;//camera reload
    private int cameraID = 1;//cameraID

    //weapons
    public int nOfWeapons = 2;
    private List<Weapon> weapons = new List<Weapon>();
    public GameObject weapon0;
    public GameObject weapon1;
    public GameObject weapon2;
    public int activeWeapon=0;
    private GameObject activeWeaponGO;
    public bool weaponSound;
    public float bullets9mm;
    public float bullets12mm = 100;

    //userUI
    public GameObject userUI;

    //Damage system
    public float health = 100;
    public float armor = 5;
    public Collider standingShape;

    //Animation vars
    bool inSeat = false;
    bool inSeatIdle = false;

    void Start()
    {
        a = GetComponent<Animation>();
        rb = GetComponent<Rigidbody>();
        /*List<Weapon> tws = new List<Weapon>();
        Weapon aa = new Weapon(0,"321", 1, 2, 3, 4);
        Weapon b = new Weapon(1,"123", 1, 22, 23, 4);
        tws.Add(aa);
        if (tws.Contains(b))
        {
            Debug.Log("true");
        }
        else
        {
            Debug.Log("false");
        }*/
    }
    void FixedUpdate()
    {
        cameraChange();
        rotate();
        if (Input.GetKeyDown(KeyCode.P))
        {
            printWeaponsList();
            
        }
        changeWeapon();
        shooting();
        rotateWeapon();
    }
    private void Update()
    {
        userUIFunction();
    }
    void userUIFunction()
    {
        Text[] textUI = userUI.GetComponentsInChildren<Text>();
        //Debug.Log("textUI len: " + textUI.Length);
        if (textUI.Length > 0)
        {
            textUI[0].text = "bullets12mm: " + bullets12mm + "\nbullets9mm: " + bullets9mm + "\nhealth: " + health + "\narmor:" + armor;
        }
    }
    void shooting()
    {
        if (weapons.Count > 0)
        {
            if (Input.GetMouseButton(0))
            {
                activeWeaponGO.GetComponent<weaponSCR>().shoot();
            }
        }
    }
    public void Damage(float d)
    {
        health -= d - d * armor/100;
        if(health <= 0)
        {
            gameObject.SetActive(false);
            Debug.Log("game over");
            gameManager.GetComponent<GameManager>().gameOver();
        }
    }
    void printWeaponsList()
    {
        for(int i = 0; i<weapons.Count; i++)
        {
            Debug.Log(weapons[i].toString());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "weapon")
        {
            pickItems(collision.gameObject);
            Debug.Log(weapons.Count);
        }
    }
    void pickItems(GameObject go)
    {
        weaponSCR g = go.GetComponent<weaponSCR>();
        Weapon current_Weapon = new Weapon(g.id, g.name, g.damage, g.reloadTime,g.fireRate, g.maxDistance);
        //Debug.Log("weapons.Contains(current_Weapon) " + weapons.Contains(current_Weapon));
        if (!hasWeapon(current_Weapon) && weapons.Count <= nOfWeapons)//weapons.Count <= nOfWeapons && !weapons.Contains(current_Weapon)
        {
            weapons.Add(new Weapon(g.id, g.name, g.damage, g.reloadTime, g.fireRate, g.maxDistance));
            weapon0.SetActive(false);
            weapon1.SetActive(false);
            weapon2.SetActive(false);
            switch (g.id){
                case 0:
                    weapon0.SetActive(true);
                    activeWeapon = 0;
                    activeWeaponGO = weapon0;
                    break;
                case 1:
                    weapon1.SetActive(true);
                    activeWeapon = 1;
                    activeWeaponGO = weapon1;
                    break;
                case 2:
                    weapon2.SetActive(true);
                    activeWeapon = 2;
                    activeWeaponGO = weapon2;
                    break;
            }
            //Instantiate(go);
            Destroy(go);
        }
    }
    bool hasWeapon(Weapon w)
    {
        bool _out = false;
        for(int i = 0; i<weapons.Count; i++)
        {
            if(weapons[i].id == w.id)
            {
                _out = true;
                break;
            }
        }
        return _out;
    }
    void rotateWeapon()
    {
        //Debug.Log("x,y,z = " + Input.mousePosition.x + " , " + Input.mousePosition.y + " , " + Input.mousePosition.z);
        if (activeWeaponGO != null)
        {
            activeWeaponGO.transform.rotation = camera.transform.rotation;
        }
    }
    void changeWeapon()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            if (weapons.Count > 0)
            {
                if (Input.mouseScrollDelta.y > 0)
                {
                    activeWeapon += 1;
                    if (activeWeapon >= weapons.Count)
                        activeWeapon = weapons.Count;
                }
                else
                {
                    activeWeapon -= 1;
                    if (activeWeapon < 0) activeWeapon = 0;
                }
                switch (activeWeapon)
                {
                    case 0:
                            weapon0.SetActive(false);
                            weapon1.SetActive(false);
                        weapon2.SetActive(false);
                        break;
                    case 1:
                        if (hasWeapon(new Weapon(0))){
                            weapon1.SetActive(false);
                            weapon2.SetActive(false);
                            weapon0.SetActive(true);
                            activeWeaponGO = weapon0;
                        }
                        break;

                    case 2:
                        if (hasWeapon(new Weapon(1)))
                        {
                            weapon0.SetActive(false);
                            weapon2.SetActive(false);
                            weapon1.SetActive(true);
                            activeWeaponGO = weapon1;
                        }
                        break;
                    case 3:
                        if(hasWeapon(new Weapon(2)))
                        {
                            weapon0.SetActive(false);
                            weapon1.SetActive(false);
                            weapon2.SetActive(true);
                            activeWeaponGO = weapon2;
                        }
                        break;
                }
            }
            Debug.Log(activeWeapon);
            for (int i=0; i<weapons.Count; i++)
            {
                Debug.Log(weapons[i].toString());
            }
        }
        
    }
    void cameraChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && reloadTime <= Time.time)
        {
            reloadTime = Time.time + 1f;
            if (camera.transform.position == point1.transform.position)
            {
                cameraID = 2;
                camera.transform.position = point2.transform.position;
            }
            else
            {
                cameraID = 1;
                camera.transform.position = point1.transform.position;
            }
        }
        if(cameraID == 2)
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -35, 75);
            camera.transform.eulerAngles = new Vector3(pitch, yaw, 0);
        }
    }
    void rotate()
    {
        //rotate
        forward_ = transform.forward;
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -35, 75);
        camera.transform.eulerAngles = new Vector3(pitch, yaw, 0);
        transform.eulerAngles = new Vector3(0, yaw, 0);
        bool idle = true;
        if (!inSeat)
        {
            if (Input.GetKey(KeyCode.Q) && idle)
            {
                idle = false;
                a.Play("hi");
            }
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
            {
                idle = false;
                a.Play("run");
                transform.position += forward_ * runIndex;
            }
            if (Input.GetKey(KeyCode.W) && idle)
            {
                idle = false;
                a.Play("walk");
                transform.position += forward_ * force;
            }
            if (Input.GetKey(KeyCode.S))
            {
                idle = false;
                a.Play("walk");
                transform.position -= forward_ * force;
            }
            if (Input.GetKey(KeyCode.D))
            {
                idle = false;
                a.Play("walk");
                transform.position += transform.right * force;
            }
            if (Input.GetKey(KeyCode.A))
            {
                idle = false;
                a.Play("walk");
                transform.position -= transform.right * force;
            }
            if (Input.GetKeyDown(KeyCode.Space) && onGround())
            {
                idle = false;
                a.Play("jump");
                rb.AddForce(transform.up * jumpForce);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && false)
        {
            idle = false;
            inSeat = !inSeat;
            standingShape.enabled = !standingShape.enabled;
        }
        if (inSeat)
        {
            if (activeWeaponGO != null)
            {
                activeWeaponGO.transform.position = weaponPos2.transform.position;
            }
            inSeatIdle = true;
            if (Input.GetKey(KeyCode.W))
            {
                inSeatIdle = false;
                a.Play("walkSeat");
                transform.position += forward_ * force/2;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inSeatIdle = false;
                a.Play("walkSeat");
                transform.position -= forward_ * force/2;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inSeatIdle = false;
                a.Play("walkSeat");
                transform.position += transform.right * force/2;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inSeatIdle = false;
                a.Play("walkSeat");
                transform.position -= transform.right * force/2;
            }
            if (inSeatIdle)
            {
                a.Play("seatIdle");
            }
        }
        else if(activeWeaponGO != null)
        {
            activeWeaponGO.transform.position = weaponPos.transform.position;
        }
        if (!inSeat && Input.GetKeyDown(KeyCode.LeftControl))
        {
            a.Play("idleToSeat");
        }
        if (idle && !inSeat)
        {
            a.Play("idle");
        }
    }
    bool onGround()
    {
        bool out_ = false;
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.up * -1, Color.white, 0.04f);
        if (Physics.Raycast(transform.position, transform.up * -1, out hit, 0.04f))
        {
                Debug.Log(Vector3.Distance(hit.point, transform.position));
                out_ = true;
        }
        return out_;
    }
}
public class Weapon
{
    public int id;
    public string name;
    public float damage;
    public float reloadTime;
    public float fireRate;
    public float maxDistance;
    public bool found = false;
    public Weapon(int i, string n, float d, float rT, float rOfF, float mD)
    {
        id = i;
        name = n;
        damage = d;
        reloadTime = rT;
        fireRate = rOfF;
        maxDistance = mD;
    }
    public Weapon(int i)
    {
        id = i; name = "default"; damage = 1;reloadTime = 0; fireRate = 0; maxDistance = 0;
    }
    public bool Equals(Weapon w)
    {
        return w.id == id ? true : false;
    }
    public string toString()
    {
        return "id: " + id + " name: " + name + " damage: " + damage + " reloadTime: " + reloadTime + " fireRate: " + fireRate + " maxDist: " + maxDistance;
    }
}