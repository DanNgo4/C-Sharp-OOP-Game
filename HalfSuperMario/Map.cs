﻿using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HalfSuperMario
{
    public class Map
    {
        private string[,] _map;   // 2D array of string

        public string[,] Maps
        {
            get 
            { 
                return _map; 
            }
        }

        public Map(string[,] map)
        {
            _map = map;
        }

        public Map() : this(new string[,] {
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "b", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "b", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "b", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "b", "", "", "", "", "", "", "b", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" }
        })
        { }
    }
}
