using UnityEngine;
using System.Collections;

public class AbstractInGameMenu : MonoBehaviour {
    
	public virtual void LeaveMenu ()
    {
        gameObject.SetActive(false);
    }

    public virtual void SetData ( object[] args )
    {

    }
}
