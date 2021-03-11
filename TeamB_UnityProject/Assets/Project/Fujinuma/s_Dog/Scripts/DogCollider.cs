using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogCollider : MonoBehaviour
{
    bool isStroke = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void StrokeOn()
    {
        isStroke = true;
    }
    void StrokeOff()
    {
        isStroke = false;
    }
}