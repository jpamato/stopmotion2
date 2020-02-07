using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Events {
	//Input Manager
	public static System.Action<GameObject> OnMouseCollide = delegate { };
	public static System.Action OnKeyRed = delegate { };
	public static System.Action OnKeyYellow = delegate { };
	public static System.Action OnKeyGreen = delegate { };
	public static System.Action OnKeyP = delegate { };

	public static System.Action OnAnyKey = delegate { };

	public static System.Action<int> ShowFrame = delegate { };

	public static System.Action OnConfig = delegate { };
}
