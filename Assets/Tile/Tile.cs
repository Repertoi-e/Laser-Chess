using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour {
    
    void OnMouseEnter() {
        var animator = GetComponentInChildren<Animator>();
        animator.SetBool("isHovered", true);
    }

    void OnMouseExit() {
        
        var animator = GetComponentInChildren<Animator>();
        animator.SetBool("isHovered", false);
    }
}
