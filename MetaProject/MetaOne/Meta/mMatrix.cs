using System;

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

		public override bool Equals(object obj)
		{
			if (!(obj is mMatrix))
			{
				return false;
			}
			mMatrix mMatrix = (mMatrix)obj;
			return this.m00 == mMatrix.m00 && this.m01 == mMatrix.m01 && this.m02 == mMatrix.m02 && this.m03 == mMatrix.m03 && this.m10 == mMatrix.m10 && this.m11 == mMatrix.m11 && this.m12 == mMatrix.m12 && this.m13 == mMatrix.m13 && this.m20 == mMatrix.m20 && this.m21 == mMatrix.m21 && this.m22 == mMatrix.m22 && this.m23 == mMatrix.m23 && this.m30 == mMatrix.m30 && this.m31 == mMatrix.m31 && this.m32 == mMatrix.m32 && this.m33 == mMatrix.m33;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool IsNaN(mMatrix matrix_buffer)
		{
			bool flag = !float.IsNaN(matrix_buffer.m00) || float.IsNaN(matrix_buffer.m01) || float.IsNaN(matrix_buffer.m02) || float.IsNaN(matrix_buffer.m03) || float.IsNaN(matrix_buffer.m10) || float.IsNaN(matrix_buffer.m12) || float.IsNaN(matrix_buffer.m13) || float.IsNaN(matrix_buffer.m20) || float.IsNaN(matrix_buffer.m21) || float.IsNaN(matrix_buffer.m22) || float.IsNaN(matrix_buffer.m23) || float.IsNaN(matrix_buffer.m30) || float.IsNaN(matrix_buffer.m31) || float.IsNaN(matrix_buffer.m32) || float.IsNaN(matrix_buffer.m33);
			return !flag;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[",
				this.m00,
				",",
				this.m01,
				",",
				this.m02,
				",",
				this.m03,
				"]\n[",
				this.m10,
				",",
				this.m11,
				",",
				this.m12,
				",",
				this.m13,
				"]\n[",
				this.m20,
				",",
				this.m21,
				",",
				this.m22,
				",",
				this.m23,
				"]\n[",
				this.m30,
				",",
				this.m31,
				",",
				this.m32,
				",",
				this.m33,
				"]"
			});
		}

		public static bool operator ==(mMatrix m1, mMatrix m2)
		{
			return m1.Equals(m2);
		}

		public static bool operator !=(mMatrix m1, mMatrix m2)
		{
			return !m1.Equals(m2);
		}
	}
}
