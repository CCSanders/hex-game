using UnityEngine;

public class MapCamera : MonoBehaviour {

	public float stickMinZoom, stickMaxZoom;

	public float swivelMinZoom, swivelMaxZoom;

	public float moveSpeedMinZoom, moveSpeedMaxZoom;

	public float rotationSpeed;

    public Transform Stick
    {
        get
        {
            return stick;
        }
        set { }
    }

    public Transform Swivel
    {
        get
        {
            return swivel;
        }
        set { }
    }

	Transform swivel, stick;

	public HexGrid grid;

	float zoom = 1f;

	float rotationAngle;

	static MapCamera instance;

	public static bool Locked {
		set {
			instance.enabled = !value;
		}
	}

	public static void ValidatePosition () {
		instance.AdjustPosition(0f, 0f);
	}

	void Awake () {
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);
	}

	void OnEnable () {
		instance = this;
		ValidatePosition();
        ResetCamera();
	}

	void Update () {
		float rotationDelta = Input.GetAxis("Rotation");
		if (rotationDelta != 0f) {
			AdjustRotation(rotationDelta);
		}

		float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");
		if (xDelta != 0f || zDelta != 0f) {
			AdjustPosition(xDelta, zDelta);
		}
	}

	public void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01(zoom + delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);

		float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}

    public float GetZoom()
    {
        return zoom;
    }

	void AdjustRotation (float delta) {
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		if (rotationAngle < 0f) {
			rotationAngle += 360f;
		}
		else if (rotationAngle >= 360f) {
			rotationAngle -= 360f;
		}
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
	}

	void AdjustPosition (float xDelta, float zDelta) {
		Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition =
			grid.wrapping ? WrapPosition(position) : ClampPosition(position);
	}

    public void MoveToPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

	Vector3 ClampPosition (Vector3 position) {
		float xMax = (grid.cellCountX - 0.5f) * HexMetrics.innerDiameter;
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		return position;
	}

	Vector3 WrapPosition (Vector3 position) {
		float width = grid.cellCountX * HexMetrics.innerDiameter;
		while (position.x < 0f) {
			position.x += width;
		}
		while (position.x > width) {
			position.x -= width;
		}

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		grid.CenterMap(position.x);
		return position;
	}

    public void ResetCamera()
    {
        float width = grid.cellCountX * HexMetrics.innerDiameter;
        float height = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
        transform.position = new Vector3(width / 2, transform.position.y, height / 2);
        transform.rotation = Quaternion.identity;
        rotationAngle = 0;

        grid.CenterMap(transform.position.x);
    }
}