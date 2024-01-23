using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;

public class ArenaController : MonoBehaviour
{
    public static ArenaController instance;

    private static bool frozen = false;

    [SerializeField] private CinemachineVirtualCamera kingCamera;

    [SerializeField] private Transform king;
    [SerializeField] private float turnDuration = 1f;
    [SerializeField] private Ease turnEase = Ease.Linear;

    [Header("Fame Settings")]
    [SerializeField] private int requiredFameForMercy = 50;

    public static List<PawnController> alivePawns = new List<PawnController>();

    private void Awake()
    {
        if (instance != null) { Destroy(instance); }
        instance = this;
        frozen = false;
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

    public void AskKingForMercy()
    {
        Invoke("ForceLookAtKing", 0.5f);
    }

    public void ForceLookAtKing()
    {
        Debug.Log("KING demands your attention! ");
        //Everybody will look at its MAJESTY
        foreach (PawnController _controller in alivePawns)
        {
            if (_controller != null)
            {
                Vector3 _toward = instance.king.transform.position - _controller.transform.position;
                DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () => _controller.transform.DOLookAt(_toward, instance.turnDuration, AxisConstraint.Y).SetEase(instance.turnEase));
            }
        }

        //Including PEASANTS
        foreach (Spectator _spectator in CrowdManager.spectators)
        {
            Vector3 _toward = instance.king.transform.position - _spectator.transform.position;
            DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () => _spectator.transform.DOLookAt(_toward, instance.turnDuration, AxisConstraint.Y).SetEase(instance.turnEase));
            //_spectator.transform.DOLookAt(_toward, instance.turnDuration, AxisConstraint.Y).SetEase(instance.turnEase);
        }

        //Camera travel HERE <=======================
        kingCamera.m_Priority = 11;

        Invoke("KingJudgement", turnDuration + 1f);
    }

    private void KingJudgement()
    {
        //Show either a THUMBS UP or THUMBS DOWN

        if (FameController.GetFameValue() > requiredFameForMercy)
        {
            Invoke("ShowMercy", 1f);
        } else
        {
            Invoke("KillPlayer", 1f);
        }
    }

    private void KillPlayer()
    {
        //Put back camera HERE <======
        kingCamera.m_Priority = 9;

        //Death anim for player

        Invoke("GameOver", 1f);
    }

    public void GameOver()
    {
        GameOverPanel.instance.ShowGameOverPanel();
    }

    private void ShowMercy()
    {
        //Put back camera HERE <======
        kingCamera.m_Priority = 9;

        UnfreezeArena();

        PlayerController.instance.Regenerate();
    }
}
