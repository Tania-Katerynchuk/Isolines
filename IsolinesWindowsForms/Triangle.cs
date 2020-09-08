using System;

namespace IsolinesWindowsForms
{
    public class Triangle
    {
        private Vector3[] m_Vectors;

        public Vector3[] Vectors
        {
            get { return m_Vectors; }
        }

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            m_Vectors = new Vector3[3];
            m_Vectors[0] = v1;
            m_Vectors[1] = v2;
            m_Vectors[2] = v3;
        }

        public Int32 IndexOfZ(Single z)
        {
            Int32 index;

            index = 0;

            for (Int32 i = 0; i < 3; i++)
            {
                if (m_Vectors[i].Z == z)
                {
                    index = i;
                }
            }

            return index;
        }
        public Int32 MidIndexZ(Int32 min, Int32 max)
        {
            Int32 index;

            index = 0;

            for (Int32 i = 0; i < 3; i++)
            {
                if (i != min && i != max)
                {
                    index = i;
                }
            }

            return index;
        }
    }
}
