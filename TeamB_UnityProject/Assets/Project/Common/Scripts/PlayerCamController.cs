using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerCamController : MonoBehaviour
{
    [SerializeField]
    float rotateSpeed = 1.7f;
    Transform _camRoot;
    [SerializeField]
    float staggerSpeed = 0.3f, staggerIntensity = 0.02f;
    float _noiseParam = 0f;
    [SerializeField]
    Vector2 rotateRangeX = new Vector2(-20f,20f);
    [SerializeField]
    Vector2 rotateRangeY = new Vector2(-20f,20f);
    private void Start() {
        _camRoot = transform;
        this.UpdateAsObservable()
        .Subscribe(_=>
        {
            Stagger(_camRoot);
            RotateView(_camRoot);
            RestrictRotate(_camRoot);
        });
    }
    void RotateView(Transform target){
            if (!Input.GetMouseButton(0)) return;
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");
            if(x != 0 || y != 0){
                target.localEulerAngles += new Vector3(y * rotateSpeed, -x * rotateSpeed, 0);
            }
    }
    void Stagger(Transform target){
        var noiseX = Mathf.PerlinNoise(_noiseParam, 0) - 0.5f;
        var noiseY = Mathf.PerlinNoise(0, _noiseParam) - 0.5f;
        _noiseParam += staggerSpeed * Time.deltaTime;
        if(_noiseParam >= 256)_noiseParam = 0;
        target.localEulerAngles += new Vector3(noiseX * staggerIntensity, noiseY * staggerIntensity, 0);
    }
    void RestrictRotate(Transform target){
        var current = target.localEulerAngles;
        var x = current.x > 180 ? current.x - 360 : current.x;
        var y = current.y > 180 ? current.y - 360 : current.y;
        current = new Vector3(x, y, 0);
        target.localEulerAngles = new Vector3(Mathf.Clamp(current.x,rotateRangeX.x,rotateRangeX.y), Mathf.Clamp(current.y,rotateRangeY.x,rotateRangeY.y), 0);
    }
}
