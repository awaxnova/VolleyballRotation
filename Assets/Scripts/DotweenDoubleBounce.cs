using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// This does a double bounce animation using DOTween, on the Y axis, on a UI item on a canvas, not in world space.
/// For world space, use DoMoveY instead of DOLocalMoveY.
/// </summary>
public class DotweenDoubleBounce : MonoBehaviour
{
    [SerializeField] private float bounceHeight = 4.0f;
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private float delayBetweenBounces = 1.3f; // Adjust this delay as needed

    [SerializeField] private bool performBounce = false;
    [SerializeField] private BounceDirection bounceDirection = BounceDirection.Y;
    public enum BounceDirection
    {
        Y,
        X,
        Z,
       
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        // Call the DoubleBounce method to initiate the effect
        performBounce = true;
        DoubleBounce();
    }
        
    private void OnDisable()
    {
        performBounce = false;
    }

    private void BounceUp(UnityAction onCompleteAction)
    {        
        if(bounceDirection == BounceDirection.Y)
        {
            transform.DOLocalMoveY(bounceHeight, bounceDuration)
                .SetEase(Ease.OutQuad) // Adjust the ease function as needed
                .OnComplete(() =>
                {
                    onCompleteAction?.Invoke();
                });
        }
        else if (bounceDirection == BounceDirection.X)
        {
            transform.DOLocalMoveX(bounceHeight, bounceDuration)
                .SetEase(Ease.OutQuad) // Adjust the ease function as needed
                .OnComplete(() =>
                {
                    onCompleteAction?.Invoke();
                });
        }
        else
        {
            transform.DOLocalMoveZ(bounceHeight, bounceDuration)
                .SetEase(Ease.OutQuad) // Adjust the ease function as needed
                .OnComplete(() =>
                {
                    onCompleteAction?.Invoke();
                });
        }
    }

    private void BounceDown(UnityAction onCompleteAction)
    {
        if(bounceDirection == BounceDirection.Y)
        {
            transform.DOLocalMoveY(0, bounceDuration)
                .SetEase(Ease.OutQuad) // Adjust the ease function as needed
                .OnComplete(() =>
                {
                    onCompleteAction?.Invoke();
                });
        }
        else if (bounceDirection == BounceDirection.X)
        {
            transform.DOLocalMoveX(0, bounceDuration)
                .SetEase(Ease.OutQuad) // Adjust the ease function as needed
                .OnComplete(() =>
                {
                    onCompleteAction?.Invoke();
                });
        }
        else
        {
            transform.DOLocalMoveZ(0, bounceDuration)
                .SetEase(Ease.OutQuad) // Adjust the ease function as needed
                .OnComplete(() =>
                {
                    onCompleteAction?.Invoke();
                });
        }
    }

    private void DoubleBounce()
    {
        BounceUp(() => { BounceDown(() => { BounceUp(() => { BounceDown( () => { DelayBetweenBounces(); }); }); } ); });
    }

    private void DelayBetweenBounces()
    {
        // After the second bounce, add a delay and then call DoubleBounce recursively for a loop
        DOVirtual.DelayedCall(delayBetweenBounces, () =>
        {
            if (performBounce)
            {
                DoubleBounce();
            }

        });
    }
}
