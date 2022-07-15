using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    private Material _material;
    private IDisposable _disposable;
    public void Initialize(Circle circle)
    {
        if (_material == null)
        {
            foreach (var mat in GetComponentInChildren<Renderer>().materials)
            {
                if (mat.name.Contains("Cust"))
                {
                    _material = mat;
                    break;
                }
            }
        }
        var cc = circle.OwnerPlayer.mainColor;
        _material.color = new Color(cc.r - 0.1f, cc.g - 0.1f, cc.b - 0.1f);
        transform.position = circle.transform.position;
    }
    public void Dispose()
    {
        if(_disposable == null)
        {
            _disposable = GetComponent<IDisposable>();
        }
        if(gameObject.activeSelf)
            _disposable.Dispose();
    }
}
