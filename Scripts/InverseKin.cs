using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class IK
{ 

  
    public static Vector3 ReverseY(Vector3 inputVector)
    {
        return new Vector3(inputVector.x, -inputVector.y, inputVector.z);
    }

    public static Quaternion LeftToRightQuaternion(Quaternion leftHandQuat)
    {
        //Negate the x, y, and z values of the quaternion to convert it to a right-handed quaternion
        Quaternion rightHandQuat = new Quaternion(leftHandQuat.x, -leftHandQuat.y, leftHandQuat.z, -leftHandQuat.w);
        return rightHandQuat;
    }    

    public static Matrix4x4 hdh(float th, float d, float a, float al)
    {
        // Calculate the cosine of THETA
        float ct = Mathf.Cos(th);
        // Calculate the sine of THETA
        float st = Mathf.Sin(th);

        // Create the rotation matrix around the z-axis
        Matrix4x4 rz = new Matrix4x4();
        rz.SetRow(0, new Vector4(ct, -st, 0, 0));
        rz.SetRow(1, new Vector4(st, ct, 0, 0));
        rz.SetRow(2, new Vector4(0, 0, 1, 0));
        rz.SetRow(3, new Vector4(0, 0, 0, 1));

        // Create the translation matrix along the z-axis and x-axis
        Matrix4x4 trans = new Matrix4x4();
        trans.SetRow(0, new Vector4(1, 0, 0, a));
        trans.SetRow(1, new Vector4(0, 1, 0, 0));
        trans.SetRow(2, new Vector4(0, 0, 1, d));
        trans.SetRow(3, new Vector4(0, 0, 0, 1));

        // Calculate the cosine of ALPHA
        float cal = Mathf.Cos(al);
        // Calculate the sine of ALPHA
        float sal = Mathf.Sin(al);

        // Create the rotation matrix around the x-axis
        Matrix4x4 rx = new Matrix4x4();
        rx.SetRow(0, new Vector4(1, 0, 0, 0));
        rx.SetRow(1, new Vector4(0, cal, -sal, 0));
        rx.SetRow(2, new Vector4(0, sal, cal, 0));
        rx.SetRow(3, new Vector4(0, 0, 0, 1));

        // Calculate the homogeneous transformation matrix
        return rz * trans * rx;
    }

    public static Matrix4x4[] dirkinA(float[] q)
    {
        // Parameters of the UR5 robot
        float d1 = 0.163f;
        float d2 = 0.138f;
        float d3 = -0.131f;
        float d4 = 0.127f;
        float d5 = 0.1f;
        float d6 = 0.1f;
        float a2 = -0.425f;
        float a3 = -0.392f;

            // Define matrices A (6 degrees of freedom)
        Matrix4x4[] A = new Matrix4x4[6];
        A[0] = hdh(q[0]*Mathf.PI/180, d1, 0,  Mathf.PI / 2);
        A[1] = hdh(q[1]*Mathf.PI/180, d2, a2, 0);
        A[2] = hdh(q[2]*Mathf.PI/180, d3, a3, 0);
        A[3] = hdh(q[3]*Mathf.PI/180, d4, 0,  Mathf.PI / 2);
        A[4] = hdh(q[4]*Mathf.PI/180, d5, 0, -Mathf.PI / 2);
        A[5] = hdh(q[5]*Mathf.PI/180, d6, 0, 0);

        return A;
    }

    public static Matrix4x4 dirkinT6(float[] q)
    {
        // A[0] ... A[5] - List of matrices A (input).
        // T6 - Matrix of the robot's endpoint position (output).
        Matrix4x4[] A = dirkinA(q);
        Matrix4x4 T6 = A[0] * A[1] * A[2] * A[3] * A[4] * A[5];

        return T6;
    }


    public static float[,] jacobi0(float[] q)
    {
        // q - Joint angles of the robot (input).
        // J - Geometric Jacobian matrix (output).

        // Calculate transformation matrices A.
        Matrix4x4[] A = dirkinA(q);

        // Transformation matrix of endpoint T5.
        Matrix4x4 T6 = dirkinT6(q);

        // Initialize empty Jacobian matrix.
        float[,] J = new float[6, 6];

        // Initialize variables.
        float[] z_0 = { 0, 0, 1 };     // Vector in direction of z_0 axis.
        float[] p_0 = { 0, 0, 0 };     // Vector of the position of the first joint in the reference frame.
        Matrix4x4 Tn = Matrix4x4.identity;    // Initial value of the partial matrix Tn; n = 1,2...

        // 1st COLUMN OF THE JACOBIAN SUBMATRIX
        // Calculate 1st column of the position Jacobian submatrix.
        float[] col0 = new float[3];
        col0 = Vector3.Cross(z_0.ToUnityVector3(), T6.GetColumn(3).ToUnityVector3() - p_0.ToUnityVector3()).ToFloatArray();
        for (int i = 0; i < 3; i++)
        {
            J[i, 0] = col0[i];
        }
        // Calculate 1st column of the rotation Jacobian submatrix.
        for (int i = 0; i < 3; i++)
        {
            J[i + 3, 0] = z_0[i];
        }

        // 2nd TO 6th COLUMNS OF THE JACOBIAN SUBMATRIX
        for (int j = 1; j < 6; j++)
        {
            // Calculate the matrix Tn, which contains information about Zj-1 and Pj-1.
            Tn = Tn * A[j - 1];

            // Calculate the j-th column of the position Jacobian submatrix.
            float[] col = new float[3];
            col = Vector3.Cross(Tn.GetColumn(2).ToUnityVector3(), T6.GetColumn(3).ToUnityVector3() - Tn.GetColumn(3).ToUnityVector3()).ToFloatArray();
            for (int i = 0; i < 3; i++)
            {
                J[i, j] = col[i];
            }

            // Calculate the j-th column of the rotation Jacobian submatrix.
            float[] z_j = new float[3];
            z_j = Tn.GetColumn(2).ToFloatArray();
            for (int i = 0; i < 3; i++)
            {
                J[i + 3, j] = z_j[i];
            }
        }

        return J;
    }

    public static float[] calculate_new_q(float[] q, float[] dv)
    {

        float[,] jacobian = IK.jacobi0(q);
        float[,] transJ = IK.Transpose(jacobian);
        float k = 0.1f;
        float[,] tmp66 = IK.MultiplyScalarToMatrix(k*k, IK.IdentityMatrix6x6());
        float[,] J_x_transJ = IK.MultiplyMatrices(jacobian,transJ);
        float[,] tmp66_2 = IK.MatrixSum(J_x_transJ, tmp66);
        tmp66 = IK.InverseMatrix(tmp66_2);
        float [,] invJ = IK.MultiplyMatrices(transJ,tmp66);

        float eps_a = 0.01f;
      
        float [,] invJ_x_J = IK.MultiplyMatrices(invJ,jacobian);
        float [,] I_m_invJ_x_J = IK.SubtractMatrices(IK.IdentityMatrix6x6(),invJ_x_J);
        float [] tmp6 = IK.MultiplyScalarVector(eps_a, IK.ElbowUp(q));
        float [] tmp6_2 = IK.MatrixTimesVector(I_m_invJ_x_J, tmp6);

        float [] dq = IK.MinusOfVectors(IK.MatrixTimesVector(invJ,dv), tmp6_2);

        float [] new_q = IK.SumOfVectors(dq, q);

        return new_q;
    }


    public static float[] ToFloatArray(this Vector3 vector)
    {
        return new float[] { vector.x, vector.y, vector.z };
    }

    public static float[] ToFloatArray(this Vector4 vector)
    {
        return new float[] { vector.x, vector.y, vector.z };
    }    

    public static Vector3 ToUnityVector3(this float[] values)
    {
        if (values == null || values.Length != 3)
        {
            throw new System.ArgumentException("Input array must contain exactly 3 elements.");
        }
        return new Vector3(values[0], values[1], values[2]);
    }

    public static Vector3 ToUnityVector3(this Vector4 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }    
   
    public static float[] ElbowUp(float[] q)
    {
        float [] q_new = { 0.0f, 0.0f, q[2]-90.0f, 0.0f, 0.0f, 0.0f };
        return q_new;

    } 

    public static float[,] MatrixSum(float[,] a, float[,] b)
    {
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);

        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = a[i, j] + b[i, j];
            }
        }

        return result;
    }

    public static float[] SumOfVectors(float[] dq, float[] q)
    {

        float[] q_new = new float[6];
        // Sum the values of both input vectors and store them in the result float[]
        for (int i = 0; i < 6; i++)
        {
            q_new[i] = dq[i] + q[i];
        }

        return q_new;
    }  

    public static float[] MinusOfVectors(float[] dq, float[] q)
    {

        float[] q_new = new float[6];
        // Sum the values of both input vectors and store them in the result float[]
        for (int i = 0; i < 6; i++)
        {
            q_new[i] = dq[i] - q[i];
        }

        return q_new;
    }     

    public static float[] MultiplyScalarVector(float scalar, float[] vector)
    {
        float[] result = new float[6];

        for (int i = 0; i < 6; i++)
        {
            result[i] = scalar * vector[i];
        }

        return result;
    }

    public static float[] UpdateVector(float[] vector)
    {
        float[] result = new float[6];

        for (int i = 0; i < 6; i++)
        {
            result[i] = vector[i];
        }

        return result;
    }

    public static float[,] IdentityMatrix6x6()
    {
        float[,] I = new float[6, 6];

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                I[i, j] = (i == j) ? 1 : 0;
            }
        }

        return I;
    }

    public static float[] MatrixTimesVector(float[,] matrix, float[] vector)
    {
        if (matrix.GetLength(0) != 6 || matrix.GetLength(1) != 6 || vector.Length != 6)
        {
            throw new ArgumentException("Matrix and vector must be of size 6.");
        }
        
        float[] result = new float[6];
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }
        return result;
    }

    public static float[,] MultiplyMatrices(float[,] a, float[,] b)
    {
        // Initialize the result matrix.
        float[,] result = new float[6, 6];

        // Loop through each row of the first matrix.
        for (int i = 0; i < 6; i++)
        {
            // Loop through each column of the second matrix.
            for (int j = 0; j < 6; j++)
            {
                // Initialize the sum for this element.
                float sum = 0;

                // Loop through each element in the row of the first matrix
                // and the column of the second matrix, and multiply them.
                for (int k = 0; k < 6; k++)
                {
                    sum += a[i, k] * b[k, j];
                }

                // Assign the sum to the corresponding element in the result matrix.
                result[i, j] = sum;
            }
        }

        // Return the result matrix.
        return result;
    }


    public static float[,] SubtractMatrices(float[,] a, float[,] b)
    {
        if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
        {
            throw new System.ArgumentException("Matrices must have the same dimensions.");
        }

        int rows = a.GetLength(0);
        int cols = a.GetLength(1);

        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = a[i, j] - b[i, j];
            }
        }

        return result;
    }

    public static float[] Vector3ToFloatArray(Vector3 v1, Vector3 v2)
    {
        float[] result = new float[6];
        result[0] = v1.x;
        result[1] = v1.y;
        result[2] = v1.z;
        result[3] = v2.x;
        result[4] = v2.y;
        result[5] = v2.z;
        return result;
    }

    public static float[,] InverseMatrix(float[,] a)
    {
        // Create an augmented matrix combining 'a' with the identity matrix.
        int n = a.GetLength(0);
        float[,] augmented = new float[n, 2 * n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                augmented[i, j] = a[i, j];
            }
            augmented[i, i + n] = 1.0f;
        }

        // Perform Gaussian elimination to transform the left half of the augmented matrix into the identity matrix.
        for (int i = 0; i < n; i++)
        {
            // Find the pivot row (the row with the largest absolute value in the current column).
            int pivotRow = i;
            float pivotValue = Mathf.Abs(augmented[i, i]);
            for (int j = i + 1; j < n; j++)
            {
                float absValue = Mathf.Abs(augmented[j, i]);
                if (absValue > pivotValue)
                {
                    pivotRow = j;
                    pivotValue = absValue;
                }
            }

            // Swap the pivot row with the current row if necessary.
            if (pivotRow != i)
            {
                for (int j = 0; j < 2 * n; j++)
                {
                    float temp = augmented[i, j];
                    augmented[i, j] = augmented[pivotRow, j];
                    augmented[pivotRow, j] = temp;
                }
            }

            // Scale the current row so that the pivot element becomes 1.
            float scale = augmented[i, i];
            for (int j = i; j < 2 * n; j++)
            {
                augmented[i, j] /= scale;
            }

            // Eliminate the pivot column from all other rows.
            for (int j = 0; j < n; j++)
            {
                if (j != i)
                {
                    float factor = augmented[j, i];
                    for (int k = i; k < 2 * n; k++)
                    {
                        augmented[j, k] -= factor * augmented[i, k];
                    }
                }
            }
        }

        // Extract the right half of the augmented matrix (the inverse of 'a').
        float[,] inverse = new float[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                inverse[i, j] = augmented[i, j + n];
            }
        }

        return inverse;
    }

    public static float[,] MultiplyScalarToMatrix(float scalar, float[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int columns = matrix.GetLength(1);
        float[,] result = new float[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                result[i, j] = scalar * matrix[i, j];
            }
        }

        return result;
    }


    public static float[,] Transpose(float[,] a)
    {
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);
        float[,] result = new float[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[j, i] = a[i, j];
            }
        }

        return result;
    }

}


