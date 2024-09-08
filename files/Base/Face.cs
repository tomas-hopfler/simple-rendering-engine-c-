using System.Numerics;

namespace ConsoleEngine
{
	public class Face
	{
		public List<Vertex> Vertices { get; private set; }
		public Quaternion Rotation { get; private set; }
		public Vector3 Position => Center();

		public int Color = 0;

		public Face()
		{
			Vertices = new List<Vertex>();
			Rotation = Quaternion.Identity;
		}
		public Face(List<Vertex> vertices)
		{
			Vertices = vertices;
			Rotation = Quaternion.Identity;
		}
		public Vector3 Normal()
		{
			if (Vertices.Count < 3)
			{
				return Vector3.Zero;
			}

			Vector3 edge1 = Vertices[1].Position - Vertices[0].Position;
			Vector3 edge2 = Vertices[2].Position - Vertices[0].Position;
			return Vector3.Normalize(Vector3.Cross(edge1, edge2));
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
		public List<Edge> GetEdges() // get edges from faces
		{
			List<Edge> edges = new List<Edge>();

			for (int i = 0; i < Vertices.Count; i++)
			{
				Vertex start = Vertices[i];
				Vertex end = Vertices[(i + 1) % Vertices.Count];
				edges.Add(new Edge(start, end));
			}

			return edges;
		}
	}
}
