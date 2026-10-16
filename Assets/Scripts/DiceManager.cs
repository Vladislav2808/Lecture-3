
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int count = 2;

    private const float minForce = 12f;
    private const float maxForce = 20f;

    private Dice[] diceScripts;
    private Rigidbody[] bodies;

    private bool done = true;

    private void Awake()
    {
        diceScripts = new Dice[count];
        bodies = new Rigidbody[count];

        var spacing = 4f;
        var columns = Mathf.CeilToInt(Mathf.Sqrt(count));
        var rows = Mathf.CeilToInt((float)count / columns);

        for (var i = 0; i < count; i++)
        {
            var row = i / columns;
            var column = i % columns;

            var x = (column - (columns - 1) / 2f) * spacing;
            var z = (row - (rows - 1) / 2f) * spacing;

            var position = transform.position + new Vector3(x, 5f, z);
            var spawned = Instantiate(prefab, position, Random.rotation, transform);

            diceScripts[i] = spawned.GetComponent<Dice>();
            bodies[i] = spawned.GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        Throw();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
            Throw();
    }

    private void Throw()
    {
        if (!done)
            return;

        done = false;

        for (var i = 0; i < diceScripts.Length; i++)
        {
            diceScripts[i].ClearState();

            bodies[i].linearVelocity = Vector3.zero;
            bodies[i].angularVelocity = Vector3.zero;

            var force = Random.Range(minForce, maxForce);
            var x = Random.Range(-1.5f, 1.5f);
            var z = Random.Range(-1.5f, 1.5f);

            var direction = new Vector3(x, 1f, z).normalized;

            bodies[i].AddForce(direction * force, ForceMode.Impulse);

            var spin = Random.onUnitSphere * Random.Range(15f, 25f);
            bodies[i].AddTorque(spin, ForceMode.Impulse);
        }

        StartCoroutine(WaitDicesRoutine());
    }

    private IEnumerator WaitDicesRoutine()
    {
        var ready = false;
        var sum = 0;

        while (!ready)
        {
            ready = true;
            sum = 0;

            for (var i = 0; i < diceScripts.Length; i++)
            {
                if (!diceScripts[i].IsStopped)
                {
                    ready = false;
                    break;
                }

                sum += diceScripts[i].Value;
            }

            yield return null;
        }

        Debug.Log($"Счёт: {sum}");
        done = true;
    }
}
