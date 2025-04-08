using UnityEngine;

[CreateAssetMenu(fileName = "WalkerSO", menuName = "Scriptable Objects/WalkerSO")]
public class WalkerSO : ScriptableObject
{
	public AnimationCurve HorizontalCurve;
	public AnimationCurve VerticalCurve;
}
