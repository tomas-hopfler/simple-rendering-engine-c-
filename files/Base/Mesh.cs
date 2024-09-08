using System;
using System.Numerics;

namespace ConsoleEngine
{
	public class Mesh
	{
		// edges calculated from faces
		public List<Vertex> Vertices { get; private set; }
		public List<Face> Faces { get; private set; }
		public Quaternion Rotation { get; private set; }
		public Vector3 Position => Center();

		public int Color = 0;

		public Mesh()
		{
			Vertices = new List<Vertex>();
			Faces = new List<Face>();
			Rotation = Quaternion.Identity;
			SetColor(0, 127, 127);
		}

		public void SetColor(int r, int g, int b)
		{
			foreach(Face face in Faces)
			{
				r = Math.Clamp(r, 0, 255);
				g = Math.Clamp(g, 0, 255);
				b = Math.Clamp(b, 0, 255);

				face.Color =  (r << 16) | (g << 8) | b;
			}
		}

		public void Rotate(float angleX, float angleY, float angleZ)
		{
			Quaternion rotation = CreateQuaternionFromEuler(angleX, angleY, angleZ);
			Rotation *= rotation;

			Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(rotation);
			Vector3 center = Position;

			for (int i = 0; i < Vertices.Count; i++)
			{
				Vector3 relativePos = Vertices[i].Position - center;
				relativePos = Vector3.Transform(relativePos, rotationMatrix);
				Vertices[i].Position = relativePos + center;
			}
		}

		public void RotateAround(Vector3 point, float angleX, float angleY, float angleZ)
		{
			Quaternion rotation = CreateQuaternionFromEuler(angleX, angleY, angleZ);
			Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(rotation);

			Rotation *= rotation;

			for (int i = 0; i < Vertices.Count; i++)
			{
				Vector3 relativePos = Vertices[i].Position - point;
				relativePos = Vector3.Transform(relativePos, rotationMatrix);
				Vertices[i].Position = relativePos + point;
			}
		}

		public void Scale(float X, float Y, float Z)
		{
			foreach(Vertex vertex in Vertices)
			{
				vertex.Scale(X,Y,Z);
			}
		}

		public void SetRotation(float angleX, float angleY, float angleZ)
		{
			Rotation = CreateQuaternionFromEuler(angleX, angleY, angleZ);
			Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(Rotation);
			Vector3 center = Position;

			for (int i = 0; i < Vertices.Count; i++)
			{
				Vector3 relativePos = Vertices[i].Position - center;
				relativePos = Vector3.Transform(relativePos, rotationMatrix);
				Vertices[i].Position = relativePos + center;
			}
		}

		public Vector3 GetEulerAngles()
		{
			return QuaternionToEuler(Rotation);
		}

		public void SetPosition(Vector3 newPosition)
		{
			Vector3 currentCenter = Center();
			Vector3 displacement = newPosition - currentCenter;

			foreach (var vertex in Vertices)
			{
				vertex.Position += displacement;
			}
		}

		public void Move(float dx, float dy, float dz)
		{
			Vector3 movement = new Vector3(dx, dy, dz);

			Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(Rotation);
			Vector3 rotatedMovement = Vector3.Transform(movement, rotationMatrix);

			foreach (var vertex in Vertices)
			{
				vertex.Position += rotatedMovement;
			}
		}

		public Vector3 Center()
		{
			if (Vertices.Count == 0)
			{
				return Vector3.Zero;
			}

			Vector3 sum = Vector3.Zero;
			foreach (var vertex in Vertices)
			{
				sum += vertex.Position;
			}
			return sum / Vertices.Count;
		}

		private Quaternion CreateQuaternionFromEuler(float angleX, float angleY, float angleZ)
		{
			float radX = DegreesToRadians(angleX);
			float radY = DegreesToRadians(angleY);
			float radZ = DegreesToRadians(angleZ);

			return Quaternion.CreateFromYawPitchRoll(radY, radX, radZ);
		}

