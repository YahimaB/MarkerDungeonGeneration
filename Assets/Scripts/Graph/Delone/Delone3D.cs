/* Adapted from https://github.com/Bl4ckb0ne/delaunay-triangulation

Copyright (c) 2015-2019 Simon Zeni (simonzeni@gmail.com)


Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:


The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.


THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

using System.Collections.Generic;
using UnityEngine;

namespace Graph.Delone
{
    public class Delone3D
    {
        public List<Vertex> Vertices { get; private set; }
        public List<Edge> Edges { get; private set; }
        public List<Tetrahedron> Tetrahedra { get; private set; }

        private Delone3D()
        {
            Edges = new List<Edge>();
            Tetrahedra = new List<Tetrahedron>();
        }

        public static Delone3D Triangulate(List<Vertex> vertices)
        {
            var delone = new Delone3D();
            delone.Vertices = new List<Vertex>(vertices);
            delone.Triangulate();

            return delone;
        }

        private void Triangulate()
        {
            var minX = Vertices[0].Position.x;
            var minY = Vertices[0].Position.y;
            var minZ = Vertices[0].Position.z;
            var maxX = minX;
            var maxY = minY;
            var maxZ = minZ;

            foreach (var vertex in Vertices)
            {
                if (vertex.Position.x < minX) minX = vertex.Position.x;
                if (vertex.Position.x > maxX) maxX = vertex.Position.x;
                if (vertex.Position.y < minY) minY = vertex.Position.y;
                if (vertex.Position.y > maxY) maxY = vertex.Position.y;
                if (vertex.Position.z < minZ) minZ = vertex.Position.z;
                if (vertex.Position.z > maxZ) maxZ = vertex.Position.z;
            }

            var dx = maxX - minX;
            var dy = maxY - minY;
            var dz = maxZ - minZ;
            var deltaMax = Mathf.Max(dx, dy, dz) * 2;

            var p1 = new Vertex(new Vector3(minX - deltaMax, minY - 1, minZ - deltaMax));
            // Vertex p1 = new Vertex(new Vector3(minX - 1, minY - 1, minZ - 1));
            var p2 = new Vertex(new Vector3(maxX + deltaMax, minY - 1, minZ - 1));
            var p3 = new Vertex(new Vector3(minX - 1, maxY + deltaMax, minZ - 1));
            var p4 = new Vertex(new Vector3(minX - 1, minY - 1, maxZ + deltaMax));

            //Добавить внешний тетраэдр основу
            Tetrahedra.Add(new Tetrahedron(p1, p2, p3, p4));

            //Для каждой точки (центра комнаты) в пространстве
            foreach (var vertex in Vertices)
            {
                var triangles = new List<Triangle>();
                foreach (var t in Tetrahedra)
                {
                    if (t.CircumCircleContains(vertex.Position))
                    {
                        //Получить все треугольники (triangles), принадлежащие тетраэдрам,
                        //чьи описывающие сферы включают точку. Сами тетраэдры пометить
                        //на удаление
                        t.IsBad = true;
                        triangles.Add(new Triangle(t.A, t.B, t.C));
                        triangles.Add(new Triangle(t.A, t.B, t.D));
                        triangles.Add(new Triangle(t.A, t.C, t.D));
                        triangles.Add(new Triangle(t.B, t.C, t.D));
                    }
                }

                for (var i = 0; i < triangles.Count; i++)
                {
                    //если есть одинаковые треугольники - пометить их на удаление
                    for (var j = i + 1; j < triangles.Count; j++)
                    {
                        if (Triangle.AlmostEqual(triangles[i], triangles[j]))
                        {
                            triangles[i].IsBad = true;
                            triangles[j].IsBad = true;
                        }
                    }
                }

                //Удалить все помеченные тетраэдры и треугольники
                Tetrahedra.RemoveAll(t => t.IsBad);
                triangles.RemoveAll(t => t.IsBad);

                //Создать новые тетраэдры из комбинации оставшихся треугольников и новой точки
                foreach (var triangle in triangles)
                {
                    Tetrahedra.Add(new Tetrahedron(triangle.U, triangle.V, triangle.W, vertex));
                }
            }

            Tetrahedra.RemoveAll(t => t.ContainsVertex(p1) || t.ContainsVertex(p2) || t.ContainsVertex(p3) || t.ContainsVertex(p4));

            var edgeSet = new HashSet<Edge>();

            foreach (var t in Tetrahedra)
            {
                var ab = new Edge(t.A, t.B);
                var bc = new Edge(t.B, t.C);
                var ca = new Edge(t.C, t.A);
                var da = new Edge(t.D, t.A);
                var db = new Edge(t.D, t.B);
                var dc = new Edge(t.D, t.C);

                if (edgeSet.Add(ab))
                    Edges.Add(ab);

                if (edgeSet.Add(bc))
                    Edges.Add(bc);

                if (edgeSet.Add(ca))
                    Edges.Add(ca);

                if (edgeSet.Add(da))
                    Edges.Add(da);

                if (edgeSet.Add(db))
                    Edges.Add(db);

                if (edgeSet.Add(dc))
                    Edges.Add(dc);
            }
        }
    }
}