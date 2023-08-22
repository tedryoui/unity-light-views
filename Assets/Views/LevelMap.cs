using Assets;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelMap : View<LevelMapController>
{
    [SerializeField, ViewChild] private Button close;

    public Action onViewOpen;

    private void Awake()
    {
        onViewOpen = delegate { };
    }

    protected override void Start()
    {
        base.Start();

        close.onClick.AddListener(Close);
    }

    public override void Open()
    {
        base.Open();

        onViewOpen?.Invoke();
    }
}

public class LevelMapController : ViewModel<LevelMap>
{
    public override void Initialize()
    {
        View.onViewOpen += UpdateView;
        UpdateView();
    }

    private void UpdateView()
    {
        Debug.Log("Update level map view");
    }
}