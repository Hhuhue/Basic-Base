using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models.Observers
{
    public interface IViewObserver
    {
        void TileChanged(int positionX, int positionY);

        void ToggleTileSelector(bool enabled);
    }
}
