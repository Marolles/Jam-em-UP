using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    public static CrowdManager instance;
    public static List<Spectator> spectators;

    private void Awake()
    {
        if (instance != null) { Destroy(instance); }
        instance = this;

        spectators = new List<Spectator>();
    }

    private void Start()
    {
        Invoke("ShuffleSpectatorList", Time.deltaTime);
    }

    private void ShuffleSpectatorList()
    {
        Debug.Log("Spectator amount: " + spectators.Count);
        spectators = spectators.OrderBy(i => Guid.NewGuid()).ToList();
    }

    public void UpdateCrowdColor() //Must be called EVERY TIME the fame gets updated
    {
        float _fameValue = FameController.GetFameValueNormalized();
        float _angrynessPercent = 1f - (_fameValue * 2);
        float _happinessPercent = 2 * (_fameValue - 0.5f);
        //List<Spectator> _spectatorsAffectedByChange = GetSpectatorsFromRange()

        for (int i = 0; i < spectators.Count; i++)
        {
            float _spectatorPercentValue = (float)i / (float)spectators.Count;
            if (_fameValue < 0.5)
            {
                //People are angry, color them accordingly
                if (_spectatorPercentValue > _angrynessPercent)
                {
                    spectators[i].SetNeutral();
                } else
                {
                    spectators[i].SetAngry();
                }
            }
            else if (_fameValue > 0.5)
            {
                if (_spectatorPercentValue > _happinessPercent)
                {
                    spectators[i].SetNeutral();
                } else
                {
                    spectators[i].SetHappy();
                }
            } else
            {
                spectators[i].SetNeutral();
            }
        }
    }
}
