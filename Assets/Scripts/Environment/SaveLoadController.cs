using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;

public class SaveLoadController : MonoBehaviour
{
    [Header("Character")]
    private GameObject _character;
    private Weapon _weapon;
    private HealthPoints _healthPoints;
    private Vector3 _savePosition;
    private int _saveAmmo;
    private int _saveHp;
    private bool _isDataSaved;

    private void Start()
    {
        // _weapon = character.GetComponentInChildren<Weapon>();
        // _healthPoints = character.GetComponent<HealthPoints>();
    }

    public void SaveState()
    {
        _isDataSaved = true;
        _savePosition = _character.transform.position;
        _saveAmmo = _weapon.Ammo;
        _saveHp = _healthPoints.GetHp;
    }

    public void LoadState()
    {
        if (!_isDataSaved)
        {
            return;
        }

        _character.SetActive(true);
        _character.transform.position = _savePosition;
        _weapon.Ammo = _saveAmmo;
        _healthPoints.SetHp(_saveHp);
    }

    public void PauseScene()
    {
        Time.timeScale = 0;
    }

    public void ResumeScene()
    {
        Time.timeScale = 1f;
    }

    public void SetCharacter(GameObject gameObjectNice)
    {
        _character = gameObjectNice;
        _weapon = _character.GetComponentInChildren<Weapon>();
        _healthPoints = _character.GetComponent<HealthPoints>();
    }
}
