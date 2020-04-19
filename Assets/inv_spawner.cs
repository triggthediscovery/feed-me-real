using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inv_spawner : MonoBehaviour {
    public GameObject[] objs;
    public float spawn_chance = 0.5f;
    public GameObject player;

    public bool unspawned = true;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (unspawned) {
            if (Random.Range(0.0f, 1.0f) <= spawn_chance) {
                int inv_num = objs.Length;
                int inv_sel = Random.Range(0, inv_num);

                GameObject new_obj = Instantiate(objs[inv_sel]);
                new_obj.transform.position = transform.position;
                new_obj.transform.rotation = transform.rotation;
                new_obj.transform.localScale = transform.localScale;
                new_obj.tag = "Untagged";

                add_inventory_object new_comp = new_obj.AddComponent<add_inventory_object>();
                new_comp.inv_obj = objs[inv_sel];
                new_comp.player = player;
            }

            unspawned = false;
        }
    }
}
