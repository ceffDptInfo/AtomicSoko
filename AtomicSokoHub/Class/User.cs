﻿using AtomicSokoLibrary;
using Microsoft.AspNetCore.SignalR;
using System.Drawing;

namespace AtomicSokoHub
{
    public enum UserState
    {
        Spectator,
        InLife,
        Dead,
    }

    public class User
    {

        public string? Id { get; set; }
        public string? UserName { get; set; }
        public ISingleClientProxy? Proxy { get; set; }
        public bool IsReady { get; set; } = false;
        public UserState State { get; set; } = UserState.Spectator;
        public bool IsConnected { get; set; } = true;
        public string Color { get; set; } = string.Empty;
        public string? Skin { get; set; }
        public bool Admin { get; set; } = false;
        public Int64 Cash { get; set; } = 0;
        public PowerUps PowerUp { get; set; } = PowerUps.None;
        public int CashMultipliers { get; set; } = 1;

        public void EmptyUser()
        {
            UserName = string.Empty;
            Proxy = null;
            IsConnected = false;
            Color = string.Empty;
            Skin = null;
        }

    }
}
