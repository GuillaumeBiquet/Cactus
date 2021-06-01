using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class GameSettings : ScriptableObject
{

    [SerializeField] string gameVersion = "0.0.0";
    [SerializeField] byte maxPlayers = 4;
    public string GameVersion { get { return gameVersion; } }
    public byte MaxPlayers { get { return maxPlayers; } }

}
