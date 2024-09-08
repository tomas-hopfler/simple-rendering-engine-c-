using System.Globalization;

namespace ConsoleEngine
{
	public class MeshLoader
	{
		public static Mesh Import(string filePath) // load .obj file into mesh
		{
			Mesh mesh = new Mesh();
			List<Vertex> vertices = new List<Vertex>();
			List<List<int>> faces = new List<List<int>>();

			using (StreamReader reader = new StreamReader(filePath))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (parts.Length == 0) continue;

					switch (parts[0])
					{
						case "v":  // Vertex
							if (parts.Length >= 4)
							{
								float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
								float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
								float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
								vertices.Add(new Vertex(x, y, z));
							}
							break;

						case "f":  // Face
							List<int> faceIndices = new List<int>();
							for (int i = 1; i < parts.Length; i++)
							{
								string[] vertexData = parts[i].Split('/');
								int vertexIndex = int.Parse(vertexData[0]) - 1;
								faceIndices.Add(vertexIndex);
							}
							faces.Add(faceIndices);
							break;
					}
				}
			}

			mesh.Vertices.AddRange(vertices);

			// Create faces
			foreach (var faceIndices in faces)
			{
				Face face = new Face();

				for (int i = 0; i < faceIndices.Count; i++)
				{
					int currentIndex = faceIndices[i];
					int nextIndex = faceIndices[(i + 1) % faceIndices.Count];

					face.Vertices.Add(vertices[currentIndex]);
				}
				mesh.Faces.Add(face);
			}

			return mesh;
		}

		public static void Export(Scene scene, string filePath) // export scene into .obj file
		{
			using (StreamWriter writer = new StreamWriter(filePath))
			{
				int vertexOffset = 1;

				foreach (object obj in scene.Objects)
				{
					if (obj is Mesh mesh)
					{
						// Vertices
						foreach (Vertex vertex in mesh.Vertices)
						{
							writer.WriteLine($"v {vertex.Position.X.ToString(CultureInfo.InvariantCulture)} {vertex.Position.Y.ToString(CultureInfo.InvariantCulture)} {vertex.Position.Z.ToString(CultureInfo.InvariantCulture)}");
						}

						// Faces
						foreach (Face face in mesh.Faces)
						{
							writer.Write("f");
							foreach (Vertex vertex in face.Vertices)
							{
								int index = mesh.Vertices.IndexOf(vertex) + vertexOffset;
								writer.Write($" {index}");
							}
							writer.WriteLine();
						}

						vertexOffset += mesh.Vertices.Count;
					}
				}
			}
		}
	}
}
