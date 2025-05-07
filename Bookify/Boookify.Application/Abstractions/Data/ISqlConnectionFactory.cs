using System.Data;

namespace Bookify.Application.Abstractions.Data;
//Dapper链接数据库的工厂接口
public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
