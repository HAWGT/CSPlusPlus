using CSGOPlusPlus.Helpers;
using CSGSI;
using CSGSI.Nodes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WindowsInput.Native;

namespace CSGOPlusPlus.Modules
{
    class CSGOCore
    {

        public MainForm mainForm;

        public static GameStateListener _gsl;

        private VirtualKeyCode slot1 = VirtualKeyCode.VK_1;
        private VirtualKeyCode slot2 = VirtualKeyCode.VK_2;
        private VirtualKeyCode slot3 = VirtualKeyCode.VK_3;

        private VirtualKeyCode killKey = VirtualKeyCode.VK_6;
        private VirtualKeyCode killHSKey = VirtualKeyCode.VK_7;
        private VirtualKeyCode moneyKey = VirtualKeyCode.VK_8;
        private VirtualKeyCode reloadKey = VirtualKeyCode.VK_9;
        private VirtualKeyCode mvpKey = VirtualKeyCode.VK_0;
        private VirtualKeyCode flashKey = VirtualKeyCode.VK_I;
        private VirtualKeyCode smokeKey = VirtualKeyCode.VK_O;
        private VirtualKeyCode burnKey = VirtualKeyCode.VK_P;
        private VirtualKeyCode knifeKey = VirtualKeyCode.VK_L;
        private VirtualKeyCode assistKey = VirtualKeyCode.VK_V;
        private VirtualKeyCode deathKey = VirtualKeyCode.VK_N;
        private VirtualKeyCode spKillKey = VirtualKeyCode.VK_F;
        private VirtualKeyCode aceKey = VirtualKeyCode.VK_J;
        private VirtualKeyCode spAceKey = VirtualKeyCode.VK_T;
        private VirtualKeyCode taserKey = VirtualKeyCode.VK_H;

        private readonly int CS_MM_MAXMONEY = 16000;
        private readonly int CS_WM_MAXMONEY = 8000;

        private readonly int CS_MM_TEAMSIZE = 5;
        private readonly int CS_WM_TEAMSIZE = 2;

        private int lastKills = 0;
        private int lastKillsHS = 0;
        private int lastAssists = 0;
        private int lastDeaths = 0;
        private int lastMVPs = 0;

        private bool reloading = false;
        private bool maxMoney = false;
        private bool knife = false;

        private bool flashed = false;
        private bool smoked = false;
        private bool burning = false;

        private string player;

        private int rounds = 0;

        private bool playing = false;

        private ushort port = 13337;

        public CSGOCore(MainForm reference)
        {
            this.mainForm = reference;
            CreateGsifile();

            Process[] pname = Process.GetProcessesByName("csgo");
            if (pname.Length == 0)
            {
            }

            _gsl = new GameStateListener(port);
            _gsl.NewGameState += OnNewGameState;


            if (!_gsl.Start())
            {
            }
        }

