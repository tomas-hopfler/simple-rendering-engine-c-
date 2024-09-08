using System.Numerics;

namespace ConsoleEngine
{
	public class Vertex
	{
		// rotation moved to face
		public Vector3 Position { get; set; }

		public Vertex(float X = 0, float Y = 0, float Z = 0)
		{
			Position = new Vector3(X, Y, Z);
		}

		public Vertex(Vector3 position)
		{
			Position = position;
		}

		public void Move(float X, float Y, float Z)
		{
			Vector3 movement = new Vector3(X, Y, Z);
			Position += movement;
		}

		public void Scale(float X, float Y, float Z)
		{
			Position = new Vector3(Position.X * X, Position.Y * Y, Position.Z * Z);
		}

		public void RotateAround(Vector3 center, float angleX, float angleY, float angleZ)
		{
			Vector3 relativePos = Position - center;
			Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(
				DegreesToRadians(angleY),
				DegreesToRadians(angleX),
				DegreesToRadians(angleZ));
			relativePos = Vector3.Transform(relativePos, rotation);
			Position = relativePos + center;
		}

		public void RotateAround(Vector3 center, Vector3 angle)
		{
			Vector3 relativePos = Position - center;
			Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(
				DegreesToRadians(angle.Y),
				DegreesToRadians(angle.X),
				DegreesToRadians(angle.Z));
			relativePos = Vector3.Transform(relativePos, rotation);
			Position = relativePos + center;
		}

		public void RotateAround(float X, float Y, float Z, float angleX, float angleY, float angleZ)
		{
			Vector3 center = new Vector3(X, Y, Z);
			Vector3 relativePos = Position - center;
			Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(
				DegreesToRadians(angleY),
				DegreesToRadians(angleX),
				DegreesToRadians(angleZ));
			relativePos = Vector3.Transform(relativePos, rotation);
			Position = relativePos + center;
		}

		private float DegreesToRadians(float degrees)
		{
			return degrees * (float)(Math.PI / 180.0);
		}

		public static float Distance(Vertex point1, Vertex point2)
		{
			return Vector3.Distance(point1.Position, point2.Position);
		}

		public static Vertex operator -(Vertex a, Vertex b)
		{
			return new Vertex(a.Position - b.Position);
		}

		public static Vertex operator +(Vertex a, Vertex b)
		{
			return new Vertex(a.Position + b.Position);
		}

		public static Vertex operator *(Vertex v, float scalar)
		{
			return new Vertex(v.Position * scalar);
		}

		public Vertex Normalize()
		{
			return new Vertex(Vector3.Normalize(Position));
		}

		public float Length()
		{
			return Position.Length();
		}

		public Vertex Cross(Vertex other)
		{
			return new Vertex(Vector3.Cross(Position, other.Position));
		}
	}
}
