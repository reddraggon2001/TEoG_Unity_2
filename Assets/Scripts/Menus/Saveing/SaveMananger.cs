﻿using System;
using System.IO;
using UnityEngine;

public class SaveMananger : MonoBehaviour
{
    public static SaveMananger Instance { get; private set; }

    [SerializeField] private PlayerMain player = null;

    [SerializeField] private Dorm dorm = null;

    private DirectoryInfo SaveFolder;

    private string newSavePath, lastSavePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        player = player != null ? player : PlayerMain.GetPlayer;

        SaveFolder = Directory.Exists(SaveSettings.SaveFolder)
            ? new DirectoryInfo(SaveSettings.SaveFolder)
            : Directory.CreateDirectory(SaveSettings.SaveFolder);
        Settings.SetImperial = PlayerPrefs.HasKey("Imperial") ? PlayerPrefs.GetInt("Imperial") == 1 : false; ;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            NewSaveGame();
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            QuickLoad();
        }
    }

    public void NewSaveGame()
    {
        SaveName saveName = new SaveName(player, DateTime.Now);
        newSavePath = SaveFolder.FullName + saveName.CleanSave + ".json";
        File.WriteAllText(newSavePath, NewSave.SaveData());
        SavedEvent?.Invoke();
        lastSavePath = newSavePath;
    }

    public void SaveAndQuit()
    {
        NewSaveGame();
        if (File.Exists(lastSavePath))
        {
            Debug.Log("Pause menu; save & quit");
            Application.Quit();
        }
        else
        {
            Debug.LogError("Save failed...");
        }
    }

    public void QuickLoad()
    {
        string json = File.ReadAllText(lastSavePath);
        NewSave.LoadData(json);
    }

    public Save NewSave => new Save(player, dorm);

    public delegate void SavedGame();

    public static event SavedGame SavedEvent;
}

public class SaveName
{
    public SaveName(PlayerMain player, DateTime parDate)
    {
        Name = player.Identity.FullName;
        Lvl = player.ExpSystem.Level.ToString();
        Date = parDate.ToString();
    }

    private readonly string Name;
    private readonly string Lvl;
    private readonly string Date;

    public string CleanSave
    {
        get
        {
            string cleanNow = Date; //.Remove(Date.Length - 3, 3);
            //    .Replace(":", "-").Replace(" ", "-");
            string cleanPath = $"{Name}-Lvl{Lvl}-{cleanNow}";
            char[] illegal = Path.GetInvalidFileNameChars();
            // if file contain illegal chars replave them
            if (illegal.Length > 0 && cleanPath.IndexOfAny(illegal) != -1)
            {
                foreach (char c in illegal)
                {
                    if (cleanPath.IndexOf(c) != -1)
                    {
                        cleanPath = cleanPath.Replace(c.ToString(), "-");
                    }
                }
            }
            cleanPath = cleanPath.Replace(" ", "-");
            return cleanPath;
        }
    }
}