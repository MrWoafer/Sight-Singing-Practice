using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Tie : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SpriteShapeController spriteShape;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPoints(Vector3 startPoint, Vector3 endPoint)
    {
        Spline spline = spriteShape.spline;

        spline.SetPosition(0, transform.InverseTransformPoint(startPoint));
        spline.SetPosition(1, transform.InverseTransformPoint(endPoint));

        spriteShape.RefreshSpriteShape();
    }
}
