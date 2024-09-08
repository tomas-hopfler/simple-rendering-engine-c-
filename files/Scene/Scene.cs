using System.Collections;
using System.Data;
using System.Drawing;
using System.Numerics;
using System.Xml.Schema;

namespace ConsoleEngine
{
    public class Scene
	{
		public ArrayList Objects = new ArrayList();
		public Camera Camera;

		public Scene()
		{
			Objects = new ArrayList();
			Camera = new Camera(new Vector3(0, 0, 0),new Vector3(0, 0, 0), 1920, 1080);
		}

		public void Add<T>(T obj)
		{
			Objects.Add(obj);
		}

		public void Render(ConsoleDisplay condis)
		{
			FrameBuffer.Clear();

			List<Tuple<Face, double>> allFaces = new List<Tuple<Face, double>>();

			foreach (var obj in Objects)
			{
				if (obj is Mesh mesh)
				{
					foreach (Face face in mesh.Faces)
					{
						double distance = Vector3.Distance(face.Center(), Camera.Position);
						allFaces.Add(new Tuple<Face, double>(face, distance));
					}
				}
			}

			var sortedFaces = allFaces.OrderByDescending(tuple => tuple.Item2).Select(tuple => tuple.Item1);

			foreach (Face face in sortedFaces)
			{
				if (Camera != null)
				{
					Vertex[] projectedVertices = face.Vertices.Select(v => Camera.Project(v)).ToArray();

					if (projectedVertices.All(v => v != null))
					{
						double distance = Vector3.Distance(face.Center(), Camera.Position);
						int colorValue = (int)Math.Min(distance, 255);

						List<Vector2> vector2List = projectedVertices.Select(v => new Vector2(v.Position.X, v.Position.Y)).ToList();

						FrameBuffer.DrawPolygon(vector2List, true, colorValue);
					}
				}
			}

			condis.Render();
		}

		public void Render(WindowsDisplay windis)
		{
			FrameBuffer.Clear();

			List<Tuple<Face, double>> allFaces = new List<Tuple<Face, double>>();

			foreach (var obj in Objects)
			{
				if (obj is Mesh mesh)
				{
					foreach (Face face in mesh.Faces)
					{
						double distance = Vector3.Distance(face.Center(), Camera.Position);
						allFaces.Add(new Tuple<Face, double>(face, distance));
					}
				}
			}

			var sortedFaces = allFaces.OrderByDescending(tuple => tuple.Item2).Select(tuple => tuple.Item1);

			foreach (Face face in sortedFaces)
			{
				if (Camera != null)
				{
					Vertex[] projectedVertices = face.Vertices.Select(v => Camera.Project(v)).ToArray();

					if (projectedVertices.All(v => v != null))
					{
						double distance = Vector3.Distance(face.Center(), Camera.Position);

						List<Vector2> vector2List = projectedVertices.Select(v => new Vector2(v.Position.X, v.Position.Y)).ToList();

						// random shading
						if(distance != 0)
						{
							FrameBuffer.DrawPolygon(vector2List, true, face.Color/(int)distance);
						}

						// solidface no shading
						//FrameBuffer.DrawPolygon(vector2List, true, face.Color;
					}
				}
			}

			windis.Render();
		}
	}
}
