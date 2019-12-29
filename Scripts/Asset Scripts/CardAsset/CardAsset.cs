using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetingOptions
{
    NoTarget,
    AllCreatures, 
    EnemyCreatures,
    YourCreatures, 
    AllCharacters, 
    EnemyCharacters,
    YourCharacters
}
public class CardAsset : ScriptableObject 
{
    [Header("General info")]
    public CharacterAsset characterAsset;  
    [TextArea(2,3)]
    public string Description; 
	public Sprite CardImage;
    public int ManaCost;

    [Header("Creature Info")]
    public int MaxHealth;
    public int Attack;
    public int AttacksForOneTurn = 1;
    public bool Taunt;
    public bool Charge;
    public string CreatureScriptName;
    public int specialCreatureAmount;

    [Header("SpellInfo")]
    public string SpellScriptName;
    public int specialSpellAmount;
    public TargetingOptions Targets;

}
