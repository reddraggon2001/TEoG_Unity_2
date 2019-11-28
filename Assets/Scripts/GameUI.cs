﻿using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    // In menu pause short keys still work
    private bool menuPaused = false;

    // Disable shortkeys
    private bool totalPaused = false;

    public KeyBindings keys;

    [Header("Main panels")]
    public GameObject gameui;

    public GameObject battle;
    public GameObject menus;
    public GameObject buildings;

    [Header("Battle panels")]
    public GameObject combat;

    public GameObject sex;
    public GameObject lose;
    public GameObject home;

    [Header("Options, Save ,etc")]
    public GameObject pausemenu;

    public GameObject savemenu;
    public GameObject options;
    public GameObject bigeventlog;
    public GameObject questMenu;
    public GameObject inventory;
    public GameObject vore;
    public GameObject essence;
    public GameObject levelUp;
    public GameObject looks;

    [Header("Eventlog stuff")]
    public GameObject openEventlog;

    public GameObject closedEventlog;

    [Space]
    public CombatMain combatButtons;

    public AfterBattleMain afterBattleActions;
    private float _eventTime;

    private void Update()
    {
        if (Input.GetKeyDown(keys.escKey))
        {
            EscapePause();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!totalPaused || !menuPaused)
            {
                gameui.SetActive(!gameui.activeSelf);
            }
        }
        // if in menus or main game(not combat)
        if (!totalPaused)
        {
            if (Input.GetKeyDown(keys.saveKey))
            {
                ResumePause(savemenu);
            }
            else if (Input.GetKeyDown(keys.optionsKey))
            {
                ResumePause(options);
            }
            else if (Input.GetKeyDown(keys.questKey))
            {
                ResumePause(questMenu);
            }
            else if (Input.GetKeyDown(keys.inventoryKey))
            {
                ResumePause(inventory);
            }
            else if (Input.GetKeyDown(keys.voreKey))
            {
                ResumePause(vore);
            }
            else if (Input.GetKeyDown(keys.essenceKey))
            {
                ResumePause(essence);
            }
            else if (Input.GetKeyDown(keys.lvlKey))
            {
                ResumePause(levelUp);
            }
            else if (Input.GetKeyDown(keys.eventKey))
            {
                _eventTime = Time.time;
            }
            else if (Input.GetKeyDown(keys.lookKey))
            {
                ResumePause(looks);
            }
            if (Input.GetKeyUp(keys.eventKey))
            {
                if (Time.time - _eventTime > 0.8f)
                {
                    openEventlog.SetActive(openEventlog.activeSelf ? false : true);
                    closedEventlog.SetActive(closedEventlog.activeSelf ? false : true);
                }
                else if (BigEventLog()) { }
            }
        }
    }

    public void Resume()
    {
        foreach (Transform child in menus.transform)
        {
            child.gameObject.SetActive(false);
        }
        ToggleBigPanel(gameui);
        menuPaused = false;
        totalPaused = false;
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        ToggleBigPanel(menus);
        foreach (Transform child in menus.transform)
        {
            child.gameObject.SetActive(false);
        }
        menuPaused = true;
        Time.timeScale = 0f;
    }

    public void TotalPause()
    {
        totalPaused = true;
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool BigEventLog()
    {
        if (gameui.activeSelf)
        {
            Pause();
            bigeventlog.SetActive(true);
            return true;
        }
        Resume();
        return false;
    }

    public void StartCombat(EnemyPrefab enemy)
    {
        TotalPause();
        foreach (Transform p in battle.transform)
        {
            p.gameObject.SetActive(false);
        }
        ToggleBigPanel(battle);
        List<EnemyPrefab> toAdd = new List<EnemyPrefab> { enemy };
        combatButtons.SetUpCombat(toAdd);
    }

    public void LeaveCombat()
    {
        Resume();
    }

    private void ToggleBigPanel(GameObject toActivate)
    {
        foreach (Transform bigPanel in transform)
        {
            bigPanel.gameObject.SetActive(false);
        }
        toActivate.SetActive(true);
    }

    public void ResumePause(GameObject toBeActivated)
    {
        if (menuPaused)
        {
            Resume();
        }
        else
        {
            Pause();
            toBeActivated.SetActive(true);
        }
    }

    public void EnterHome()
    {
        TotalPause();
        ToggleBigPanel(home);
        foreach (Transform child in home.transform)
        {
            child.gameObject.SetActive(false);
        }
        home.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void LeaveHome()
    {
        Resume();
    }

    public void EscapePause()
    {
        if (menus.activeSelf)
        {
            Resume();
        }
        else if (pausemenu.activeSelf)
        {
            Time.timeScale = 1f;
            menuPaused = false;
            pausemenu.SetActive(false);
        }
        else
        {
            menuPaused = true;
            Time.timeScale = 0f;
            pausemenu.SetActive(true);
        }
    }

    public void EnterBuilding()
    {
        TotalPause();
        ToggleBigPanel(buildings);
        // Disable all buildings
        foreach (Transform building in buildings.transform)
        {
            building.gameObject.SetActive(false);
        }
    }
}