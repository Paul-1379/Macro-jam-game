using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject toInstantiateAtStart;
    
    [HideInInspector] public List<Mirror> mirrors = new List<Mirror>();
    public static GameManager Instance;

    private void Awake()
    {
        SetSingleton();

        Instantiate(toInstantiateAtStart);
    }

    private void SetSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Game Manager is already instantiated");
        }
    }
}
