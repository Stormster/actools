﻿using System;
using System.Linq;
using AcManager.Tools.Helpers;
using AcManager.Tools.Managers.Online;
using AcTools.Processes;
using StringBasedFilter;

namespace AcManager.Tools.Filters {
    public class ServerEntryTester : IParentTester<ServerEntry> {
        public static readonly ServerEntryTester Instance = new ServerEntryTester();

        public static string InnerParameterFromKey(string key) {
            switch (key) {
                case "country":
                    return nameof(ServerEntry.Country);

                case "ip":
                    return nameof(ServerEntry.Ip);

                case "d":
                case "drivers":
                case "players":
                case "full":
                case "free":
                    return nameof(ServerEntry.CurrentDriversCount);

                case "driver":
                case "player":
                case "driverteam":
                case "playerteam":
                    return nameof(ServerEntry.CurrentDrivers);

                case "track":
                case "trackid":
                    return nameof(ServerEntry.Track);

                case "a":
                case "available":
                case "car":
                case "carid":
                    return nameof(ServerEntry.Cars);

                case "p":
                case "password":
                    return nameof(ServerEntry.PasswordRequired);

                case "errors":
                case "haserrors":
                    return nameof(ServerEntry.HasErrors);

                case "active":
                    return nameof(ServerEntry.CurrentSessionType);

                case "booking":
                    return nameof(ServerEntry.BookingMode);

                case "practice":
                case "qualification":
                case "race":
                case "drift":
                case "hotlap":
                case "timeattack":
                    return nameof(ServerEntry.Sessions);

                case "left":
                case "ended":
                    return nameof(ServerEntry.SessionEnd);

                case "name":
                case null:
                    return nameof(ServerEntry.DisplayName);
            }

            return null;
        }

        public static string InheritingParameterFromKey(string key) {
            return InnerParameterFromKey(key);
        }

        public string ParameterFromKey(string key) {
            return InheritingParameterFromKey(key);
        }

        public bool Test(ServerEntry obj, string key, ITestEntry value) {
            if (key == null) {
                return SettingsHolder.Content.SimpleFiltering && value.Test(obj.TrackId) ||
                        value.Test(obj.Id) || value.Test(obj.DisplayName);
            }

            switch (key) {
                case "country":
                    return value.Test(obj.Country);

                case "ip":
                    return value.Test(obj.Ip);

                case "carid":
                    return obj.Cars?.Any(x => value.Test(x.CarObject.Id)) == true;

                case "car":
                    return obj.Cars?.Any(x => value.Test(x.CarObject.Id) || value.Test(x.CarObject.Name)) == true;

                case "a":
                case "available":
                    return obj.Cars?.Where(x => x.Available > 0).Any(x => value.Test(x.CarObject.Id) || value.Test(x.CarObject.Name)) == true;

                case "driver":
                case "player":
                    return obj.CurrentDrivers?.Any(x => value.Test(x.Name)) == true;

                case "driverteam":
                case "playerteam":
                    return obj.CurrentDrivers?.Any(x => value.Test(x.Team)) == true;

                case "trackid":
                    return value.Test(obj.Track?.Id);

                case "track":
                    return value.Test(obj.Track?.Id) || value.Test(obj.Track?.Name);

                case "c":
                case "cap":
                case "capacity":
                    return value.Test(obj.Capacity);

                case "d":
                case "drivers":
                case "players":
                    return value.Test(obj.CurrentDriversCount);

                case "full":
                    return value.Test(obj.CurrentDriversCount == obj.Capacity);

                case "free":
                    return value.Test(obj.Capacity - obj.CurrentDriversCount);

                case "missing":
                    return value.Test(obj.TrackId != null && obj.Track == null || obj.Cars?.Any(x => !x.CarExists) == true);

                case "ping":
                    return obj.Ping.HasValue && value.Test((double)obj.Ping);

                case "p":
                case "password":
                    return value.Test(obj.PasswordRequired);

                case "errors":
                case "haserrors":
                    return value.Test(obj.HasErrors);

                case "booking":
                    return value.Test(obj.BookingMode);

                case "active":
                    return value.Test(obj.CurrentSessionType?.ToString());

                case "left":
                    var now = DateTime.Now;
                    return obj.SessionEnd > now && value.Test((obj.SessionEnd - now).TotalMinutes);

                case "ended":
                    return value.Test(obj.SessionEnd <= DateTime.Now);
            }

            Game.SessionType sessionType;
            switch (key) {
                case "practice":
                    sessionType = Game.SessionType.Practice;
                    break;

                case "qualification":
                    sessionType = Game.SessionType.Qualification;
                    break;

                case "race":
                    sessionType = Game.SessionType.Race;
                    break;

                case "drift":
                    sessionType = Game.SessionType.Drift;
                    break;

                case "hotlap":
                    sessionType = Game.SessionType.Hotlap;
                    break;

                case "timeattack":
                    sessionType = Game.SessionType.TimeAttack;
                    break;

                case "name":
                    return value.Test(obj.DisplayName);

                default:
                    return false;
            }

            var session = obj.Sessions?.FirstOrDefault(x => x.Type == sessionType);
            return value.Test(session?.Duration / 60d ?? 0d);
        }

        public bool TestChild(ServerEntry obj, string key, IFilter filter) {
            switch (key) {
                case null:
                case "car":
                    return obj.Cars?.Any(x => filter.Test(CarObjectTester.Instance, x.CarObject)) == true;

                case "track":
                    return obj.Track != null && filter.Test(TrackBaseObjectTester.Instance, obj.Track);

                case "a":
                case "available":
                    return obj.Cars?.Where(x => x.Available > 0).Any(x => filter.Test(CarObjectTester.Instance, x.CarObject)) == true;
            }

            return false;
        }
    }
}
