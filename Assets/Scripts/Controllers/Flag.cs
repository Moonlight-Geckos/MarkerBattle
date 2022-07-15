using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private Material _material;
    private HashSet<Circle> _circles;
    public void Initialize(Circle circle)
    {
        if( _material == null)
        {
            foreach (var mat in GetComponentInChildren<Renderer>().materials)
            {
                if (mat.name.Contains("Cust")) {
                    _material = mat;
                    break;
                }
            }

            _circles = new HashSet<Circle>();
        }
        _circles.Add(circle);

        var cc = circle.OwnerPlayer.mainColor;
        _material.color = new Color(cc.r - 0.1f, cc.g - 0.1f, cc.b - 0.1f);

        SettlePosition();
    }
    private void SettlePosition()
    {
        Vector3 pos = Vector3.zero;
        foreach(var c in _circles)
        {
            pos += c.transform.position;
        }
        pos /= _circles.Count;

        //Calculate closest circle
        float minDis = float.MaxValue;
        Vector3 desiredPos = Vector3.zero;
        foreach(var c in _circles)
        {
            float d = Vector3.Distance(c.transform.position, pos);
            if(d < minDis)
            {
                minDis = d;
                desiredPos = c.transform.position + (pos - c.transform.position).normalized * c.Radius / 2f;
            }
        }
        transform.position = desiredPos;
    }
}
