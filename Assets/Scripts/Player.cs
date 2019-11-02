using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
	// TODO replace with Deck object
	public Deck deck;

	public GameObject hand;

	private bool isAi;
	public string Name;
	
	public Player(string name, bool isAi) {
		this.Name = name;
		this.isAi = isAi;

		deck = new Deck();
	}
}
