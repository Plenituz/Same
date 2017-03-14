using UnityEngine;
using System.Collections;

public class Path {
	ArrayList points = new ArrayList ();//contiens tous les points 
	ArrayList actions = new ArrayList();//contiens les actions pour aller d'un point a l'autre
	//actions[0] est l'action pour aller de point 0 a point 1, 
	//action[1] est l'action pour aller de point 1 a point 2, etc
	float lenght = 0;

	public Path(Vector2 startPoint){
		points.Add (startPoint);
	}

	public void CubicTo(Vector2 firstHandle, Vector2 secondHandle, Vector2 endPoint){
		points.Add (endPoint);
		actions.Add (new CubicToAction (firstHandle, secondHandle));
	}

	public void rLineTo(Vector2 endPoint){
		points.Add (endPoint);
		actions.Add (new LineToAction ());
	}

	public void Seal(){
		for (int i = 0; i < actions.Count; i++) {
			lenght += ((Action)actions [i]).MeasureLenght ((Vector2) points [i], (Vector2) points [i + 1]);
		}
	}

	public Vector2 GetPointAtPercent(float percent){
		if (lenght == 0)
			Debug.Log ("path is not sealed");
		if (percent == 1.0f)
			percent = 0.9999f;//scotch
		float percentPx = lenght * percent;
		float at = 0f;
		int index = 0;
		for (int i = 0; i < actions.Count; i++) {
			at += ((Action)actions [i]).lenght;
			if (at > percentPx) {
				index = i;
				at -= ((Action)actions [i]).lenght;
				break;
			}
		}
		float localPercent = (percentPx - at) / ((Action)actions [index]).lenght;
		return ((Action)actions [index]).GetPointAtPercent (localPercent, (Vector2) points [index], (Vector2) points [index + 1]);
	}

    public static float partialBezierPoint(float t, float a, float b, float c, float d)
    {
        float C1 = ( d - (3.0f * c) + (3.0f * b) - a );
        float C2 = ( (3.0f * c) - (6.0f * b) + (3.0f * a) );
        float C3 = ( (3.0f * b) - (3.0f * a) );
        float C4 = ( a );

        return ( C1*t*t*t + C2*t*t + C3*t + C4  );
    }

    /**
     *
     * @param t from 0 to 1
     * @param p1 origin point
     * @param p2 first handle
     * @param p3 second handle
     * @param p4 arriving point
     * @return the point at t% of the curve defined by the points
     */
	public static Vector2 bezierPoint2d(float t, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4){
		Vector2 p = new Vector2();
        p.x = partialBezierPoint(t, p1.x, p2.x, p3.x, p4.x);
        p.y = partialBezierPoint(t, p1.y, p2.y, p3.y, p4.y);
        return p;
    }

    /**
     *
     * @param t from 0 to 1
     * @param p1 origin point int[]{x, y, z}
     * @param p2 first handle int[]{x, y, z}
     * @param p3 second handle int[]{x, y, z}
     * @param p4 arriving point int[]{x, y, z}
     * @return the point at t% of the curve defined by the points
     */
	public static Vector3 bezierPoint3d(float t, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4){
		Vector3 p = new Vector3();
        p.x = (int) partialBezierPoint(t, p1.x, p2.x, p3.x, p4.x);
        p.y = (int) partialBezierPoint(t, p1.y, p2.y, p3.y, p4.y);
        p.z = (int) partialBezierPoint(t, p1.z, p2.z, p3.z, p4.z);
        return p;
    }
}

public abstract class Action{
	public const int CUBIC_TO = 1;
	public const int LINE_TO = 2;

	public int name;
	/**
	 * has to be set in MeasureLenght
	 */
	public float lenght;
	/**
	 * Has to set the value of lenght
	 */
	abstract public float MeasureLenght (Vector2 origin, Vector2 end);
	abstract public Vector2 GetPointAtPercent (float percent, Vector2 origin, Vector2 end);
}

public class CubicToAction : Action{
	public Vector2 firstHandle;
	public Vector2 secondHandle;

	public CubicToAction(Vector2 firstHandle, Vector2 secondHandle){
		this.firstHandle = firstHandle;
		this.secondHandle = secondHandle;
		this.name = CUBIC_TO;
	}

	override public float MeasureLenght(Vector2 origin, Vector2 end){
		float r = 0f;
		float step = 0.01f;
		for (float i = step; i < 1.0f; i += step) {
			Vector2 v1 = Path.bezierPoint2d (i-step, origin, firstHandle, secondHandle, end);
			Vector2 v2 = Path.bezierPoint2d (i, origin, firstHandle, secondHandle, end);
			//Debug.DrawLine (v1, v2, Color.red, 10f, false);
			r += Vector2.Distance(v1, v2);
		}
		lenght = r;
		return r;
	}

	override public Vector2 GetPointAtPercent(float percent, Vector2 origin, Vector2 end){
		return Path.bezierPoint2d (percent, origin, firstHandle, secondHandle, end);
	}
}

public class LineToAction : Action{

	public LineToAction(){
		this.name = LINE_TO;
	}

	override public float MeasureLenght(Vector2 origin, Vector2 end){
		lenght = Vector2.Distance (origin, end);
		return lenght;
	}

	// public Vector2 GetPointAtPercent(float percent, Vector2 origin, Vector2 end){
	//	return new Vector2 (0f, 0f);
	//}
	override public Vector2 GetPointAtPercent(float percent, Vector2 origin, Vector2 end){
		ComplexNumber c = new ComplexNumber (end.x, end.y, ComplexNumber.NUMERICAL);
		ComplexNumber convert = new ComplexNumber (c.getModulus () * percent, c.getArgument (), ComplexNumber.GEOMETRICAL);
		Vector2 r = new Vector2 (convert.getRealPart (), convert.getImaginaryPart ());
		r += origin;
		return r;
	}
}