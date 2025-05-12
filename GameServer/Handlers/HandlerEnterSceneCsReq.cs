using GameServer.Resources;
using GameServer.Database;
using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdEnterSceneCsReq)]
    internal class HandlerEnterSceneCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<EnterSceneCsReq>(packet);

            GameData.MapEntranceData.TryGetValue(req.EntryId, out var entranceConfig);
            session._Player.Data.SceneCompData.FloorId = entranceConfig.FloorId;
            session._Player.Data.SceneCompData.PlaneId = entranceConfig.PlaneId;
            session._Player.Data.SceneCompData.EntryId = entranceConfig.Id;
            session._Player.SceneMgr = new Game.SceneManager(session._Player);

            if (req.TeleportId > 0)
            {
                session._Player.SceneMgr.TeleportId = req.TeleportId;
            }

            var enterSceneByServer = new EnterSceneByServerScNotify
            {
                Reason = EnterSceneReason.EnterSceneReasonNone,
                Scene = session._Player.SceneMgr.Load(),
                Lineup = session._Player.LineupMgr.GetCurLineupProto(),
            };

            session._Player.SceneMgr.TeleportId = 0;

            session.Send(NetPacket.Create(CmdId.CmdEnterSceneByServerScNotify, enterSceneByServer));
            
            var moveNotify = new SceneEntityMoveScNotify
            {
                EntryId = session._Player.Data.SceneCompData.EntryId,
                Motion = new MotionInfo()
                {
                    Pos = new Position(session._Player.Data.SceneCompData.Pos).ToProto(),
                    Rot = new Position(session._Player.Data.SceneCompData.Rot).ToProto()
                }
            };

            //session.Send(NetPacket.Create(CmdId.CmdSceneEntityMoveScNotify, moveNotify));

            session.Send(NetPacket.Create(CmdId.CmdEnterSceneScRsp));
        }
    }
}
