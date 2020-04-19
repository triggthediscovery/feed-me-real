using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_behavior : MonoBehaviour {
    Animator anim;

    public float[] delta_f;
    public float[] offsets_f;
    public float[] speeds_f;
    public float[] delta_r;
    public float[] offsets_r;
    public float[] speeds_r;

    public GameObject player;
    player_control player_script;
    public float close_dist = 1;
    public float attack;
    float attack_targ;

    public bool stunned = false;
    float stun_time = 0;
    float stun_resist = 2;

    public int alert = 0;

    public GameObject low_alert;
    public GameObject high_alert;

    public float health = 100;

    GameObject money = null;

    public GameObject dead_variant;
    AudioSource source;

    // Start is called before the first frame update
    void Start() {
        player_script = player.GetComponent<player_control>();
        anim = gameObject.GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        source.pitch = Random.Range(0.5f,2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        float forward = 0;
        float rotate = 0;

        float player_dist = Vector3.Distance(gameObject.transform.position, player.transform.position) / 100;

        if (player_script.carrying != null && player_script.carrying.tag == "npc" && player_dist < 0.1f) {
            alert = 1;
        }
        
        if (health < 0) {
            alertNpcs(2, 8);

            GameObject corpse = Instantiate(dead_variant);
            corpse.transform.position = transform.position;
            corpse.transform.rotation = transform.rotation;

            Destroy(gameObject);
        }

        if (alert == 2) {
            if (Random.Range(0.0f, 1.0f) < 0.01f) {
                alertNpcs(1,8);
            }
        }

        // check for money
        if (Random.Range(0.0f, 1.0f) < 0.1f) {
            if (money == null) { 
                GameObject[] monies = GameObject.FindGameObjectsWithTag("money");
                GameObject[] roses = GameObject.FindGameObjectsWithTag("rose");

                GameObject[] attract_objs = new GameObject[monies.Length + roses.Length];


                monies.CopyTo(attract_objs, 0);
                roses.CopyTo(attract_objs, monies.Length);

                GameObject closest = null;
                float closest_dist = 100000;

                foreach (GameObject i_money in attract_objs) {
                    if (Vector3.Distance(gameObject.transform.position, i_money.transform.position) < closest_dist && (i_money.transform.parent == null || i_money.tag == "rose")) {
                        closest_dist = Vector3.Distance(gameObject.transform.position, i_money.transform.position);
                        closest = i_money;
                    }

                    if (Vector3.Distance(gameObject.transform.position, i_money.transform.position) < closest_dist*3 && i_money.tag == "rose") {
                        closest_dist = Vector3.Distance(gameObject.transform.position, i_money.transform.position);
                        closest = i_money;
                    }
                }

                if (closest_dist < 5 && closest != null) {
                    money = closest;
                }
            } else if (money.activeInHierarchy == false) {
                money = null;
            }
        }

        if (alert == 0) {
            if (money == null) {
                for (int i = 0; i < speeds_f.Length; i++) {
                    forward += (Mathf.Sin((Time.time - offsets_f[i]) * delta_f[i]) * speeds_f[i]) + (speeds_f[i] / 1.5f);
                }

                for (int i = 0; i < speeds_r.Length; i++) {
                    rotate += Mathf.Sin((Time.time - offsets_r[i]) * delta_r[i]) * speeds_r[i];
                }

                forward = Mathf.Clamp(forward, -0.02f, 0.02f);
                rotate = Mathf.Clamp(rotate, -1.0f, 1.0f);
            } else {
                float money_dist = Vector3.Distance(gameObject.transform.position, money.transform.position);
                float angle = Mathf.Atan((gameObject.transform.position.z - money.transform.position.z) / (gameObject.transform.position.x - money.transform.position.x)) * 57.2977f;

                if ((gameObject.transform.position.x - money.transform.position.x) < 0) {
                    angle += 180;
                }

                angle += transform.eulerAngles.y;
                angle += 90;

                while (angle > 180) angle -= 360;
                while (angle < -180) angle += 360;

                angle = angle / 20;

                if (float.IsNaN(angle)) angle = 0;

                rotate = Mathf.Clamp(-angle, -1, 1);
                forward = Mathf.Clamp(money_dist/100, 0, 0.03f);
                
                if (money_dist<1 && money.tag == "money") {
                    anim.SetTrigger("crouch");
                    money.SetActive(false);
                }
            }

            attack_targ = 0;
        } else {
            float angle = Mathf.Atan((gameObject.transform.position.z - player.transform.position.z) / (gameObject.transform.position.x - player.transform.position.x)) * 57.2977f;

            if ((gameObject.transform.position.x - player.transform.position.x) < 0) {
                angle += 180;
            }

            angle += transform.eulerAngles.y;
            angle += 90;

            while (angle > 180) angle -= 360;
            while (angle < -180) angle += 360;

            angle = angle / 20;

            if (float.IsNaN(angle)) angle = 0;

            rotate = Mathf.Clamp(-angle, -1, 1);
            forward = Mathf.Clamp(player_dist, 0, 0.03f);

            attack_targ = 1;
        }

        if (player_dist < 0.01 && (player_script.attacking || (player_script.inventory_sel != -1 && player_script.inventory[player_script.inventory_sel].tag == "sword"))) {
            if (Random.Range(0.0f, 1.0f) < 0.1f) {
                alertNpcs(1,8);
            }

            if (player_script.attacking) {
                stun_resist -= Time.deltaTime;
                health -= Time.deltaTime * 20;
            } else {
                health -= Time.deltaTime * 30;
            }
        }

        if (stun_resist < 0) {
            stun();
            stun_resist = 2;
        }

        if (stunned) {
            stun_time -= Time.deltaTime;

            if (stun_time < 0) {
                anim.SetTrigger("exit_stun");
                stunned = false;
            }

            transform.Find("stars").gameObject.SetActive(true);
        }

        if (attack > 0.5f) {
            if (!source.isPlaying) {
                source.Play();
                }
        }
        else {
            source.Stop();
        }

        if (!stunned) {
            transform.Find("stars").gameObject.SetActive(false);

            attack = Mathf.Lerp(attack, attack_targ, 0.03f);
            anim.SetFloat("attack", attack);


            anim.SetFloat("forward", forward * 50);
            gameObject.GetComponent<Rigidbody>().MovePosition(gameObject.transform.position + gameObject.transform.forward * forward);
            gameObject.transform.Rotate(new Vector3(0, rotate, 0));
        }

        if (alert != 1 && low_alert != null) {
            low_alert.SetActive(false);
        } else {
            low_alert.SetActive(true);
        }

        if (alert != 2 && high_alert != null) {
            high_alert.SetActive(false);
        }
        else {
            high_alert.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.relativeVelocity.magnitude > 2 && (collision.gameObject.tag == "npc" || collision.gameObject.tag == "throwable_obj") &&
            (player_script.thrown.Contains(gameObject) || player_script.thrown.Contains(collision.gameObject))) {
            Debug.Log("collision");
            stun();
            health -= 50;

            alertNpcs(1,8);
        }
        else {
            //Debug.Log(collision.relativeVelocity.magnitude);
        }
    }

    void stun() {
        anim.SetTrigger("enter_stun");
        stunned = true;
        stun_time = 10;
    }

    void alertNpcs(int level, float range) {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("npc");

        foreach (GameObject npc in npcs) {
            float dist = Vector3.Distance(gameObject.transform.position, npc.transform.position);

            if (dist < range && npc.GetComponent<npc_behavior>() != null) {
                if (npc.GetComponent<npc_behavior>().alert < level) npc.GetComponent<npc_behavior>().alert = level;
            }
        }
    }
}
