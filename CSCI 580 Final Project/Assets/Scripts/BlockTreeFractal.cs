using System.Collections.Generic;
using UnityEngine;

public class BlockTreeFractal : MonoBehaviour
{
    public string TreeIdenfitier = string.Empty;
    public int MaxTreeNodeDepth = 8;
    public int MinTreeNodeDepth = 2;
    public int LeafNodeDepth = 4;
    private int targetNodeDepth = 0;
    private int currentNodeDepth = 0;
    public Mesh BranchMesh;
    public Material BranchMaterial;
    public Mesh LeafMesh;
    public Material LeafMaterial;
    public float BranchScale = 1.0f;
    public float MaxChildScale = 1.0f;
    public float MinChildScale = 0.6f;
    private Vector3 growthDirection;
    private int maxNumberOfChildNodes = 1;
    private List<GameObject> childFractals;
    private bool finishedAddingChildren = false;

    private void Start()
    {
        if (currentNodeDepth != 0)
        {
            if (this.currentNodeDepth < this.LeafNodeDepth)
            {
                gameObject.AddComponent<MeshFilter>().mesh = BranchMesh;
                gameObject.AddComponent<MeshRenderer>().material = BranchMaterial;
            }
            else
            {
                //Debug.Log("Leaf!");
                gameObject.AddComponent<MeshFilter>().mesh = LeafMesh;
                gameObject.AddComponent<MeshRenderer>().material = LeafMaterial;
            }
        }
        else
        {
            growthDirection = Vector3.up;
        }
        targetNodeDepth = Random.Range(MinTreeNodeDepth, MaxTreeNodeDepth);
        maxNumberOfChildNodes = System.Math.Min(currentNodeDepth + 1, 6);
        childFractals = new List<GameObject>();
        if (MinChildScale > MaxChildScale)
        {
            MinChildScale = MaxChildScale;
        }
        if (MinTreeNodeDepth > MaxTreeNodeDepth)
        {
            MinTreeNodeDepth = MaxTreeNodeDepth;
        }
    }

    private string GetChildNodeName(int childIndex)
    {
        string childIdentifier = string.Empty;
        if (!string.IsNullOrEmpty(TreeIdenfitier))
        {
            childIdentifier = TreeIdenfitier + ":child:" + childIndex;
        }
        else
        {
            childIdentifier = System.Guid.NewGuid().ToString();
        }
        return childIdentifier;
    }

    public float GrowthAnimationFrameTimeInSeconds = 0.25f;
    private float secondsSinceLastGrowthAnimation = 0;
    void Update()
    {
        if(finishedAddingChildren)
        {
            return;
        }

        if (currentNodeDepth >= targetNodeDepth)
        {
            //Debug.Log("Recursion depth has reached its maximum.");
            return;
        }

        if (this.childFractals.Count >= this.maxNumberOfChildNodes)
        {
            //Debug.Log("Child fractal count has reached its maximum.");
            finishedAddingChildren = true;
            return;
        }

        secondsSinceLastGrowthAnimation += Time.deltaTime;
        if (secondsSinceLastGrowthAnimation < GrowthAnimationFrameTimeInSeconds)
        {
            return;
        }
        secondsSinceLastGrowthAnimation -= GrowthAnimationFrameTimeInSeconds;

        if (currentNodeDepth == 0)
        {
            AddChildNode(this.growthDirection, GetChildNodeName(0));
            return;
        }

        Vector3 childNodeGrowthDirection = GetNextChildDirection();
        if (childNodeGrowthDirection != Vector3.zero)
        {
            AddChildNode(childNodeGrowthDirection, GetChildNodeName(this.childFractals.Count));
        }
        else
        {
            finishedAddingChildren = true;
        }
    }

    private void AddChildNode(Vector3 childDirection, string childId)
    {
        Debug.Log("AddChildNode()");
        GameObject childFractal = new GameObject("fractal:" + childId);
        this.childFractals.Add(childFractal);
        childFractal.AddComponent<BlockTreeFractal>().Initialize(this, childDirection);
        // TODO: Implement a method to delete all children so the fract can be regenerated.
    }

    private void Initialize(BlockTreeFractal parentNode, Vector3 childDirection)
    {
        Debug.Log("Initialize()");
        this.BranchMesh = parentNode.BranchMesh;
        this.BranchMaterial = parentNode.BranchMaterial;
        this.LeafMesh = parentNode.LeafMesh;
        this.LeafMaterial = parentNode.LeafMaterial;
        this.LeafNodeDepth = parentNode.LeafNodeDepth;
        this.MinTreeNodeDepth = parentNode.MinTreeNodeDepth;
        this.MaxTreeNodeDepth = parentNode.MaxTreeNodeDepth;
        this.currentNodeDepth = parentNode.currentNodeDepth + 1;
        this.transform.parent = parentNode.transform;
        this.MaxChildScale = parentNode.MaxChildScale;
        this.MinChildScale = parentNode.MinChildScale;
        float childScale = Random.Range(this.MinChildScale, this.MaxChildScale);
        this.transform.localScale = (Vector3.one * childScale);
        float halfBranchScale = BranchScale / 2;
        this.transform.localPosition = childDirection * (halfBranchScale + halfBranchScale * childScale);
        this.growthDirection = childDirection;
    }

    private HashSet<Vector3> childDirectionsUsed = new HashSet<Vector3>();
    private Vector3 GetNextChildDirection()
    {
        Vector3 result = Vector3.zero;
        int numberOfAttempts = 0;
        while (result == Vector3.zero || (childDirectionsUsed.Contains(result) && numberOfAttempts < 5))
        {
            int selection = Random.Range(0, 16);
            if (selection < 5)
            {
                result = Vector3.up;
            }
            else if (selection == 5)
            {
                result = Vector3.right;
            }
            else if (selection == 6)
            {
                result = Vector3.left;
            }
            else if (selection == 7)
            {
                result = Vector3.forward;
            }
            else if (selection == 8)
            {
                result = Vector3.back;
            }
            else if (selection == 9)
            {
                result = Vector3.down;
            }
            else
            {
                result = this.growthDirection;
            }
            numberOfAttempts++;
        }

        if ((result == Vector3.left && this.growthDirection == Vector3.right) ||
            (result == Vector3.right && this.growthDirection == Vector3.left) ||
            (result == Vector3.up && this.growthDirection == Vector3.down) ||
            (result == Vector3.down && this.growthDirection == Vector3.up) ||
            (result == Vector3.forward && this.growthDirection == Vector3.back) ||
            (result == Vector3.back && this.growthDirection == Vector3.forward))
        {
            result = this.growthDirection;
        }

        if (childDirectionsUsed.Contains(result))
        {
            result = this.growthDirection;

            if (childDirectionsUsed.Contains(this.growthDirection))
            {
                result = Vector3.zero;
            }
        }

        childDirectionsUsed.Add(result);
        return result;
    }
}
