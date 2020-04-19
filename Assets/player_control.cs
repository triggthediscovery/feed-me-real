using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class player_control : MonoBehaviour
{
    Animator anim;
    public float speed = 0.1f;

    public GameObject carrying = null;

    public float throw_speed = 50;

    public bool attacking;

    public GameObject health_slider_obj;
    Slider health_slider;
    public GameObject stamina_slider_obj;
    Slider stamina_slider;

    public GameObject slider_obj;
    Slider slider;

    public float health = 100;
    public float stamina = 100;

    public float slap_damage = 10;

    public List<GameObject> inventory;
    public int inventory_sel = -1;

    public List<GameObject> thrown;

    float jump_prev;
    AudioSource source;

    public AudioClip oof;
    public AudioClip throw_audio;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        health_slider = health_slider_obj.GetComponent<Slider>();
        stamina_slider = stamina_slider_obj.GetComponent<Slider>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward", Input.GetAxis("Vertical"));

        stamina = stamina += Time.deltaTime * 6;

        if (carrying != null && inventory_sel == -1) {
            stamina -= Time.deltaTime * 10;
        }

        if (stamina < 0) {
            throw_obj(0, throw_speed * 0.8f);
        }

        if (stamina > 100) stamina = 100;

        health_slider.value = health / 100;
        stamina_slider.value = stamina / 100;

        if (health < 0) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        gameObject.GetComponent<Rigidbody>().MovePosition(gameObject.transform.position + gameObject.transform.forward * Input.GetAxis("Vertical") * speed);
        gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0));

        GameObject[] npcs = GameObject.FindGameObjectsWithTag("npc");

        // scroll inv
        if (jump_prev < 0.5f && Input.GetAxis("Jump") >= 0.5f) {
            inventory_sel++;

            if (inventory_sel == inventory.Count) {
                inventory_sel = -1;
            }

            if (inventory_sel != -1) {
                carrying = inventory[inventory_sel];
            } else {
                carrying = null;
            }
        }

        jump_prev = Input.GetAxis("Jump");

        for (int i=0; i<inventory.Count; i++) {
            inventory[i].transform.localPosition = new Vector3(0, 1.5f, 6.0f);
            inventory[i].transform.localRotation = Quaternion.Euler(-180, 0, 0);

            if (inventory_sel != i) {
                inventory[i].SetActive(false);
            } else {
                inventory[i].SetActive(true);
            }
        }

        foreach (GameObject obj in inventory) {
        }

        // try to pick up
        if (Input.GetAxis("Fire1") > 0.5f && carrying==null && stamina > 60) {
            GameObject[] throwable_obj = GameObject.FindGameObjectsWithTag("throwable_obj");

            GameObject[] all_throwable = new GameObject[npcs.Length + throwable_obj.Length];

            npcs.CopyTo(all_throwable, 0);
            throwable_obj.CopyTo(all_throwable, npcs.Length);

            GameObject closest = null;
            float closest_dist = 100000;

            for (int i = 0; i < all_throwable.Length; i++) {
                if (Vector3.Distance(gameObject.transform.position, all_throwable[i].transform.position) < closest_dist) {
                    closest_dist = Vector3.Distance(gameObject.transform.position, all_throwable[i].transform.position);
                    closest = all_throwable[i];
                }
            }

            if (closest_dist < 1 && closest != null) {
                carrying = closest;
                thrown.Add(carrying);

                stamina -= 60;
            } else {
                Debug.Log(closest_dist);
                Debug.Log("no throwables!");
            }
        }

        foreach (GameObject npc in npcs) {
            if (npc != carrying && npc.GetComponent<npc_behavior>() != null) {
                npc_behavior behave = npc.GetComponent<npc_behavior>();

                if (behave != null && behave.attack > 0.5f && Vector3.Distance(transform.position, npc.transform.position) < 1) {
                    health -= Time.deltaTime * slap_damage;
                    if (!source.isPlaying) {
                        source.clip = oof;
                        source.Play();
                    }
                }
            }
        }

        if (carrying != null && inventory_sel == -1) {
            carrying.transform.position = gameObject.transform.position + new Vector3(0,0.8f,0);
            carrying.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0,0,90);

            if (carrying.GetComponent<CapsuleCollider>() != null)
                carrying.GetComponent<CapsuleCollider>().enabled = false;

            gameObject.transform.Find("Armature").Find("Bone").Find("Bone_001").Find("Bone_003_l").localRotation = Quaternion.Euler(-4, 90, -90);
            gameObject.transform.Find("Armature").Find("Bone").Find("Bone_001").Find("Bone_003_r").localRotation = Quaternion.Euler(4, 90, 90);

            carrying.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        } else if (carrying != null) {
            gameObject.transform.Find("Armature").Find("Bone").Find("Bone_001").Find("Bone_003_l").localRotation = Quaternion.Euler(-4,180,-90);
            gameObject.transform.Find("Armature").Find("Bone").Find("Bone_001").Find("Bone_003_r").localRotation = Quaternion.Euler(4,180,90);
        } else {
            gameObject.transform.Find("Armature").Find("Bone").Find("Bone_001").Find("Bone_003_l").localRotation = Quaternion.Euler(-4, -90, -90);
            gameObject.transform.Find("Armature").Find("Bone").Find("Bone_001").Find("Bone_003_r").localRotation = Quaternion.Euler(4, -90, 90);
        }

        // try to throw
        if (Input.GetAxis("Fire2") > 0.5f && carrying) {

            Debug.Log(inventory_sel);

            if (inventory_sel != -1) {
                inventory.Remove(inventory[inventory_sel]);
                inventory_sel = -1;

                Debug.Log(inventory.Count);

                carrying.transform.parent = null;
            }

            if (carrying.tag == "npc" && carrying.GetComponent<npc_behavior>() != null) {
                source.clip = throw_audio;
                source.Play();
            }

            throw_obj(throw_speed, throw_speed * 0.8f);
        }
        
        anim.SetFloat("attack", Input.GetAxis("Fire3"));
        if (Input.GetAxis("Fire3") > 0.5f) {
            attacking = true;
        } else {
            attacking = false;
        }
    }

    void throw_obj(float p_x, float p_y) {
        carrying.transform.position = carrying.transform.position + new Vector3(0, 0.2f, 0);
        if (carrying.GetComponent<CapsuleCollider>() != null)
            carrying.GetComponent<CapsuleCollider>().enabled = true;
        carrying.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
        carrying.gameObject.GetComponent<Rigidbody>().AddForce((gameObject.transform.forward * p_x * 4) + (gameObject.transform.up * p_y * 4));
        carrying = null;

    }
}
