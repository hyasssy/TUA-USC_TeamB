using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cinemachine;
using System.Linq;

public class PlayerCamController : MonoBehaviour
{
    //少しpositionも動くようにするともっとリッチになるか。
    [SerializeField]
    float rotateSpeed = 1.7f;
    [SerializeField]
    float staggerSpeed = 0.3f, staggerIntensity = 0.02f;
    float _noiseParam = 0f;
    [SerializeField]
    Vector2 rotateRangeX = new Vector2(-20f,20f);
    [SerializeField]
    Vector2 rotateRangeY = new Vector2(-20f,20f);
    [field:SerializeField, RenameField(nameof(VirtualCamera_default))]
    public Transform VirtualCamera_default { get; private set; } = default;
    Vector3 defaultCamAngle;
    public Transform cameraTarget { get; private set; } = default;
    bool _cameraFixed = false;
    CancellationTokenSource _cts;
    public void ChangeCamera(Transform targetVirtualCamera){
        if(_cts == null){
            _cts = new CancellationTokenSource();
        }else{
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }
        _cameraFixed = true;
        targetVirtualCamera.gameObject.SetActive(true);
        cameraTarget = targetVirtualCamera;
    }
    /// <summary>If set no argument, it sets default virtual camera.</summary>
    public void ChangeCamera(){
        ChangeCameraTask().Forget();
    }
    async UniTask ChangeCameraTask(){
        cameraTarget.gameObject.SetActive(false);
        cameraTarget = VirtualCamera_default;
        await UniTask.Delay((int)(FindObjectOfType<CinemachineBrain>().m_DefaultBlend.BlendTime * 1000), cancellationToken: _cts.Token);
        _cameraFixed = false;
    }
    private void OnDisable() {
        if(_cts != null){
            _cts.Cancel();
        }
    }

    private void Start() {
        _cameraFixed = false;
        defaultCamAngle = VirtualCamera_default.localEulerAngles;
        //シーン上のvirtualcameraの表示をリセットする。
        FindObjectsOfType<CinemachineVirtualCamera>().ToList().ForEach(f => f.gameObject.SetActive(false));
        VirtualCamera_default.gameObject.SetActive(true);
        cameraTarget = VirtualCamera_default;
        this.UpdateAsObservable()
        .Subscribe(_=>
        {
            Stagger();
            RotateView();
            RestrictRotate();
        }).AddTo(this);
    }
    void RotateView(){
        if(_cameraFixed) return;
        if (!Input.GetMouseButton(0)) return;
        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");
        if(x != 0 || y != 0){
            cameraTarget.localEulerAngles += new Vector3(y * rotateSpeed, -x * rotateSpeed, 0);
        }
    }
    void Stagger(){
        var noiseX = Mathf.PerlinNoise(_noiseParam, 0) - 0.5f;
        var noiseY = Mathf.PerlinNoise(0, _noiseParam) - 0.5f;
        _noiseParam += staggerSpeed * Time.deltaTime;
        if(_noiseParam >= 256)_noiseParam = 0;
        cameraTarget.localEulerAngles += new Vector3(noiseX * staggerIntensity, noiseY * staggerIntensity, 0);
    }
    void RestrictRotate(){
        if(_cameraFixed) return;
        var current = cameraTarget.localEulerAngles;
        var x = current.x > 180 ? current.x - 360 : current.x;
        var y = current.y > 180 ? current.y - 360 : current.y;
        current = new Vector3(x, y, 0);
        cameraTarget.localEulerAngles = defaultCamAngle + new Vector3(Mathf.Clamp(current.x,rotateRangeX.x,rotateRangeX.y), Mathf.Clamp(current.y,rotateRangeY.x,rotateRangeY.y), 0);
    }
}
