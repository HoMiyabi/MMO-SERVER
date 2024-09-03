namespace GameServer.Database;

public class Db
{
    /// <summary>
    /// 可配置的参数
    /// </summary>
    private static string host = "127.0.0.1";
    private static int port = 3306;
    private static string user = "root";
    private static string password = "sql04517FuNc";
    private static string dbName = "game";

    private static string connectionString =
        $"Data Source={host};Port={port};User ID={user};Password={password};" +
        $"Initial Catalog={dbName};Charset=utf8mb4;SslMode=none;Min pool size=10";

    public static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
        .UseConnectionString(FreeSql.DataType.MySql, connectionString)
        .UseAutoSyncStructure(true) // 自动同步实体到数据库
        .Build();
}