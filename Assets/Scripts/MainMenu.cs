using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour{
    public void BeginButton() {
        SceneManager.LoadScene(1);
    }

    public void EndGame(){
        Application.Quit();
    }

    public void OnMouseEnter() {
        GetComponent<Renderer>().material.color = Color.red;		//Change Color to red!
    }

    public void OnMouseExit(){
        GetComponent<Renderer>().material.color = Color.white;		//Change Color to white!
    }
}