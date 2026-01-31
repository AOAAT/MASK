using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameStateSnapshot
{
    public Vector3 playerPosition;
    public List<IMaskPower> ownedMasks;
    public int[] altarStates;
    public bool[] maskActiveStates;

    public GameStateSnapshot(Vector3 pos, List<IMaskPower> masks, int[] altars, bool[] masksActive)
    {
        playerPosition = pos;
        ownedMasks = new List<IMaskPower>(masks);
        altarStates = (int[])altars.Clone();
        maskActiveStates = (bool[])masksActive.Clone();
    }
}