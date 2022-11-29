using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LindenmayerSystemFractal : MonoBehaviour
{
    [Range(0, 0)]
    public int Seed;
    [Range(0, 1024)]
    public int Iterations = 2;
    public int IterationVariance = 1;
    public string HardCodedFractal = string.Empty;
    [Range(0, 90.0f)]
    public float Angle = 25.0f;
    public bool RandomAngle = true;
    public Mesh BranchMesh;
    public Material BranchMaterial;
    public Vector3 BranchMeshSize = new Vector3(1, 1, 1);
    public Mesh LeafMesh;
    public Material LeafMaterial;
    public Vector3 LeafMeshSize = new Vector3(1, 1, 1);
    private string axiom;
    private Dictionary<char, string> productionRules;
    private Dictionary<char, Action> productionActions;
    private Stack<Tuple<Vector3, Vector3, Vector3, Vector3>> stateStack;
    private string fractalString;
    private int currentFractalStringIndex;
    private Vector3 currentDirection;
    private GameObject fractalParent;
    private float[] iterationRotations;

    [Range(0, 100)]
    public int LeafPercentage = 50;
    public float IterationScaleFactor = 0.95f;
    public float ScaleVariance = 0.05f;

    private void Start()
    {
        if(this.RandomAngle)
        {
            Angle = UnityEngine.Random.Range(25, 75.0f);
        }
        this.axiom = null;
        this.productionRules = null;
        this.fractalString = string.Empty;
        this.currentFractalStringIndex = 0;
        this.currentDirection = Vector3.up;
        this.Iterations = UnityEngine.Random.Range(this.Iterations - this.IterationVariance, this.Iterations + this.IterationVariance);
        this.iterationRotations = new float[this.Iterations];
        for (int i = 0; i < this.Iterations; i++)
        {
            this.iterationRotations[i] = UnityEngine.Random.Range(-1.0f, 1.0f);
        }
        this.stateStack = new Stack<Tuple<Vector3, Vector3, Vector3, Vector3>>();
        this.fractalParent = new GameObject("Fractal:" + this.axiom);
        this.fractalParent.transform.position = this.transform.position;
        this.fractalParent.transform.rotation = this.transform.rotation;
        this.fractalParent.transform.localScale = this.transform.localScale;
        this.productionActions = new Dictionary<char, Action>()
        {
            { 'F', DrawForward },
            { 'G', MoveForward },
            { 'X', Skip },
            { '-', TurnLeft },
            { '+', TurnRight },
            { '*', TurnUp },
            { '/', TurnDown },
            { '[', PushState },
            { ']', PopState }
        };
        switch (this.Seed)
        {
            case 0:
                SeedZero();
                break;
            case 1:
                SeedOne();
                break;
            case 2:
                SeedTwo();
                break;
            case 3:
                SeedThree();
                break;
            case 4:
                KochCurve();
                break;
            default:
                GenerateSerprenskiTriangle();
                break;
        }
        this.PrintSeed();
        this.GenerateFractalPatternString();
    }

    private void GenerateFractalPatternString()
    {
        if (!string.IsNullOrEmpty(this.HardCodedFractal))
        {
            this.fractalString = this.HardCodedFractal;
            return;
        }
        this.PrintFractalPattern();
        this.fractalString = this.axiom;
        StringBuilder generatedFractalPattern;
        for (int i = 0; i < this.Iterations; i++)
        {
            generatedFractalPattern = new StringBuilder();
            for (int c = 0; c < this.fractalString.Length; c++)
            {
                string ruleString = string.Empty;
                this.productionRules.TryGetValue(this.fractalString[c], out ruleString);
                if (!string.IsNullOrEmpty(ruleString))
                {
                    generatedFractalPattern.Append(ruleString);
                }
                else
                {
                    generatedFractalPattern.Append(this.fractalString[c].ToString());
                }
            }
            this.fractalString = generatedFractalPattern.ToString();

            this.PrintFractalPattern();
        }
    }

    private void PrintFractalPattern()
    {
        //Debug.Log("Fractal Pattern: " + this.fractalString);
    }

    public float GrowthAnimationFrameTimeInSeconds = 0.25f;
    private float secondsSinceLastGrowthAnimation = 0;
    private void Update()
    {
        if (this.productionRules == null || string.IsNullOrEmpty(this.axiom) || string.IsNullOrEmpty(this.fractalString))
        {
            return;
        }

        if (this.currentFractalStringIndex >= this.fractalString.Length)
        {
            return;
        }

        // Every x seconds, goto next iteration and interpret the string
        secondsSinceLastGrowthAnimation += Time.deltaTime;
        while(secondsSinceLastGrowthAnimation > GrowthAnimationFrameTimeInSeconds && this.currentFractalStringIndex < this.fractalString.Length)
        {
            secondsSinceLastGrowthAnimation -= GrowthAnimationFrameTimeInSeconds;

            // Apply a rule for the given character:
            Action productionAction = this.productionActions[this.fractalString[this.currentFractalStringIndex]];

            //Debug.Log("Current Character: " + this.fractalString[this.currentFractalStringIndex]);
            if (productionAction != null)
            {
                productionAction.Invoke();
            }

            // Increment the index of what has been drawn:
            this.currentFractalStringIndex++;
        }
    }
    private void SeedZero()
    {
        this.axiom = "F";
        this.productionRules = new Dictionary<char, string>()
        {
            { 'F', "F[-F][+F]" }
        };
    }
    private void SeedOne()
    {
        this.Iterations = 2;
        this.axiom = "F";
        this.productionRules = new Dictionary<char, string>()
        {
            { 'F', "F+FF-FF-F-F+F+FF-F-F+F+FF+FF-F" }
        };
    }
    private void SeedTwo()
    {
        this.Iterations = 4;
        this.axiom = "-F";
        this.productionRules = new Dictionary<char, string>()
        {
            { 'F', "F+F-F-F+F" }
        };
    }
    private void SeedThree()
    {
        this.Iterations = 4;
        this.axiom = "-F";
        this.productionRules = new Dictionary<char, string>()
        {
            { 'X', "[-FX][+FX][FX]" },
            { 'F', "FF" }
        };
    }
    private void GenerateSerprenskiTriangle()
    {
        this.axiom = "F-G-G";
        this.productionRules = new Dictionary<char, string>()
        {
            { 'F', "F-G+F+G-F" },
            { 'G', "GG" }
        };
    }
    private void gwefsdfsdf()
    {
        this.axiom = "X";
        this.productionRules = new Dictionary<char, string>
        {
            { 'X', "[F-[X+X]+F[+FX]-X]" },
            { 'F', "FF" }
        };
    }
    private void KochCurve()
    {
        this.Angle = 90.0f;
        this.axiom = "F";
        this.productionRules = new Dictionary<char, string>()
        {
            { 'F', "F+F-F-F+F" }
        };
    }
    private void PrintPosition()
    {
        //Debug.Log("Current Position: " + this.transform.position.ToString());
        //Debug.Log("Current Direction: " + this.currentDirection.ToString());
    }
    private void PrintSeed()
    {
        //Debug.Log("Axiom: " + this.axiom);
        //Debug.Log("Iterations: " + this.Iterations);
        foreach (KeyValuePair<char, string> rule in this.productionRules)
        {
            //Debug.Log("Rule: " + rule.Key + " -> " + rule.Value);
        }
    }

    private bool IsLeaf()
    {
        float iterationPercentage = ((float)this.currentFractalStringIndex / (float)this.fractalString.Length) * 100.0f; ;
        if (iterationPercentage < (100 - this.LeafPercentage))
        {
            return false;
        }
        return true;
    }
    private void MoveForward()
    {
        //transform.Translate(Vector3.up);
        //Debug.Log("MoveForward()");
        this.transform.localScale *= IterationScaleFactor + UnityEngine.Random.Range(-ScaleVariance, ScaleVariance);
        if (!IsLeaf())
        {
            Vector3 translation = new Vector3(
                this.currentDirection.x * this.BranchMeshSize.x / 2.0f * this.transform.localScale.x,
                this.currentDirection.y + this.BranchMeshSize.y / 2.0f * this.transform.localScale.y,
                this.currentDirection.z * this.BranchMeshSize.z / 2.0f * this.transform.localScale.z);
            this.transform.Translate(translation);
        } else
        {
            Vector3 translation = new Vector3(
                this.currentDirection.x * this.LeafMeshSize.x / 2.0f * this.transform.localScale.x,
                this.currentDirection.y + this.LeafMeshSize.y / 2.0f * this.transform.localScale.y / 2.0f,
                this.currentDirection.z * this.LeafMeshSize.z / 2.0f * this.transform.localScale.z);
            this.transform.Translate(translation);
        }
        //this.PrintPosition();
    }
    private void DrawForward()
    {
        //Debug.Log("DrawForward()");
        this.MoveForward();
        GameObject childFractal = new GameObject();
        float iterationPercentage = ((float)this.currentFractalStringIndex / (float)this.fractalString.Length) * 100.0f; ;
        if (!IsLeaf())
        {
            childFractal.AddComponent<MeshFilter>().mesh = BranchMesh;
            childFractal.AddComponent<MeshRenderer>().material = BranchMaterial;
            childFractal.transform.localScale = new Vector3(this.transform.localScale.x / 4.0f, this.transform.localScale.y * 1.5f, this.transform.localScale.z / 4.0f);
        }
        else
        {
            childFractal.AddComponent<MeshFilter>().mesh = LeafMesh;
            childFractal.AddComponent<MeshRenderer>().material = LeafMaterial;
            childFractal.transform.localScale = new Vector3(this.transform.localScale.x * 0.5f, this.transform.localScale.y * 0.5f, this.transform.localScale.z * 0.5f);
        }
        childFractal.transform.position = this.transform.position;
        childFractal.transform.rotation = this.transform.rotation;
        childFractal.transform.parent = this.fractalParent.transform;
    }
    private void TurnLeft()
    {
        //Debug.Log("TurnLeft()");
        this.transform.Rotate(Vector3.left * this.Angle * UnityEngine.Random.Range(-1.0f, 1.0f));
        this.transform.Rotate(Vector3.down * UnityEngine.Random.Range(-360.0f, 360.0f));
    }
    private void TurnRight()
    {
        //Debug.Log("TurnRight()");
        this.transform.Rotate(Vector3.right * this.Angle * UnityEngine.Random.Range(-1.0f, 1.0f));
        this.transform.Rotate(Vector3.up * UnityEngine.Random.Range(-360.0f, 360.0f));
    }
    private void TurnUp()
    {
        //Debug.Log("TurnUp()");
        this.transform.Rotate(Vector3.up * this.Angle);
    }
    private void TurnDown()
    {
        //Debug.Log("TurnDown()");
        this.transform.Rotate(Vector3.down * this.Angle);
    }
    private void Skip()
    {
        // Do Nothing
    }
    private void PushState()
    {
        //Debug.Log("PushState()");
        Tuple<Vector3, Vector3, Vector3, Vector3> state = new Tuple<Vector3, Vector3, Vector3, Vector3>(
            this.transform.position,
            this.transform.rotation.eulerAngles,
            this.currentDirection,
            this.transform.localScale
            );
        stateStack.Push(state);
    }
    private void PopState()
    {
        //Debug.Log("PopState()");
        Tuple<Vector3, Vector3, Vector3, Vector3> state = this.stateStack.Pop();
        this.transform.position = state.Item1;
        this.transform.rotation = Quaternion.Euler(state.Item2);
        this.currentDirection = state.Item3;
        this.transform.localScale = state.Item4;
    }
}