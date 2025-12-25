// Strict constant-speed vertical control (no inertia)

using UnityEngine;


using UnityEngine.InputSystem;


namespace SpaceWave.Gameplay {


  public class WaveController : MonoBehaviour {


    [SerializeField] float verticalSpeed = 8f;


    [SerializeField] float yMin = -4.5f, yMax = 4.5f;


    bool isPressing; Vector3 pos;


    void Awake(){ Application.targetFrameRate = 60; QualitySettings.vSyncCount = 0; pos = transform.position; }


    public void OnPress(InputAction.CallbackContext ctx){ if (ctx.started || ctx.performed) isPressing = true; if (ctx.canceled) isPressing = false; }


    void UpdateInputLegacy(){ isPressing = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0); }


    void Update(){


      UpdateInputLegacy();


float dy = (isPressing ? +verticalSpeed : -verticalSpeed) * Time.deltaTime;
      pos.y = Mathf.Clamp(pos.y + dy, yMin, yMax);
      transform.position = pos;
    }
  }
}


