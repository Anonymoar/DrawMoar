﻿using System.Drawing;


namespace DrawMoar
{
    public interface ILayer
    {
        /// <summary>
        /// Состояние видимости слоя. 
        /// true - слой видимый, false - невидимый.
        /// При экспорте кадра в картинку картинка получается только из видимых слоёв.
        /// </summary>
        bool Visible { get; set; }


        /// <summary>
        /// Название слоя.
        /// </summary>
        string Name { get; set; }


        /// <summary>
        /// Отображает содержимое слоя в консоли/на канвасе
        /// </summary>
        void Draw(Graphics g);

        Point Position { get; set; }
    }
}
