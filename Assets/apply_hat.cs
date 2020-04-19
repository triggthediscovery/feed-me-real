using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class apply_hat : MonoBehaviour
{
    public GameObject player;
    public GameObject hat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float player_dist = Vector3.Distance(gameObject.transform.position, player.transform.position);

        if (player_dist < 1) {
            GameObject new_hat = Instantiate(hat, player.transform.Find("Armature").Find("Bone").Find("Bone_001").Find("Bone_002"));
            new_hat.transform.localPosition = new Vector3(-1.2f, 0, 0);
            new_hat.transform.localRotation = Quaternion.Euler(0, 0, 90);

            foolNpcs(8);

            Destroy(gameObject);
        }
    }

    void foolNpcs(float range) {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("npc");

        foreach (GameObject npc in npcs) {
            float dist = Vector3.Distance(gameObject.transform.position, npc.transform.position);

            if (dist > range) {
                npc.GetComponent<npc_behavior>().alert = 0;
            }
        }
    }
}
