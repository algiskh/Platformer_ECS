
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig", order = 1)]
public class PlayerConfig: ScriptableObject
{
	[SerializeField] private float _health;
	[SerializeField] private float _speed;

	public float Health => _health;
	public float Speed => _speed;
}