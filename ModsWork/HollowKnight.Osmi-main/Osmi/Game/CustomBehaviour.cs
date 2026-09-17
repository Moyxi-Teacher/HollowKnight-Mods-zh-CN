namespace Osmi.Game;

[PublicAPI]
public class CustomBehaviour : MonoBehaviour {
	public event Action<CustomBehaviour> StartEvent = null!;
	public event Action<CustomBehaviour> OnEnableEvent = null!;
	public event Action<CustomBehaviour> OnDisableEvent = null!;
	public event Action<CustomBehaviour> OnDestroyEvent = null!;
	public event Action<CustomBehaviour> UpdateEvent = null!;
	public event Action<CustomBehaviour> FixedUpdateEvent = null!;

	protected void Start() => StartEvent?.Invoke(this);
	protected void OnEnable() => OnEnableEvent?.Invoke(this);
	protected void OnDisable() => OnDisableEvent?.Invoke(this);
	protected void OnDestroy() => OnDestroyEvent?.Invoke(this);
	protected void Update() => UpdateEvent?.Invoke(this);
	protected void FixedUpdate() => FixedUpdateEvent?.Invoke(this);
}