public class InverseKin : MonoBehaviour
{

    private GameObject Base;
	private GameObject J1;
    private GameObject J2;
    private GameObject J3;
    private GameObject J4;
    private GameObject J5;
    private GameObject J6;
    private GameObject TCP;
    private GameObject sphere;

    private float[] q;   
    private float[] qPr;

    public GameObject target;
    public float velTmax = 250.0f;
    public float velOmax = 90.0f;
    public float KT = 1.0f;
    public float KO = 100.0f;

    // Start is called before the first frame update
    void Start()
    {

		Base = GameObject.Find("Base");
        J1 = GameObject.Find("Joint1");
        J2 = GameObject.Find("Joint2");
        J3 = GameObject.Find("Joint3");
        J4 = GameObject.Find("Joint4");
        J5 = GameObject.Find("Joint5");
        J6 = GameObject.Find("Joint6");    
        TCP  = GameObject.Find("TCP");    

        sphere  = GameObject.Find("Sphere");

        //q = new float[] {q1_start, q2_start, q3_start, q4_start, q5_start, q6_start};
        q = new float[] {0.0f, -60.0f, 120.0f, 120.0f, -90.0f, -90.0f};
        //qPr = new float[] {0.0f, 0.0f, 90.0f, 0.0f, 0.0f, 0.0f};
        //q = new float[] {97.45f, -89.42f, 97.98f, -113.08f, -86.42f, 9.18f};
        //q = new float[] {21.66f, -85.7f, 117.25f, 327.58f, -24.47f, -177.28f};

        J1.transform.localEulerAngles = new Vector3(0, 0, -q[0]);
        J2.transform.localEulerAngles = new Vector3(-90, 0, -q[1]);
        J3.transform.localEulerAngles = new Vector3(0, 0, -q[2]);
        J4.transform.localEulerAngles = new Vector3(0, 0, -q[3]);
        J5.transform.localEulerAngles = new Vector3(-90, 0, -q[4]);
        J6.transform.localEulerAngles = new Vector3(90, 0, -q[5]);  
        
        Matrix4x4 T6 = IK.dirkinT6(q); 
        sphere.transform.localRotation = IK.LeftToRightQuaternion(T6.rotation);
        sphere.transform.localPosition = IK.ReverseY(T6.GetColumn(3));                
        
    }

