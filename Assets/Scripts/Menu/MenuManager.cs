using System.Collections;
using System.Collections.Generic;
using Character;
using Menu;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private int nextScene;
    [SerializeField] private int menuScene;
    public GameObject character;

    public void LoadNexScene()
    {
        SceneManager.LoadScene(nextScene);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    
    
    public void DebugBoss()
    {
        PlayerInformation.playerAmmo = 20000;
        PlayerInformation.playerHp = 20000;
        SceneManager.LoadScene(2);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadBoss()
    {
        PlayerInformation.playerAmmo = character.GetComponentInChildren<Weapon>().Ammo;
        PlayerInformation.playerHp = character.GetComponent<HealthPoints>().GetHp;
        SceneManager.LoadScene(2);
    }

    public void SetGodMode()
    {
        PlayerInformation.playerAmmo = 20000;
        PlayerInformation.playerHp = 20000;
    }
    
    public void SetNormalMode()
    {
        PlayerInformation.playerAmmo = 50;
        PlayerInformation.playerHp = 200;
    }

    public void Check()
    {
        Debug.Log(PlayerInformation.playerAmmo + "  " + PlayerInformation.playerHp);
    }

    public void SetCharacter(GameObject objectGame)
    {
        character = objectGame;
    }

}
