using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inheritance : MonoBehaviour
{
    NarseseParser.TruthValue _truthValue;
    Concept _subject, _predicate;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(NarseseParser.TruthValue truthVal, Concept subject, Concept predicate)
    {
        SetTruthValue(truthVal);
        _subject = subject;
        _predicate = predicate;

        DrawInheritance();
    }

    public NarseseParser.TruthValue GetTruthValue()
    {
        return _truthValue;
    }

    public void SetTruthValue(NarseseParser.TruthValue truthVal)
    {
        _truthValue = truthVal;
    }

    public bool ConnectsCompoundTerm()
    {
        return _subject.isCompoundTerm() || _predicate.isCompoundTerm();
    }

    private void DrawInheritance()
    {
        Vector3[] positions = new Vector3[2];
        positions[0] = _subject.transform.position;
        positions[1] = _predicate.transform.position;
        this.gameObject.GetComponent<LineRenderer>().SetPositions(positions);

        Color startColor = this.gameObject.GetComponent<LineRenderer>().startColor;
        Color endColor = this.gameObject.GetComponent<LineRenderer>().endColor;
        startColor = new Color(startColor.r, startColor.g, startColor.b, System.Single.Parse(GetTruthValue()._confidence));
        endColor = new Color(endColor.r, endColor.g, endColor.b, System.Single.Parse(GetTruthValue()._confidence));
        this.gameObject.GetComponent<LineRenderer>().startColor = startColor;
        this.gameObject.GetComponent<LineRenderer>().endColor = endColor;
    }

}
