﻿using System;

using System.Drawing;

using DrawMoar.BaseElements;


using System.Windows.Controls;

namespace DrawMoar
{
    public interface ILayer : ICloneable
    {
        bool Visible { get; set; }

        string Name { get; set; }

        void Print(Canvas canvas);

        void Draw(Graphics g);

        void Transform(Transformation transformation);
    }
}
