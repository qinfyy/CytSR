using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdSceneEntityMoveCsReq)]
    internal class HandlerSceneEntityMoveCsReq : IPacketHandler
    {

        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<SceneEntityMoveCsReq>(packet);

            var rsp = new SceneEntityMoveScRsp
            {
                Retcode = 0,
            };

            foreach (var entityMotion in req.EntityMotionLists)
            {
                if (entityMotion.Motion != null)
                {
                    //Console.WriteLine(@$"
                    //     [POSITION] entity_id: {entityMotion.EntityId}, 
                    //     posX: {entityMotion.Motion.Pos.X}, 
                    //     posY: {entityMotion.Motion.Pos.Y}, 
                    //     posZ: {entityMotion.Motion.Pos.Z}, 
                    //     rotX: {entityMotion.Motion.Rot.X},
                    //     rotY: {entityMotion.Motion.Rot.Y},
                    //     rotZ: {entityMotion.Motion.Rot.Z},
                    //     "
                    //);
                }
                rsp.EntityMotionLists.Add(entityMotion);
            }

            session.Send(NetPacket.Create(CmdId.CmdSceneEntityMoveScRsp));
        }
    }
}
