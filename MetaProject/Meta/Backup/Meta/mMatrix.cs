// Decompiled with JetBrains decompiler
// Type: Meta.mMatrix
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

namespace Meta
{
  internal struct mMatrix
  {
    public float m00;
    public float m01;
    public float m02;
    public float m03;
    public float m10;
    public float m11;
    public float m12;
    public float m13;
    public float m20;
    public float m21;
    public float m22;
    public float m23;
    public float m30;
    public float m31;
    public float m32;
    public float m33;

    public static bool operator ==(mMatrix m1, mMatrix m2)
    {
      return m1.Equals((object) m2);
    }

    public static bool operator !=(mMatrix m1, mMatrix m2)
    {
      return !m1.Equals((object) m2);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is mMatrix))
        return false;
      mMatrix mMatrix = (mMatrix) obj;
      if ((double) this.m00 == (double) mMatrix.m00 && (double) this.m01 == (double) mMatrix.m01 && ((double) this.m02 == (double) mMatrix.m02 && (double) this.m03 == (double) mMatrix.m03) && ((double) this.m10 == (double) mMatrix.m10 && (double) this.m11 == (double) mMatrix.m11 && ((double) this.m12 == (double) mMatrix.m12 && (double) this.m13 == (double) mMatrix.m13)) && ((double) this.m20 == (double) mMatrix.m20 && (double) this.m21 == (double) mMatrix.m21 && ((double) this.m22 == (double) mMatrix.m22 && (double) this.m23 == (double) mMatrix.m23) && ((double) this.m30 == (double) mMatrix.m30 && (double) this.m31 == (double) mMatrix.m31 && (double) this.m32 == (double) mMatrix.m32)))
        return (double) this.m33 == (double) mMatrix.m33;
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public static bool IsNaN(mMatrix matrix_buffer)
    {
      return float.IsNaN(matrix_buffer.m00) && !float.IsNaN(matrix_buffer.m01) && (!float.IsNaN(matrix_buffer.m02) && !float.IsNaN(matrix_buffer.m03)) && (!float.IsNaN(matrix_buffer.m10) && !float.IsNaN(matrix_buffer.m12) && (!float.IsNaN(matrix_buffer.m13) && !float.IsNaN(matrix_buffer.m20))) && (!float.IsNaN(matrix_buffer.m21) && !float.IsNaN(matrix_buffer.m22) && (!float.IsNaN(matrix_buffer.m23) && !float.IsNaN(matrix_buffer.m30)) && (!float.IsNaN(matrix_buffer.m31) && !float.IsNaN(matrix_buffer.m32))) && !float.IsNaN(matrix_buffer.m33);
    }

    public override string ToString()
    {
      return "[" + (object) this.m00 + "," + (string) (object) this.m01 + "," + (string) (object) this.m02 + "," + (string) (object) this.m03 + "]\n[" + (string) (object) this.m10 + "," + (string) (object) this.m11 + "," + (string) (object) this.m12 + "," + (string) (object) this.m13 + "]\n[" + (string) (object) this.m20 + "," + (string) (object) this.m21 + "," + (string) (object) this.m22 + "," + (string) (object) this.m23 + "]\n[" + (string) (object) this.m30 + "," + (string) (object) this.m31 + "," + (string) (object) this.m32 + "," + (string) (object) this.m33 + "]";
    }
  }
}
