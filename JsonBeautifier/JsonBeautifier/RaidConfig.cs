using System;
using System.Collections.Generic;
using System.Text;

namespace JsonBeautifier
{
    public class RaidConfig
    {
        public string Name;
        public RaidInfo RaidDetails;
    }

    public class RaidInfo
    {
        public int rows;
        public int columns;
        public Dictionary<string, RoomInfo> rooms;
    }

    public class RoomInfo
    {
        public string roomId;
        public bool starting;
        public Dictionary<string, MissionInfo> missions;
    }

    public class MissionInfo
    {
        public MissionFilters filters;
    }

    public class MissionFilters
    {
        public MissionFilters2 filters;
    }

    public class MissionFilters2
    {
        public string[] anyTrait;
        public string[] allTraits;
    }
}
