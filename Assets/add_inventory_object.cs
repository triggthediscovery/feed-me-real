using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class add_inventory_object : MonoBehaviour {
    public GameObject player;
    public GameObject inv_obj;
    player_control p_con;

    // Start is called before the first frame update
    void Start()
    {
        p_con = player.GetComponent<player_control>();
    }

    // Update is called once per frame
    void Update() {
        float player_dist = Vector3.Distance(gameObject.transform.position, player.transform.position);

        if (player_dist < 1) {
            if (inv_obj.tag == "coffee") {
                p_con.stamina = 100;
            }
            else if (inv_obj.tag == "medkit") {
                p_con.health = 100;
            }
            else {
                GameObject new_obj = Instantiate(inv_obj, player.transform);
                new_obj.transform.localPosition = new Vector3(0, 1.5f, 6.0f);
                new_obj.transform.localRotation = Quaternion.Euler(-180, 0, 0);

                p_con.inventory.Add(new_obj);
            }

            Destroy(gameObject);
        }
    }
}
