using System;

namespace IsolinesWindowsForms
{
    public class Vector3
    {
        private Single m_X;
        private Single m_Y;
        private Single m_Z;

        public Single X
        {
            get { return m_X; }
            set { m_X = value; }
        }

        public Single Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        public Single Z
        {
            get { return m_Z; }
            set { m_Z = value; }
        }

        public Vector3(Single x, Single y, Single z = 0)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
        }
    }
}
