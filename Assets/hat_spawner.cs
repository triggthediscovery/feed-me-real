using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hat_spawner : MonoBehaviour {
    
    public GameObject[] hats;
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
                int hat_num = hats.Length;
                int hat_sel = Random.Range(0, hat_num);

                GameObject new_hat = Instantiate(hats[hat_sel]);
                new_hat.transform.position = transform.position;
                new_hat.transform.rotation = transform.rotation;
                new_hat.transform.localScale = transform.localScale;

                apply_hat new_comp = new_hat.AddComponent<apply_hat>();
                new_comp.hat = hats[hat_sel];
                new_comp.player = player;
            }

            unspawned = false;
        }
    }
}
