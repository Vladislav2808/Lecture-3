using UnityEngine;

public class Dice : MonoBehaviour
{
    public int Value { get; private set; }
    public bool IsStopped { get; private set; }

    private readonly Vector3[] dirs = {
        Vector3.up, Vector3.down, Vector3.right,
        Vector3.left, Vector3.forward, Vector3.back
    };

    private readonly int[] nums = { 1, 6, 5, 2, 4, 3 };

    private Rigidbody body;
    private float stopTime;
    private float StopDelay = 0.5f;
    private float StopSpeed = 0.01f;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var isSlow = body.linearVelocity.sqrMagnitude < StopSpeed &&
                     body.angularVelocity.sqrMagnitude < StopSpeed;

        if (!isSlow)
        {
            stopTime = 0f;
            IsStopped = false;
            return;
        }

        stopTime += Time.deltaTime;

        if (!IsStopped && stopTime >= StopDelay)
        {
            Value = FindTop();
            IsStopped = true;
        }
    }

    private int FindTop()
    {
        var minAngle = 180f;
        var index = 0;

        for (var i = 0; i < 6; i++)
        {
            var worldVector = transform.rotation * dirs[i];
            var angle = Vector3.Angle(worldVector, Vector3.up);

            if (angle < minAngle)
            {
                minAngle = angle;
                index = i;
            }
        }

        return nums[index];
    }

    public void ClearState()
    {
        IsStopped = false;
        Value = 0;
        stopTime = 0f;
    }
}
