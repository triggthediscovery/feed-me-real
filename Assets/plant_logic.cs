using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class plant_logic : MonoBehaviour {
    Animator anim;
    GameObject target = null;
    public GameObject player;
    player_control player_script;
    public float alert_dist = 5;

    public float targ_scale = 0.1f;
    float scale = 0.1f;
    public float height = 2;

    public float ground_offset;

    float endgametimer_p = 0;
    float endgametimer = 0;

    public GameObject text_obj;
    public GameObject camera_target;
    AudioSource source;

    // Start is called before the first frame update
    void Start() {
        player_script = player.GetComponent<player_control>();
        anim = gameObject.GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (player_script.carrying != null) {
            target = player_script.carrying;
        }

        if (target != null) {
            float target_dist = Vector3.Distance(gameObject.transform.position, target.transform.position);
            float anti = (alert_dist - target_dist) / alert_dist;

            anti = Mathf.Clamp(anti, 0, 1);

            float angle = Mathf.Atan((gameObject.transform.position.z - target.transform.position.z) / (gameObject.transform.position.x - target.transform.position.x)) * 57.2977f;

            if ((gameObject.transform.position.x - target.transform.position.x) < 0) {
                angle += 180;
            }

            angle = angle - 270;
            angle += 30;
            if (angle < -180) angle += 360;
            angle = Mathf.Clamp(angle, -30, 30);

            anim.SetFloat("anti", anti);
            anim.SetFloat("angle", angle);

            if (target_dist < 1 && player_script.carrying != target) {
                anim.SetTrigger("mounch");

                source.Stop();
                source.Play();

                target.SetActive(false);
                target = null;

                alertNpcs(2, 4);

                anim.SetFloat("anti", 0);
                anim.SetFloat("angle", 0);

                targ_scale *= 1.2f;
            }
        }

        scale = ((scale * 15) + (targ_scale * 1)) / 16;

        gameObject.transform.position = new Vector3(gameObject.transform.position.x, (height * scale) + ground_offset, gameObject.transform.position.z);

        gameObject.transform.localScale = new Vector3(scale, scale, scale);

        if (scale > 0.4f) {

            endgametimer_p = endgametimer;
            endgametimer += Time.deltaTime;

            if (endgametimer_p == 0 && endgametimer != 0) {
                player.GetComponent<player_control>().enabled = false;
                Transform cam = player.transform.Find("Main Camera");
                cam.parent = null;

                cam.position = camera_target.transform.position;
                cam.rotation = camera_target.transform.rotation;
            }
            
            if (endgametimer_p < 3 && endgametimer > 3) {
                Debug.Log("eat_player!");

                source.Stop();
                source.Play();

                anim.SetTrigger("eat_player");
            }

            if (endgametimer_p < 3.8f && endgametimer > 3.8f) {
                Debug.Log("disable_player!");
                text_obj.SetActive(true);

                Destroy(player.transform.Find("Cube").gameObject);
                Destroy(player);
            }

            if (endgametimer > 10) {
                Application.Quit();
            }
        }

        if (Input.GetAxis("Cancel") > 0.5f) {
            Application.Quit();
        }
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