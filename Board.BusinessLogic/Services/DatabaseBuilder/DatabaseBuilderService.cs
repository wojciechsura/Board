using AutoMapper;
using Board.BusinessLogic.Infrastructure.Document.Database;
using Board.Models.Document;
using Board.BusinessLogic.Services.Paths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Board.BusinessLogic.Services.DatabaseBuilder
{
    internal class DatabaseBuilderService : IDatabaseBuilderService
    {
        private readonly IPathService pathService;
        private readonly IMapper mapper;

        private BaseDatabase BuildSqlite(string projectPath, SQLiteDatabaseDefinition sqliteDatabase)
        {
            var sqliteDatabasePath = pathService.GetSQLiteDatabasePath(projectPath);
            return new SQLiteDatabase(sqliteDatabasePath, mapper);
        }

        public DatabaseBuilderService(IPathService pathService, IMapper mapper)
        {
            this.pathService = pathService;
            this.mapper = mapper;
        }

        public BaseDatabase Create(string projectPath, BaseDatabaseDefinition database)
        {
            switch (database)
            {
                case SQLiteDatabaseDefinition sqliteDatabase:
                    {
                        return BuildSqlite(projectPath, sqliteDatabase);
                    }
                default:
                    throw new InvalidOperationException("Unsupported database type!");
            }
        }

        public BaseDatabase Open(string projectPath, BaseDatabaseDefinition database)
        {
            switch (database)
            {
                case SQLiteDatabaseDefinition sqliteDatabase:
                    {
                        return BuildSqlite(projectPath, sqliteDatabase);
                    }
                default:
                    throw new InvalidOperationException("Unsupported database type!");
            }
        }
    }
}
