using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittingPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            Debug.Log("Player hit!");
            // Thêm mã ?? gi?m HP c?a Player ho?c th?c hi?n hành ??ng khác
            player.GetComponent<IDamageable>()?.TakeDamage(10f * Time.deltaTime);
        }
    }
}
