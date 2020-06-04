using UnityEngine;

public class Player : MonoBehaviour {
    public int id;
    public string username;

    private void Start() {
    }

    public void FixedUpdate() {
        
    }
    

    public void Initialize(int id, string username) {
        this.id = id;
        this.username = username;
    }
}