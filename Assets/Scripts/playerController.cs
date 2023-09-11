using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 input;
    private bool isXAxisInUse = false;
    private bool isYAxisInUse = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        if (input.x != 0) { input.y = 0; } //remove diagonal movement
        if (input.y != 0) { input.x = 0; }
        if (input.x != 0)
        {
            if (isXAxisInUse == false)
            {
                transform.position = transform.position + new Vector3(input.x, input.y, 0);
                isXAxisInUse = true;
            }
        }
        if (input.x == 0)
        {
            isXAxisInUse = false;
        }
        if (input.y != 0)
        {
            if (isYAxisInUse == false)
            {
                transform.position = transform.position + new Vector3(input.x, input.y, 0);
                isYAxisInUse = true;
            }
        }
        if (input.y == 0)
        {
            isYAxisInUse = false;
        }
    }
}
