using UnityEngine;
[RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteRenderer))]
public class ProceduralObstacle : MonoBehaviour {
  float scrollSpeed; SpriteRenderer sr;
  public void Configure(float speed, Vector2 size, Color color){
    scrollSpeed = speed; sr = GetComponent<SpriteRenderer>(); sr.color = color;
    var bc = GetComponent<BoxCollider2D>(); bc.size = size;
    transform.localScale = new Vector3(size.x, size.y, 1f);
  }
  void Update(){
    transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime, Space.World);
    if (transform.position.x < -40f) gameObject.SetActive(false);
  }
}
