using System.Numerics;

namespace ConsoleEngine
{
	public class Edge
	{
		public Vertex Start, End;

		public Edge()
		{
			Start = new Vertex();
			End = new Vertex();
		}
		public Edge(Vertex start, Vertex end)
		{
			Start = start;
			End = end;
		}
		public Vertex Center()
		{
			return new Vertex((Start.Position + End.Position) / 2);
		}
	}
}
