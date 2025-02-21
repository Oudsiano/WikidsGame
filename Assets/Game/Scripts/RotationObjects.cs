using UnityEngine;
using DG.Tweening;

public class RotationObjects : MonoBehaviour // TODO rename
{
    public Vector3 rotationAxis = Vector3.up; 
    public float rotationDuration = 1f; 
    public Ease easeType = Ease.Linear; 

    private void Start() // TODO Construct 
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.DOLocalRotate(transform.localRotation.eulerAngles + rotationAxis * 360f, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Restart);
    }
}


