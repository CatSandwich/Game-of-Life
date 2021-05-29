using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatDarkRift.Serialization;
using DarkRift;

namespace Common.Data
{
    public class RequestCellPlacement : EasySerializable<RequestCellPlacement>
    {
        public sbyte Player;
        public PregameGrid Grid;
    }
}
