using UnityEngine; using UnityEngine.SceneManagement;
public class UIBootstrap : MonoBehaviour {
  public void PlayEndless()=>SceneManager.LoadScene("Game");
  public void Quit(){ #if UNITY_EDITOR UnityEditor.EditorApplication.isPlaying=false; #else Application.Quit(); #endif }
}
