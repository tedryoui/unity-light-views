
using Assets;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : View<MainMenuController>
{
    [SerializeField, ViewChild] private Button settings;
    [SerializeField, ViewChild] private Button levelMap;

    public event Action onSettingsClicked;
    public event Action onLevelMapClicked;

    private void Awake()
    {
        onSettingsClicked = delegate { };
        onLevelMapClicked = delegate { };
    }

    protected override void Start()
    {
        base.Start();

        settings.onClick.AddListener(() =>  onSettingsClicked.Invoke());
        levelMap.onClick.AddListener(() => onLevelMapClicked.Invoke());
    }
}

public class MainMenuController : ViewModel<MainMenu>
{
    public override void Initialize()
    {
        View.onSettingsClicked += OpenSettings;
        View.onLevelMapClicked += OpenLevelMap;
    }

    public void OpenLevelMap()
    {
        var levelMapView = ViewCenter.GetViewModel<LevelMapController>().View;

        var isLevelMapOpened = levelMapView.IsOpened;

        if(!isLevelMapOpened)
            levelMapView.Open();
    }

    public void OpenSettings()
    {
        var settingsView = ViewCenter.GetViewModel<SettingsController>().View;

        var isSettingsOpened = settingsView.IsOpened;

        if(!isSettingsOpened)
            settingsView.Open();
    }
}