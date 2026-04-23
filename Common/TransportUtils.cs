using ICities;

namespace SleepyCommon
{
    public class TransportUtils
    {
        public enum TransportType
        {
            None,
            Road,
            Plane,
            Train,
            Ship,
        }

        // -------------------------------------------------------------------------------------------
        public static TransportType GetTransportType(Building building)
        {
            if (building.Info is not null)
            {
                switch (building.Info.GetSubService())
                {
                    case ItemClass.SubService.PublicTransportPlane:
                        {
                            return TransportType.Plane;
                        }
                    case ItemClass.SubService.PublicTransportShip:
                        {
                            return TransportType.Ship;
                        }
                    case ItemClass.SubService.PublicTransportTrain:
                        {
                            return TransportType.Train;
                        }
                }
            }

            return TransportType.Road;
        }

        // -------------------------------------------------------------------------------------------
        public static TransportType GetPrimaryTransportType(Building building)
        {
            if (building.Info is not null)
            {
                TransportInfo? transportInfo = building.Info.m_buildingAI.GetSecondaryTransportLineInfo();
                if (transportInfo is not null)
                {
                    switch (transportInfo.m_transportType)
                    {
                        case TransportInfo.TransportType.Airplane:
                            {
                                return TransportType.Plane;
                            }
                        case TransportInfo.TransportType.Ship:
                            {
                                return TransportType.Ship;
                            }
                        case TransportInfo.TransportType.Train:
                            {
                                return TransportType.Train;
                            }
                        default:
                            {
                                return TransportType.Road;
                            }
                    }
                }
            }

            return TransportType.None;
        }

        // -------------------------------------------------------------------------------------------
        public static TransportType GetSecondaryTransportType(Building building)
        {
            if (building.Info is not null)
            {
                TransportInfo? transportInfo = building.Info.m_buildingAI.GetSecondaryTransportLineInfo();
                if (transportInfo is not null)
                {
                    switch (transportInfo.m_transportType)
                    {
                        case TransportInfo.TransportType.Airplane:
                            {
                                return TransportType.Plane;
                            }
                        case TransportInfo.TransportType.Ship:
                            {
                                return TransportType.Ship;
                            }
                        case TransportInfo.TransportType.Train:
                            {
                                return TransportType.Train;
                            }
                        default:
                            {
                                return TransportType.Road;
                            }
                    }
                }
            }

            return TransportType.None;
        }

        // -------------------------------------------------------------------------------------------
        public static TransportType GetTransportType(ushort buildingId)
        {
            Building building = BuildingManager.instance.m_buildings.m_buffer[buildingId];
            if (building.m_flags != 0)
            {
                return TransportUtils.GetTransportType(building);
            }

            return TransportType.None;
        }

        public static string GetTransportDescription(TransportType type)
        {
            return Localization.Get($"TransportType{type}");
        }
    }
}