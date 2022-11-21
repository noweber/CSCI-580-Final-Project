using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fractal : MonoBehaviour
{
	public Mesh mesh;
    public Mesh leaves;
    public Material leafmaterial;
	public Material material;
    public int maxDepth;
    private int depth;
    public float childScale;
    public float angle;
    public float angle2;
    public int level;
    public Fractal parent;

	private void Start () {
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = material;

                transform.localScale = new Vector3(.5f, 1f, .5f);

        if (depth < maxDepth) {
			StartCoroutine(CreateChildren());
		}


	}
	private IEnumerator CreateChildren () {
        for (int i = 0; i < level + 1; i++) {
		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").
			AddComponent<Fractal>().Initialize(this);
        }
	}

	private void Initialize (Fractal par) {
        parent = par;
		mesh = parent.mesh;
		material = parent.material;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
        childScale = parent.childScale;
        level = parent.level + parent.parent.level;
        angle = parent.angle +  Random.Range(-30.0f, 45.0f) ;
        angle2 = parent.angle2 + Random.Range(-30.0f, 45.0f) ;

        

        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.Rotate(angle + parent.angle, 0, angle2 + parent.angle2);
        transform.localPosition = parent.transform.up * (parent.transform.localScale.y);
    




	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
