/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Json;
using KaVE.FeedbackProcessor.Model.DevelopmentHistory;

namespace KaVE.FeedbackProcessor.Database.DevelopmentHistory
{
    public class DevelopmentHistoryDatabase : IDisposable
    {
        private readonly SQLiteConnection _connection;

        public DevelopmentHistoryDatabase(string name)
        {
            EnsureDatabaseExists(name);
            _connection = new SQLiteConnection(string.Format("Data Source={0};Version=3", name));
            _connection.Open();

            CreateTable(
                "CREATE TABLE IF NOT EXISTS ContextHistories (Id INTEGER PRIMARY KEY, WorkPeriod VARCHAR(255), Timestamp DATETIME, TargetType VARCHAR(255), Context TEXT)");
            CreateTable(
                "CREATE TABLE IF NOT EXISTS OUHistories (Id INTEGER PRIMARY KEY, WorkPeriod VARCHAR(255), Timestamp DATETIME, EnclosingMethod VARCHAR(255), TargetType VARCHAR(255), ObjectUsage TEXT, IsQuery BOOLEAN)");
        }

        private static void EnsureDatabaseExists(string databaseFileName)
        {
            if (!File.Exists(databaseFileName))
            {
                SQLiteConnection.CreateFile(databaseFileName);
            }
        }

        private void CreateTable(string commandText)
        {
            new SQLiteCommand(commandText, _connection).ExecuteNonQuery();
        }

        public void Insert(string wp, DateTime timestamp, Context context, Query query)
        {
            var command = new SQLiteCommand(
                "INSERT INTO ContextHistories (WorkPeriod, Timestamp, TargetType, Context) VALUES (@wp, @timestamp, @targetType, @ctx)",
                _connection);
            command.Parameters.AddWithValue("@wp", wp);
            command.Parameters.AddWithValue("@timestamp", timestamp);
            command.Parameters.AddWithValue("@targetType", query.type);
            command.Parameters.AddWithValue("@ctx", context.ToCompactJson());
            command.ExecuteNonQuery();
        }

        public void Insert(string wp, DateTime timestamp, Query objectUsage, bool isQuery)
        {
            var command = new SQLiteCommand(
                "INSERT INTO OUHistories (WorkPeriod, Timestamp, EnclosingMethod, TargetType, ObjectUsage, IsQuery) VALUES (@wp, @timestamp, @em, @targetType, @ou, @isQuery)",
                _connection);
            command.Parameters.AddWithValue("@wp", wp);
            command.Parameters.AddWithValue("@timestamp", timestamp);
            command.Parameters.AddWithValue("@em", objectUsage.methodCtx.Name);
            command.Parameters.AddWithValue("@targetType", objectUsage.type.Name);
            command.Parameters.AddWithValue("@ou", objectUsage.ToCompactJson());
            command.Parameters.AddWithValue("@isQuery", isQuery);
            command.ExecuteNonQuery();
        }

        public IList<SSTSnapshot> GetContextHistories()
        {
            IList<SSTSnapshot> history = new List<SSTSnapshot>();
            var command = new SQLiteCommand("SELECT * FROM ContextHistories", _connection);
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var snapshot = new SSTSnapshot
                    {
                        WorkPeriodId = (string) result["WPId"],
                        Timestamp = (DateTime) result["Timestamp"],
                        Context = ((string) result["SST"]).ParseJsonTo<Context>()
                    };
                    history.Add(snapshot);
                }
            }
            return history;
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}