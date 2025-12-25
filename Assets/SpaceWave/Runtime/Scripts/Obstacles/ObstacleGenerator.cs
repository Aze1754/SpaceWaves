using UnityEngine;
public class ObstacleGenerator : MonoBehaviour {
  [SerializeField] Transform spawnRoot;
  [SerializeField] float scrollSpeed = 10f, spacing = 10f;
  [SerializeField] Color colorCircle = new Color(0.3f,0.9f,1f), colorSquare = new Color(0.7f,0.2f,1f), colorTriangle = new Color(1f,0.2f,0.6f);
  [SerializeField] GameObject obstaclePrefab;
  float nextX;
  void Start(){ if(!spawnRoot) spawnRoot = transform; nextX = 20f; }
  void Update(){ EnsureContinuous(); }
  void EnsureContinuous(){ float camRight = 30f; while (nextX < camRight){ SpawnPattern(nextX); nextX += spacing; } }
  void SpawnPattern(float baseX){
    int selector = Random.Range(0,3); float y = Random.Range(-3.5f,3.5f);
    var go = Instantiate(obstaclePrefab, spawnRoot); go.transform.position = new Vector3(baseX, y, 0);
    var po = go.GetComponent<ProceduralObstacle>(); Vector2 size; Color c;
    switch(selector){ case 0: size=new Vector2(1.5f,4f); c=colorSquare; break;
      case 1: size=new Vector2(1.2f,1.2f); c=colorCircle; break;
      default: size=new Vector2(1.8f,2.0f); c=colorTriangle; break; }
    po.Configure(scrollSpeed, size, c);
  }
}
