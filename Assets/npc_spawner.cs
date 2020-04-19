using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_spawner : MonoBehaviour
{
    public GameObject npc;
    public float rate;
    public GameObject player;
    float timer;

    public GameObject[] hats;
    public GameObject dead_variant;

    public Material[] shirts;

    static int total_npc = 0;

    // Start is called before the first frame update
    void Start()
    {
        timer = rate; 
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0 && total_npc < 1000 && Vector3.Distance(player.transform.position, transform.position) > 20) {
            timer = rate * Random.Range(0.9f, 1.1f);

            total_npc++;

            GameObject new_npc = Instantiate(npc);

            new_npc.transform.position = transform.position;

            npc_behavior behave = new_npc.GetComponent<npc_behavior>();

            behave.delta_f = new float[] {Random.Range(0.0f,1) };
            behave.offsets_f = new float[] { Random.Range(0.0f, 3) };
            behave.speeds_f = new float[] { Random.Range(0.01f, 0.05f) };
            behave.delta_r = new float[] { Random.Range(0.0f, 1) };
            behave.offsets_r = new float[] { Random.Range(0.0f, 3) };
            behave.speeds_r = new float[] { Random.Range(0.0f, 1) };

            behave.player = player;
            behave.dead_variant = dead_variant;

            int hat_num = hats.Length;
            int hat_sel = Random.Range(0, hat_num + 1);

            if (hat_sel != hat_num) {
                GameObject hat = Instantiate(hats[hat_sel], new_npc.transform.Find("Armature").Find("Bone").Find("Bone_001").Find("Bone_002"));
                hat.transform.localPosition = new Vector3(-1.2f,0,0);
                hat.transform.localRotation = Quaternion.Euler(0,0,90);
            }

            SkinnedMeshRenderer mesh = new_npc.transform.Find("Cube").gameObject.GetComponent<SkinnedMeshRenderer>();

            mesh.materials = new Material[] { mesh.materials[0], shirts[Random.Range(0, shirts.Length)], mesh.materials[1]};
            
        }
    }
}
