﻿using Dapper;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Enums;
using FomoAPI.Infrastructure.Exchanges;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repository to handle syncing symbol data from exchanges
    /// </summary>
    public class ExchangeSyncRepository : IExchangeSyncRepository
    {
        private string _connectionString;
        private readonly int _defaultBulkCopyBatchSize;

        public ExchangeSyncRepository(IOptionsMonitor<DbOptions> dbOptions)
        {
            _connectionString = dbOptions.CurrentValue.ConnectionString;
            _defaultBulkCopyBatchSize = dbOptions.CurrentValue.DefaultBulkCopyBatchSize;
        }


        public async Task<int> AddSymbols(IEnumerable<Symbol> symbols, int? batchSize = null)
        {
            if (batchSize == null)
            {
                batchSize = _defaultBulkCopyBatchSize;
            }

            var symbolDataTable = symbols.ToDataTable(
                new ColumnSchema<Symbol>("Id", typeof(int), (s => s.Id)),
                new ColumnSchema<Symbol>("Ticker", typeof(string), (s => s.Ticker)),
                new ColumnSchema<Symbol>("FullName", typeof(string), (s => s.FullName)),
                new ColumnSchema<Symbol>("ExchangeId", typeof(int), (s => s.ExchangeId)),
                new ColumnSchema<Symbol>("Delisted", typeof(bool), (s => s.Delisted))
             );

            symbolDataTable.TableName = "Symbol";

            var getSymbolCountSql = @"SELECT COUNT(*) FROM Symbol";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                int startCount = await connection.ExecuteScalarAsync<int>(getSymbolCountSql);

                // Begin bulk copy
                using (var transaction = connection.BeginTransaction())
                {
                    bool success = false;

                    try
                    {
                        using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "dbo.Symbol";
                            bulkCopy.BatchSize = batchSize.Value;
                            await bulkCopy.WriteToServerAsync(symbolDataTable);
                        }
                        await transaction.CommitAsync();
                        success = true;
                    }
                    finally
                    {
                        if (!success)
                        {
                            await transaction.RollbackAsync();
                        }
                    }
                }

                int endCount = await connection.ExecuteScalarAsync<int>(getSymbolCountSql);

                return endCount - startCount;
            }
        }

        public async Task<int> DelistSymbols(IEnumerable<int> symbolIds)
        {
            const string procedure = "dbo.DelistSymbols";

            var idDataTable = symbolIds.ToDataTable(new ColumnSchema<int>("Id", typeof(int), (id => id)));

            var tvpParam = new { tvpSymbolID = idDataTable.AsTableValuedParameter(TableType.IntIdType) };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var rowsUpdated = await connection.ExecuteAsync(procedure, tvpParam, transaction, commandType: CommandType.StoredProcedure);
                        await transaction.CommitAsync();

                        return rowsUpdated;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }     
        }

        public async Task<int> UpdateSymbols(IEnumerable<Symbol> symbols)
        {
            var dataTable = symbols.ToDataTable(
                    new ColumnSchema<Symbol>("Id", typeof(string), (s => s.Id)),
                    new ColumnSchema<Symbol>("FullName", typeof(string), (s => s.FullName))
                );

            var tvpParam = new { tvpUpdateSymbol = dataTable.AsTableValuedParameter(TableType.UpdateSymbolType) };

            const string procedure = "dbo.UpdateSymbols";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync()) 
                {
                    try
                    {
                        var rowsUpdated = await connection.ExecuteAsync(procedure, tvpParam, transaction, commandType: CommandType.StoredProcedure);
                        await transaction.CommitAsync();
                        return rowsUpdated;
                    }

                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<IReadOnlyDictionary<SymbolKey, Symbol>> GetExistingSymbols()
        {
            var selectSql = @"SELECT
                                Symbol.Id,
                                Symbol.Ticker,
                                Symbol.FullName,
                                Symbol.ExchangeId,
                                Exchange.Name
                              FROM
                                Symbol
                              INNER JOIN
                                Exchange
                              ON
                                Exchange.Id = Symbol.ExchangeId
                              WHERE
                                Symbol.Delisted = 0";

            IEnumerable<Symbol> symbols;

            using (var connection = new SqlConnection(_connectionString))
            {
                symbols = await connection.QueryAsync<Symbol>(selectSql);
            }

            return symbols.ToDictionary(s => new SymbolKey(s.Ticker, s.ExchangeId), s => s);
        }

        public async Task AddSyncHistory(string actionName, int symbolsChanged, string message, string error = null)
        {
            error = error != null ? error.Substring(0, 300) : null;

            var insertSql = @"INSERT INTO ExchangeSyncHistory (ActionName, Message, SymbolsChanged, Error, DateCreated)
                              VALUES (@actionName, @message, @symbolsChanged, @error, GETDATE())";


            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(insertSql, new { actionName, message, symbolsChanged, error});
            }
        }

        public async Task<ExchangeSyncSetting> GetSyncSettings()
        {
            var selectSql = @"SELECT TOP 1
	                            DisableSync,
                                DisableThresholds,
	                            InsertThresholdPercent,
	                            DeleteThresholdPercent,
	                            UpdateThresholdPercent,
                                Delimiter,
                                SuffixBlackList,
                                Url,
                                ClientName
                              FROM
                                ExchangeSyncSetting";

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QuerySingleAsync(selectSql);

                return new ExchangeSyncSetting
                {
                    DisableSync = result.DisableSync,
                    DisableThresholds = result.DisableThresholds,
                    InsertThresholdPercent = result.InsertThresholdPercent,
                    DeleteThresholdPercent = result.DeleteThresholdPercent,
                    UpdateThresholdPercent = result.UpdateThresholdPercent,
                    Delimiter = result.Delimiter,
                    SuffixBlackList = result.SuffixBlackList.ToString().Split(","),
                    Url = result.Url,
                    ClientName = result.ClientName
                };
            }
        }
    }
}
