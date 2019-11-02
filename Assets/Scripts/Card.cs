﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Card : MonoBehaviour {
    public GameObject go;
    public Suit suit;
    public int value { get; set; }
    private string name;
    private Material frontFace;
    private Material backFace;
    
    private sealed class CardEqualityComparer : IEqualityComparer<Card> {
        public bool Equals(Card x, Card y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return Equals(x.go, y.go) && x.suit == y.suit && x.value == y.value && x.name == y.name && Equals(x.frontFace, y.frontFace) && Equals(x.backFace, y.backFace);
        }

        public int GetHashCode(Card obj) {
            unchecked {
                var hashCode = (obj.go != null ? obj.go.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) obj.suit;
                hashCode = (hashCode * 397) ^ obj.value;
                hashCode = (hashCode * 397) ^ (obj.name != null ? obj.name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.frontFace != null ? obj.frontFace.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.backFace != null ? obj.backFace.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public static IEqualityComparer<Card> CardComparer { get; } = new CardEqualityComparer();
}
