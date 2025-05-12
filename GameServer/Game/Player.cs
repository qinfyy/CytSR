using GameServer.Network;
using GameServer.Database;
using Proto;
using Proto.Server;

namespace GameServer.Game
{
    public class Player
    {
        private bool _loggedIn = false;

        public uint Uid { get; private set; }
        public GameSession Session { get; private set; }

        public PlayerData Data { get; private set; }

        public LineupManager LineupMgr { get; private set; }
        public AvatarManager AvatarMgr { get; private set; }
        public ItemManager ItemMgr { get; private set; }
        public SceneManager SceneMgr { get; set; }

        public Player(GameSession session, uint uid, PlayerData player)
        {
            Session = session;
            Uid = uid;
            Data = player;

            ItemMgr = new ItemManager(this);
            //MultiPathAvatarMgr = new MultiPathAvatar(this);
            AvatarMgr = new AvatarManager(this);
            LineupMgr = new LineupManager(this);
            SceneMgr = new SceneManager(this);
        }


        public void InitNewPlayer()
        {
            InitPlayerData();
            AvatarMgr.InitDefaults();
            LineupMgr.InitDefaults();
            ItemMgr.InitDefaults();
            SceneMgr.InitDefaults();
        }

        public bool IsNewPlayer() => Data.BasicCompData == null;

        public bool IsLoggedIn() => _loggedIn;

        public void OnPlayerLogin()
        {
            if (!_loggedIn)
            {
                _loggedIn = true; // 当玩家登录时设置为 true

                if (IsNewPlayer())
                {
                    InitNewPlayer();
                }

                var basicComp = Data.BasicCompData;
                if (basicComp != null)
                {
                    basicComp.LoginTimes += 1;
                    basicComp.LastLoginTimestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
            }
        }

        public void OnPlayerLogout()
        {
            
            if (_loggedIn)
            {
                _loggedIn = false;
                // 保存数据
                Storage.Save(Uid, Data);
            }
        }

        public PlayerBasicInfo GetPlayerBasicInfoProto()
        {
            var basicComp = Data.BasicCompData;
            if (basicComp != null)
            {
                return new PlayerBasicInfo
                {
                    Nickname = basicComp.Nickname,
                    Level = basicComp.Level,
                    Exp = basicComp.Exp,
                    WorldLevel = basicComp.WorldLevel,
                    Stamina = 300
                };
            }

            return new PlayerBasicInfo(); // Return default if no data is available
        }

        public void Save()
        {
            Storage.Save(Uid, Data);
        }

        public void SyncClient()
        {
            var rsp = new PlayerSyncScNotify()
            {
                AvatarSync = new AvatarSync(),
                BasicInfo = GetPlayerBasicInfoProto()
            };

            foreach (var a in AvatarMgr.GetAvatarListProto())
            {
                rsp.AvatarSync.AvatarLists.Add(a);
            }

            foreach (var e in ItemMgr.GetEquipmentListProto())
            {
                rsp.EquipmentLists.Add(e);
            }

            foreach (var mpai in AvatarMgr.GetAvatarPathProto())
            {
                rsp.MultiPathAvatarInfoLists.Add(mpai);
            }

            Session.Send(NetPacket.Create(CmdId.CmdPlayerSyncScNotify, rsp));
        }

        private void InitPlayerData()
        {
            Data = new PlayerData
            {
                BasicCompData = new PlayerBasicData
                {
                    Level = 70,
                    WorldLevel = 6,
                    Nickname = "CytSR",
                    CreatedTimestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    LoginTimes = 0,
                    LastLoginTimestamp = 0
                }
            };
        }
    }
}
