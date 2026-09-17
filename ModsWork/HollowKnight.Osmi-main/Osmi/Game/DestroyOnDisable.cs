namespace Osmi.Game;

public class DestroyOnDisable : MonoBehaviour {
	public bool recycle = false;

	protected void OnDisable() {
		if (recycle) {
			gameObject.Recycle();
		} else {
			Destroy(gameObject);
		}
	}
}
