using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    public static ArenaController instance;

    private static bool frozen = false;

    [SerializeField] private Transform king;
    [SerializeField] private float turnDuration = 1f;
    [SerializeField] private Ease turnEase = Ease.Linear;
    public static List<PawnController> alivePawns = new List<PawnController>();

    private void Awake()
    {
        if (instance != null) { Destroy(instance); }
        instance = this;
        frozen = false;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            FreezeArena();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            UnfreezeArena();
            PlayerController.instance.Regenerate();
        }
    }

    public static bool IsFrozen()
    {
        return frozen;
    }
    public static void FreezeArena() //Prevent ANY movement or ANY action in the arena from happening
    {
        frozen = true;
    }

    public static void UnfreezeArena()
    {
        frozen = false;
    }

    public static async Task ForceLookAtKing(float _delay)
    {
        await Task.Delay((int)_delay * 1000);

        //Everybody will look at its MAJESTY
        foreach (PawnController _controller in alivePawns)
        {
            Vector3 _toward = instance.king.transform.position - _controller.transform.position;
            _controller.transform.DOLookAt(_toward, instance.turnDuration, AxisConstraint.Y).SetEase(instance.turnEase);
        }

        //Including PEASANTS
        foreach (Spectator _spectator in CrowdManager.spectators)
        {
            Vector3 _toward = instance.king.transform.position - _spectator.transform.position;
            _spectator.transform.DOLookAt(_toward, instance.turnDuration, AxisConstraint.Y).SetEase(instance.turnEase);
        }
    }
}
