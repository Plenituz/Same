using UnityEngine;
using System.Collections;

public class ComplexNumber{
	
	public static bool NUMERICAL = true;
	public static bool GEOMETRICAL = false;

	private float realPart;
	private float imaginaryPart;
	private float modulus;
	/**
     * in radians
     */
	private float argument;

	/**
     * initialisation as a+ib or a*exp(ib)
     * @param a real part(if NUMERICAL) or modulus(if GEOMETRICAL)
     * @param b imaginary part(if NUMERICAL) or argument(if(GEOMETRICAL)
     * @param type NUMERICAL or GEOMETRICAL
     */
	public ComplexNumber(float a, float b, bool type){
		if(type == NUMERICAL){
			realPart = a;
			imaginaryPart = b;
			fillComponents(GEOMETRICAL);
		}else if(type == GEOMETRICAL){
			this.modulus = a;
			this.argument = b;
			fillComponents(NUMERICAL);
		}

	}

	private void fillComponents(bool what) {
		if (what == NUMERICAL){
			argument %= Mathf.PI*2;
			realPart = modulus/Mathf.Sqrt(1 + Mathf.Pow(Mathf.Tan(argument), 2));
			imaginaryPart = (modulus * Mathf.Tan(argument))/Mathf.Sqrt(1 + Mathf.Pow(Mathf.Tan(argument), 2));
			if(argument > Mathf.PI/2 && argument <= Mathf.PI + (Mathf.PI/2)){
				realPart *= -1;
				imaginaryPart*= -1;
			}
			if (argument == Mathf.PI / 2 || argument == Mathf.PI + (Mathf.PI / 2) || argument == -(Mathf.PI/2)) {//scotch mathematique
				imaginaryPart *= -1;
			}
		}else if(what == GEOMETRICAL){
			modulus = Mathf.Sqrt(Mathf.Pow(realPart, 2) + Mathf.Pow(imaginaryPart, 2));
			argument = Mathf.Atan2(imaginaryPart, realPart);
			if(argument < 0){
				argument += Mathf.PI*2;
			}
			if(argument > Mathf.PI*2){
				argument -= Mathf.PI*2;
			}
		}

	}

	public float getRealPart() {
		return realPart;
	}

	public float getImaginaryPart() {
		return imaginaryPart;
	}

	public float getModulus() {
		return modulus;
	}

	/**
     *
     * @return argument in radians
     */
	public float getArgument() {
		return argument;
	}

	public Vector2 toVector(){
		return new Vector2 (realPart, imaginaryPart);
	}


	/**
     *
     * @param anchor point to rotate arround
     * @param pointToRotate point which is getting rotated arround anchor
     * @param angle in radian
     * @return the new coordonates of pointToRotate
     */
	public static Vector2 rotate(Vector2 anchor, Vector2 pointToRotate, float angle){
		ComplexNumber vector = new ComplexNumber(pointToRotate.x - anchor.x, pointToRotate.y - anchor.y, ComplexNumber.NUMERICAL);
		ComplexNumber convers = new ComplexNumber(vector.getModulus(), vector.getArgument() + angle, ComplexNumber.GEOMETRICAL);
		return new Vector2(convers.getRealPart() + anchor.x, convers.getImaginaryPart() + anchor.y);
	}

	/**
     * same as get point on line at distance but better
     * @param origin
     * @param direction
     * @param dist
     * @return
     */
	public static Vector2/*double[]*/ getPointOnVector(Vector2 origin, Vector2 direction, float dist){
		ComplexNumber vector = new ComplexNumber(direction.x, direction.y, ComplexNumber.NUMERICAL);
		ComplexNumber convers = new ComplexNumber(dist, vector.getArgument(), ComplexNumber.GEOMETRICAL);
		return new Vector2(convers.getRealPart() + origin.x, convers.getImaginaryPart() + origin.y);
		//return new double[]{convers.getRealPart() + origin.x, convers.getImaginaryPart() + origin.y};
	}

}
