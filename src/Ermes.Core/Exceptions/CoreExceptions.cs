using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Ermes.Exceptions
{
    public static class CoreExceptions
    {
        public enum SqlExceptionType {Duplicate, Unknown};
        public static SqlExceptionType FriendlySQLExceptions(Exception exception, out string userFriendlyMessageCode, out Object[] userFriendlyParams)
        {
            userFriendlyMessageCode = "UnknownError";
            userFriendlyParams = new Object[1];
            SqlExceptionType type = SqlExceptionType.Unknown;
            if (exception.InnerException == null || exception.InnerException.GetType() != typeof(PostgresException))
                return SqlExceptionType.Unknown;
            if(exception.InnerException is PostgresException innerException)
            {
                switch(innerException.SqlState)
                {
                    case "23505":
                        type = SqlExceptionType.Duplicate;
                        if(innerException.Detail.Contains(','))
                        {
                            userFriendlyMessageCode = "SqlComboDuplicate";
                            userFriendlyParams = new object[] {innerException.Detail.Split("\")=")[1].Remove(0,6).Replace("Id","")};
                        }
                        else
                        {
                            userFriendlyMessageCode = "SqlSingleDuplicate";
                            if(innerException.Detail.Contains("=("))
                                userFriendlyParams = new object[] {innerException.Detail.Split("=(")[1].Split(")")[0]};
                        }
                        break;
                    default:
                        type = SqlExceptionType.Unknown;
                        userFriendlyMessageCode = innerException.MessageText;
                        break;

                }         
            }            
            return type;
        }
    }
}
