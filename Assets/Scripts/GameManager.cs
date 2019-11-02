using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    public static GameManager instance;
    private Camera mainCamera;
	
    private Player red;
    private Player black;
    [SerializeField]
	private Player currentPlayer;

    void Awake() {
        instance = this;
        mainCamera = Camera.main;
    }

	void Start () {
        red = new Player("red", true);
        black = new Player("black", false);

		// TODO Create player method that creates deck and such?
        currentPlayer = white;
        otherPlayer = black;

        InitialSetup();
    }

	private void InitialSetup() {
		// TODO Spawn decks

		// Draw hands

	}
}
