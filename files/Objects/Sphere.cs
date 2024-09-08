using System.Numerics;

namespace ConsoleEngine
{
	public class Sphere : Mesh
	{
		public Sphere(float diameter, Vector3 position, int resolution)
		{
			float radius = diameter / 2;
			CreateSphere(radius, position, resolution);
		}

		private void CreateSphere(float radius, Vector3 center, int resolution)
		{
			for (int lat = 0; lat <= resolution; lat++)
			{
				float theta = lat * MathF.PI / resolution;
				float sinTheta = MathF.Sin(theta);
				float cosTheta = MathF.Cos(theta);

				for (int lon = 0; lon <= resolution; lon++)
				{
					float phi = lon * 2 * MathF.PI / resolution;
					float sinPhi = MathF.Sin(phi);
					float cosPhi = MathF.Cos(phi);

					float x = cosPhi * sinTheta;
					float y = cosTheta;
					float z = sinPhi * sinTheta;

					Vector3 position = new Vector3(x, y, z) * radius + center;
					Vertices.Add(new Vertex(position));
				}
			}

			for (int lat = 0; lat < resolution; lat++)
			{
				for (int lon = 0; lon < resolution; lon++)
				{
					int current = lat * (resolution + 1) + lon;
					int next = current + resolution + 1;

					Faces.Add(new Face(new List<Vertex> {
						Vertices[current],
						Vertices[current + 1],
						Vertices[next + 1],
						Vertices[next] }));
				}
			}
		}
	}
}

