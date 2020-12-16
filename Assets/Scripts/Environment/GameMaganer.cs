using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Cinemachine;
using Menu;
using UnityEngine;

public class GameMaganer : MonoBehaviour
{
    public GameObject character;
    private GameObject _characterClone;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    private Weapon _characterWeapon;
    private HealthPoints _characterHealthPoints;
    public DeathMenuManager deathMenuManager;
    public Transform characterStartPosition;
    public SaveLoadController saveLoadController;
    public MenuManager menuManager;


    private void Start()
    {
        character.GetComponentInChildren<Weapon>().Ammo = PlayerInformation.playerAmmo;
        _characterHealthPoints = character.GetComponent<HealthPoints>();
        _characterHealthPoints.SetHp(PlayerInformation.playerHp);
        _characterHealthPoints.SetDeathMenuManager(deathMenuManager);

        
        _characterClone = Instantiate(character, characterStartPosition.position, characterStartPosition.rotation);
        
        cinemachineVirtualCamera.Follow = _characterClone.transform;
        
        saveLoadController.SetCharacter(_characterClone);
        
        menuManager.SetCharacter(_characterClone);
    }
}