        private void OnNewGameState(GameState gs)
        {
            player = gs.Provider.SteamID;
            rounds = gs.Map.Round;
            if (rounds >= 0)
            {
                playing = true;
            } else
            {
                playing = false;
            }
            if (gs.Player.SteamID == player && playing && gs.Player.Activity == CSGSI.Nodes.PlayerActivity.Playing)
            {

                int kills = gs.Player.State.RoundKills;
                int killsHS = gs.Player.State.RoundKillHS;

                int assists = gs.Player.MatchStats.Assists;
                int deaths = gs.Player.MatchStats.Deaths;

                int flash = gs.Player.State.Flashed;
                int smoke = gs.Player.State.Smoked;
                int burn = gs.Player.State.Burning;

                int money = gs.Player.State.Money;

                int mvps = gs.Player.MatchStats.MVPs;

                if (flash == 0) flashed = false;
                if (smoke == 0) smoked = false;
                if (burn == 0) burning = false;

                if (kills == 0) lastKills = 0;
                if (killsHS == 0) lastKillsHS = 0;
                if (mvps == 0) lastMVPs = 0;
                if (assists == 0) lastAssists = 0;
                if (deaths == 0) lastDeaths = 0;

                if (gs.Player.Weapons.ActiveWeapon.State != CSGSI.Nodes.WeaponState.Reloading) reloading = false;

                if (money < CS_MM_MAXMONEY && gs.Map.Mode != MapMode.ScrimComp2v2) maxMoney = false;
                if (money < CS_WM_MAXMONEY && gs.Map.Mode == MapMode.ScrimComp2v2) maxMoney = false;

                if (gs.Player.Weapons.ActiveWeapon.Type != WeaponType.Knife) knife = false;


                //TASER

                if (gs.Player.Weapons.ContainsWeapon("weapon_taser"))
                {
                    BindExecuter.ExecuteBind(slot3, mainForm.Taser());
                    if (gs.Player.Weapons.ActiveWeapon.Name == "weapon_taser")
                    {
                        BindExecuter.ExecuteBind(taserKey, mainForm.Taser());
                        BindExecuter.ExecuteBind(slot2, mainForm.Taser());
                        BindExecuter.ExecuteBind(slot1, mainForm.Taser());
                    }
                }

                //ACE ALL HS

                if (killsHS != lastKillsHS && killsHS == CS_MM_TEAMSIZE && gs.Map.Mode != MapMode.ScrimComp2v2)
                {
                    BindExecuter.ExecuteBind(spAceKey, mainForm.SpecialAce());
                }

                if (killsHS != lastKillsHS && killsHS == CS_WM_TEAMSIZE && gs.Map.Mode == MapMode.ScrimComp2v2)
                {
                    BindExecuter.ExecuteBind(spAceKey, mainForm.SpecialAce());
                }

                //ACE

                if (kills != lastKills && kills == CS_MM_TEAMSIZE && gs.Map.Mode != MapMode.ScrimComp2v2)
                {
                    BindExecuter.ExecuteBind(aceKey, mainForm.Ace());
                }

                if (kills != lastKills && kills == CS_WM_TEAMSIZE && gs.Map.Mode == MapMode.ScrimComp2v2)
                {
                    BindExecuter.ExecuteBind(aceKey, mainForm.Ace());
                }


                //KNIFE KILL
                if (kills != lastKills && kills > 0 && gs.Player.Weapons.ActiveWeapon.Type == WeaponType.Knife)
                {
                    if (kills > lastKills)
                    {
                        BindExecuter.ExecuteBind(spKillKey, mainForm.SpecialKill());
                    }
                    lastKills = kills;
                }

                //MVP
                if (mvps != lastMVPs && mvps > 0)
                {
                    if (mvps > lastMVPs)
                    {
                        BindExecuter.ExecuteBind(mvpKey, mainForm.MVP());
                    }
                    lastMVPs = mvps;
                }

                //HS KILL
                if (killsHS != lastKillsHS && killsHS > 0)
                {
                    if (killsHS > lastKillsHS)
                    {
                        BindExecuter.ExecuteBind(killHSKey, mainForm.KillHS());
                    }
                    lastKillsHS = killsHS;
                }

                //KILL
                if (kills != lastKills && kills > 0)
                {
                    if (kills > lastKills)
                    {
                        BindExecuter.ExecuteBind(killKey, mainForm.Kill());
                    }
                    lastKills = kills;
                }

                //ASSISTS
                if (assists != lastAssists && assists > 0)
                {
                    if (assists > lastAssists)
                    {
                        BindExecuter.ExecuteBind(assistKey, mainForm.Assist());
                    }
                    lastAssists = assists;
                }

                //DEATH
                if (deaths != lastDeaths && deaths > 0)
                {
                    if (deaths > lastDeaths)
                    {
                        BindExecuter.ExecuteBind(deathKey, mainForm.Death());
                    }
                    lastDeaths = deaths;
                }

                //16K MONEY
                if (money == CS_MM_MAXMONEY && gs.Map.Mode != MapMode.ScrimComp2v2 && !maxMoney)
                {
                    BindExecuter.ExecuteBind(moneyKey, mainForm.Money());
                    maxMoney = true;
                }

                //8K MONEY
                if (money == CS_MM_MAXMONEY && gs.Map.Mode == MapMode.ScrimComp2v2 && !maxMoney)
                {
                    BindExecuter.ExecuteBind(moneyKey, mainForm.Money());
                    maxMoney = true;
                }

                //FLASH
                if (flash > 0 && !flashed)
                {
                    BindExecuter.ExecuteBind(flashKey, mainForm.Flash());
                    flashed = true;
                }

                //BURN
                if (burn > 0 && !burning)
                {
                    BindExecuter.ExecuteBind(burnKey, mainForm.Burn());
                    burning = true;
                }

                //SMOKE
                if (smoke > 0 && !smoked)
                {
                    BindExecuter.ExecuteBind(smokeKey, mainForm.Smoke());
                    smoked = true;
                }

                //RELOAD
                if (gs.Player.Weapons.ActiveWeapon.State == CSGSI.Nodes.WeaponState.Reloading && !reloading)
                {
                    BindExecuter.ExecuteBind(reloadKey, mainForm.Reload());
                    reloading = true;
                }

                //KNIFE
                if (gs.Player.Weapons.ActiveWeapon.Type == WeaponType.Knife && !knife)
                {
                    BindExecuter.ExecuteBind(knifeKey, mainForm.Knife());
                    knife = true;
                }
            } 
        }

        private void CreateGsifile()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");

            if (regKey != null)
            {
                string gsifolder = regKey.GetValue("SteamPath") +
                                   @"\steamapps\common\Counter-Strike Global Offensive\csgo\cfg";
                Directory.CreateDirectory(gsifolder);
                string gsifile = gsifolder + @"\gamestate_integration_CSC.cfg";
                if (File.Exists(gsifile))
                    return;

                string[] contentofgsifile =
                {
                    "\"CSC\"",
                    "{",
                    "    \"uri\"           \"http://localhost:"+port+"\"",
                    "    \"timeout\"       \"5.0\"",
                    "    \"buffer\"        \"0.1\"",
                    "    \"throttle\"      \"0.1\"",
                    "    \"heartbeat\"     \"0.1\"",
                    "    \"data\"",
                    "    {",
                    "        \"provider\"           \"1\"",
                    "        \"map\"                \"1\"",
                    "        \"round\"              \"1\"",
                    "        \"player_id\"          \"1\"",
                    "        \"player_state\"       \"1\"",
                    "        \"player_weapons\"     \"1\"",
                    "        \"player_match_stats\" \"1\"",
                    "    }",
                    "}",

                };

                File.WriteAllLines(gsifile, contentofgsifile);
            }
            else
            {
            }
        }
    }
}
