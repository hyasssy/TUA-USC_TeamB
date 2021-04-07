using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cinemachine;
using System.Linq;

public class DogPlayerCamController : MonoBehaviour
{
    [SerializeField]
    float staggerSpeed = 0.3f, staggerIntensity = 0.02f;
    float _noiseParam = 0f;
    [SerializeField]
    Vector2 rotateRangeX = new Vector2(-5f, 5f);
    [SerializeField]
    Vector2 rotateRangeY = new Vector2(-5f, 5f);
    [field: SerializeField, RenameField(nameof(VirtualCamera_default))]
    public Transform VirtualCamera_default { get; private set; } = default;
    Vector3 defaultCamAngle;
    public Transform cameraTarget { get; private set; } = default;
    CancellationTokenSource _cts;
    public async UniTask ChangeCamera(Transform targetVirtualCamera)
    {
        if (_cts == null)
        {
            _cts = new CancellationTokenSource();
        }
        else
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }
        targetVirtualCamera.gameObject.SetActive(true);
        cameraTarget = targetVirtualCamera;
        await WaitBlend();
    }
    /// <summary>If set no argument, it sets default virtual camera.</summary>
    public async UniTask ChangeCamera()
    {
        if (_cts == null)
        {
            _cts = new CancellationTokenSource();
        }
        else
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }

        cameraTarget.gameObject.SetActive(false);
        cameraTarget = VirtualCamera_default;
        cameraTarget.gameObject.SetActive(true);
        await WaitBlend();
    }
    public void SetDefaultCamera()
    {
        FindObjectsOfType<CinemachineVirtualCamera>().ToList().ForEach(v =>
        {
            v.gameObject.SetActive(false);
        });
        VirtualCamera_default.gameObject.SetActive(true);
        cameraTarget = VirtualCamera_default;
    }
    async UniTask WaitBlend()
    {
        var brain = FindObjectOfType<CinemachineBrain>();
        while (true)
        {
            //blendを確認する為に、先に1フレーム挟む必要がある。
            await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);
            if (brain.ActiveBlend == null) break;
        }
        Debug.LogWarning("CameraBlend終了");
    }
    private void OnDisable()
    {
        if (_cts != null)
        {
            _cts.Cancel();
        }
    }

    private void Start()
    {
        defaultCamAngle = VirtualCamera_default.localEulerAngles;
        //シーン上のvirtualcameraの表示をリセットする。
        SetDefaultCamera();
        this.UpdateAsObservable()
        .Subscribe(_ =>
        {
            Stagger();
            RestrictRotate();
        }).AddTo(this);
    }
    void Stagger()
    {
        var noiseX = Mathf.PerlinNoise(_noiseParam, 0) - 0.5f;
        var noiseY = Mathf.PerlinNoise(0, _noiseParam) - 0.5f;
        _noiseParam += staggerSpeed * Time.deltaTime;
        if (_noiseParam >= 256) _noiseParam = 0;
        cameraTarget.localEulerAngles += new Vector3(noiseX * staggerIntensity, noiseY * staggerIntensity, 0);
    }
    void RestrictRotate()
    {
        var current = cameraTarget.localEulerAngles;
        var x = current.x > 180 ? current.x - 360 : current.x;
        var y = current.y > 180 ? current.y - 360 : current.y;
        current = new Vector3(x, y, 0);
        cameraTarget.localEulerAngles = defaultCamAngle + new Vector3(Mathf.Clamp(current.x, rotateRangeX.x, rotateRangeX.y), Mathf.Clamp(current.y, rotateRangeY.x, rotateRangeY.y), 0);
    }
}
