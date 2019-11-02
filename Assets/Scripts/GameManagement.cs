using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour{
    public static GameManagement instance;
    private Camera mainCamera;
	
    private Player red;
    private Player black;
    [SerializeField]
    private PlayMat _playMat;

    void Awake() {
        instance = this;
        mainCamera = Camera.main;
    }

    void Start () {
        red = new Player("red", true);
        black = new Player("black", false);

        _playMat.Initialize();
    }
}