    // Update is called once per frame
    void Update()
    {

        //close the program on escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


        // Move red sphere into TCP
        Matrix4x4 T6 = IK.dirkinT6(q);
        sphere.transform.localRotation = IK.LeftToRightQuaternion(T6.rotation);
        sphere.transform.localPosition = IK.ReverseY(T6.GetColumn(3));

        // Transform target into base frame
        Transform TargetToBase = GameObjectToLocalTransform(Base, target);

        // Calculate translational velocity
        Vector3 direction = TargetToBase.localPosition - sphere.transform.localPosition;
        Vector3 velTrans = IK.ReverseY(Vector3.ClampMagnitude(KT*direction, velTmax * Time.deltaTime));
        //Vector3 velTrans = ReverseY(KT*direction);

        // Calculate rotational velocity
        Quaternion diffOri = Quaternion.Slerp(sphere.transform.localRotation, TargetToBase.localRotation, 0.1f)*Quaternion.Inverse(sphere.transform.localRotation);
        //Debug.Log(diffOri);
        Vector3 velOri = new Vector3(diffOri.x, -diffOri.y, diffOri.z);
        velOri = Vector3.ClampMagnitude(-KO*velOri, velOmax * Time.deltaTime);

        float [] dv = IK.Vector3ToFloatArray(velTrans, velOri);

        float[] new_q = IK.calculate_new_q(q, dv);

        J1.transform.localEulerAngles = new Vector3(0, 0, -new_q[0]);
        J2.transform.localEulerAngles = new Vector3(-90, 0, -new_q[1]);
        J3.transform.localEulerAngles = new Vector3(0, 0, -new_q[2]);
        J4.transform.localEulerAngles = new Vector3(0, 0, -new_q[3]);
        J5.transform.localEulerAngles = new Vector3(-90, 0, -new_q[4]);
        J6.transform.localEulerAngles = new Vector3(90, 0, -new_q[5]);         

        q = IK.UpdateVector(new_q);

    }

    
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
		
        for (int i = 0; i < 6; i++)
        {
            GUI.Label(new Rect(10, i * 30, 0, 0), "q" + (i + 1) + "= " + System.Math.Round(q[i], 2) + "°", style);
        }
    }   

    public Transform GameObjectToLocalTransform(GameObject Base, GameObject target)
    {
        // Create a new GameObject
        GameObject localTransformObject = new GameObject();
        // Get its transform component
        Transform localTransform = localTransformObject.transform;
        // Set its position to the local space position of target in the coordinate frame of Base
        localTransform.position = Base.transform.InverseTransformPoint(target.transform.position);
        // Set its rotation to the local space rotation of target in the coordinate frame of Base
        localTransform.rotation = Quaternion.Inverse(Base.transform.rotation) * target.transform.rotation;
        Destroy(localTransformObject);
        return localTransform;
    }      
    
    


}





