using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope2D : MonoBehaviour
{

    public Rigidbody2D start;
    public Rigidbody2D end;

    public int partCount;

    // Start is called before the first frame update
    private void Start()
    {
        var partVector = (end.position - start.position) / partCount;
        for (var i = 0; i < partCount; i++)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
