
using System.Collections;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int count = 2;

    private const float LowForce = 12f;
    private const float HighForce = 22f;

    private GameObject[] dices;
    private Dice[] diceScripts;
    private Rigidbody[] bodies;
    private bool done = true;

    private void Awake()
    {
        if (prefab == null || count <= 0) return;

        dices = new GameObject[count];
        diceScripts = new Dice[count];
        bodies = new Rigidbody[count];

        var columns = Mathf.CeilToInt(Mathf.Sqrt(count));
        var spacing = 2.5f;

        for (var i = 0; i < count; i++)
        {
            var spawned = Instantiate(prefab, transform);

            dices[i] = spawned;
            diceScripts[i] = spawned.GetComponent<Dice>();
            bodies[i] = spawned.GetComponent<Rigidbody>();

            var row = i / columns;
            var column = i % columns;

            var x = (column - (columns - 1) / 2f) * spacing;
            var z = (row - (columns - 1) / 2f) * spacing;

            x += Random.Range(-0.5f, 0.5f);
            z += Random.Range(-0.5f, 0.5f);

            spawned.transform.localPosition = new Vector3(x, 5f, z);
        }
    }

    private void Start()
    {
        if (dices != null)
        {
            Throw();
        }
    }

    private void Update()
    {
        var input = UnityEngine.InputSystem.Keyboard.current;

        if (input != null && input.spaceKey.wasPressedThisFrame)
        {
            Throw();
        }
    }

    private void Throw()
    {
        if (!done || dices == null) return;

        done = false;

        for (var i = 0; i < dices.Length; i++)
        {
            if (dices[i] == null || bodies[i] == null || diceScripts[i] == null)
                continue;

            diceScripts[i].ClearState();

            var push = Random.Range(LowForce, HighForce);
            var x = Random.Range(-1.5f, 1.5f);
            var z = Random.Range(-1.5f, 1.5f);

            var dir = new Vector3(x, 1f, z).normalized;
            var spin = Random.onUnitSphere;

            bodies[i].AddForce(dir * push, ForceMode.Impulse);
            bodies[i].AddTorque(spin * push, ForceMode.Impulse);
        }

        StartCoroutine(WaitDicesRoutine());
    }

    private IEnumerator WaitDicesRoutine()
    {
        yield return new WaitForFixedUpdate();

        while (!done)
        {
            var ready = true;
            var sum = 0;

            for (var i = 0; i < dices.Length; i++)
            {
                if (diceScripts[i] == null) continue;

                if (!diceScripts[i].IsStopped)
                {
                    ready = false;
                    break;
                }

                sum += diceScripts[i].Value;
            }

            if (ready)
            {
                done = true;
                Debug.Log($"Счет: {sum}");
            }

            yield return null;
        }
    }
}