		public void RotateVertexLocal(int vertexIndex, Vector3 angles)
		{
			if (vertexIndex < 0 || vertexIndex >= Vertices.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(vertexIndex), "Vertex index is out of range.");
			}

			Vertex vertex = Vertices[vertexIndex];

			Quaternion rotationX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, DegreesToRadians(angles.X));
			Quaternion rotationY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, DegreesToRadians(angles.Y));
			Quaternion rotationZ = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, DegreesToRadians(angles.Z));

			Quaternion totalRotation = rotationX * rotationY * rotationZ;

			Vector3 localPosition = vertex.Position - Position;

			localPosition = Vector3.Transform(localPosition, totalRotation);

			vertex.Position = localPosition + Position;
		}

		public Vector3 QuaternionToEuler(Quaternion q)
		{
			Vector3 angles;

			double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
			double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
			angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

			double sinp = 2 * (q.W * q.Y - q.Z * q.X);

			if (Math.Abs(sinp) >= 1)
			{
				angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
			}
			else
			{
				angles.Y = (float)Math.Asin(sinp);
			}

			double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
			double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
			angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

			return new Vector3(
				RadiansToDegrees(angles.X),
				RadiansToDegrees(angles.Y),
				RadiansToDegrees(angles.Z) );
		}

		public float DegreesToRadians(float degrees)
		{
			return degrees * (float)(Math.PI / 180.0);
		}

		public float RadiansToDegrees(float radians)
		{
			return radians * (float)(180.0 / Math.PI);
		}

		public List<Edge> GetEdges() // get edges from faces
		{
			List<Edge> allEdges = new List<Edge>();

			foreach (Face face in Faces)
			{
				allEdges.AddRange(face.GetEdges());
			}

			return allEdges;
		}

		public void Simplify(float targetPercentage) // to lower face count based on percentage
		{
			int targetFaceCount = (int)(Faces.Count * targetPercentage);

			while (Faces.Count > targetFaceCount)
			{
				float shortestEdgeLength = float.MaxValue;
				Edge shortestEdge = null;

				foreach (Edge edge in GetEdges())
				{
					float length = Vector3.Distance(edge.Start.Position, edge.End.Position);
					if (length < shortestEdgeLength)
					{
						shortestEdgeLength = length;
						shortestEdge = edge;
					}
				}

				if (shortestEdge == null)
					break;

				Vector3 midpoint = (shortestEdge.Start.Position + shortestEdge.End.Position) / 2;
				shortestEdge.Start.Position = midpoint;

				for (int i = Faces.Count - 1; i >= 0; i--)
				{
					Face face = Faces[i];
					if (face.Vertices.Contains(shortestEdge.End))
					{
		
						for (int j = 0; j < face.Vertices.Count; j++)
						{
							if (face.Vertices[j] == shortestEdge.End)
							{
								face.Vertices[j] = shortestEdge.Start;
							}
						}

						if (face.Vertices.Distinct().Count() < 3)
						{
							Faces.RemoveAt(i);
						}
					}
				}

				Vertices.Remove(shortestEdge.End);
			}
		}

		public Prism ComputeBoundingBox() // bounding box for simplified collision detection
		{
			if (Vertices.Count == 0)
			{
				return new Prism(0, 0, 0);
			}

			Vector3 min = Vertices[0].Position;
			Vector3 max = Vertices[0].Position;

			for (int i = 1; i < Vertices.Count; i++)
			{
				Vector3 position = Vertices[i].Position;
				min = Vector3.Min(min, position);
				max = Vector3.Max(max, position);
			}

			float width = max.X - min.X;
			float height = max.Y - min.Y;
			float depth = max.Z - min.Z;

			Vector3 center = (min + max) / 2;

			return new Prism(width, height, depth, center.X, center.Y, center.Z);
		}
	}
}

