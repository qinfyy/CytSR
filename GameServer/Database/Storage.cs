using System.Data.SQLite;
using ProtoBuf;
using Proto.Server;

namespace GameServer.Database
{
    public static class Storage
    {
        private static readonly string DatabaseFile = Program.Config.DatabasePath;
        private static readonly string ConnectionString = $"Data Source={DatabaseFile};Version=3;Pooling=True;Max Pool Size=100;";
        private static readonly object _uidLock = new object();

        public static void Initialize()
        {
            if (!Directory.Exists("Save"))
                Directory.CreateDirectory("Save");

            if (!File.Exists(DatabaseFile))
            {
                SQLiteConnection.CreateFile(DatabaseFile);

                using var conn = OpenConnection();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    PRAGMA journal_mode=WAL;
                    CREATE TABLE IF NOT EXISTS players (
                        uid INTEGER PRIMARY KEY,
                        account_uid TEXT UNIQUE NOT NULL,
                        data BLOB NOT NULL
                    );
                ";

                cmd.ExecuteNonQuery();
            }
        }

        private static SQLiteConnection OpenConnection()
        {
            var conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA journal_mode=WAL;";
            cmd.ExecuteNonQuery();

            return conn;
        }

        public static void Save(uint uid, PlayerData playerData)
        {
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, playerData);
            byte[] dataBytes = ms.ToArray();

            using var conn = OpenConnection();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE players SET data = @data WHERE uid = @uid;";
            cmd.Parameters.AddWithValue("@uid", uid);
            cmd.Parameters.AddWithValue("@data", dataBytes);

            int rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new Exception($"保存失败：数据库中找不到 {uid}");
        }

        public static (uint, PlayerData) GetPlayerDataByAccountUid(string accountUid)
        {
            using var conn = OpenConnection();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT uid, data FROM players WHERE account_uid = @account_uid;";
            cmd.Parameters.AddWithValue("@account_uid", accountUid);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                uint uid = Convert.ToUInt32(reader.GetInt64(0));
                byte[] dataBytes = (byte[])reader["data"];

                using var ms = new MemoryStream(dataBytes);
                var playerData = Serializer.Deserialize<PlayerData>(ms);

                return (uid, playerData);
            }
            else
            {
                uint uid = NextUid(conn);
                var newPlayerData = new PlayerData();

                using var ms = new MemoryStream();
                Serializer.Serialize(ms, newPlayerData);
                byte[] dataBytes = ms.ToArray();

                var insertCmd = conn.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO players (uid, account_uid, data)
                    VALUES (@uid, @account_uid, @data);
                ";

                insertCmd.Parameters.AddWithValue("@uid", uid);
                insertCmd.Parameters.AddWithValue("@account_uid", accountUid);
                insertCmd.Parameters.AddWithValue("@data", dataBytes);

                insertCmd.ExecuteNonQuery();

                return (uid, newPlayerData);
            }
        }

        private static uint NextUid(SQLiteConnection conn)
        {
            lock (_uidLock)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT MAX(uid) FROM players;";
                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? (uint)((long)result + 1) : 1;
            }
        }
    }
}