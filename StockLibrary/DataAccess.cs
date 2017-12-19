using Microsoft.EntityFrameworkCore;
using StockLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLibrary
{
    public class DataAccess
    {

        public static async  Task<List<DateTime>> GetAllDates()
        {
            List<DateTime> dates = new List<DateTime>();
            using (var context = new SqliteContext())
            {
                var conn = context.Database.GetDbConnection();
                try {
                    
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand())
                    {
                        string query = "select distinct FundDayDate from FundDay order by FundDayDate";
                        command.CommandText = query;
                        DbDataReader reader = await command.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                dates.Add(reader.GetDateTime(0));
                            }
                        }
                        reader.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
            return dates;
        }
                
        public static async Task AddCorrelatedIncrease(CorrelatedIncrease increase)
        {
            using (var context = new SqliteContext())
            {
                var increaseInDb = context.CorrelatedIncrease
                    .Where(w => w.PrimaryFundDayID == increase.PrimaryFundDayID)
                    .Where(w => w.SecondaryFundDayID == increase.SecondaryFundDayID)
                    .FirstOrDefault();

                if(increaseInDb == null || increaseInDb.CorrelatedIncreaseID == 0)
                {
                    await AddEntity(increase);
                }
            }
        }

        public static async Task<List<CorrelatedIncrease>> GetCorrelatedIncreases(string primarySymbol)
        {
            using (var context = new SqliteContext())
            {
                return await context.CorrelatedIncrease
                    .Include(i => i.PrimaryFundDay)
                    .Include(i => i.SecondaryFundDay)
                    .Where(w => w.PrimaryFundDay.Symbol == primarySymbol)
                    .ToListAsync();
            }
        }

        public static async Task<List<Fund>> GetActiveFunds()
        {
            using (var context = new SqliteContext())
            {
                return await context.Fund.Where(w => w.IsActive == true).ToListAsync();
            }
        }

        public static async void SetFundInactive(string symbol)
        {
            using (var context = new SqliteContext())
            {
                var fund = context.Fund.Where(w => w.Symbol == symbol).FirstOrDefault();
                if(fund != null)
                {
                    fund.IsActive = false;
                    await UpdateEntity(fund);
                }
            }
        }

        public static async Task UpsertFund(Fund fund)
        {
            var fundInDb = GetEntityByID<Fund>(fund.Symbol);

            if(fundInDb == null)
            {
                await AddEntity(fund);
            }
            else
            {
                await UpdateEntity(fund);
            }
        }

        public static async Task AddFundDays(List<FundDay> days, DateTime? earliestDate = null)
        {
            if(!earliestDate.HasValue)
            {
                earliestDate = DateTime.MinValue;
            }
            using (var context = new SqliteContext())
            {
                foreach(var day in days)
                {
                    var dayInDb = context.FundDay.Where(w => w.Symbol == day.Symbol && w.FundDayDate == day.FundDayDate).FirstOrDefault();
                    if(dayInDb == null && day.FundDayDate >= earliestDate)
                    {
                        day.Delta = ((day.Close - day.Open) / day.Open);
                        await AddEntity(day);
                    }
                }
            }
        }

        public static async Task UpdateFundDayDelta(string symbol, decimal delta)
        {
            using (var context = new SqliteContext())
            {
                string sql = "UPDATE FundDay SET Delta = {0} WHERE Symbol = {1}";
                await context.Database.ExecuteSqlCommandAsync(sql, symbol, delta);
            }
        }

        public static async Task<List<FundDay>> GetAllFundDays()
        {
            using (var context = new SqliteContext())
            {
                return await context.FundDay.ToListAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentIncrease">0.01 = 1%, 1.0 = 100%</param>
        /// <returns></returns>
        public static async Task<List<FundDay>> GetGoodFundDays(decimal percentIncrease, DateTime fromDate)
        {
            using (var context = new SqliteContext())
            {
                return await context.FundDay
                    .Where(w => w.Delta >= percentIncrease)
                    .Where(w => w.FundDayDate >= fromDate)
                    .ToListAsync();
            }
        }

        public static List<Fund> GetFundsWithoutData()
        {
            using (var context = new SqliteContext())
            {
                return context.Fund.FromSql("SELECT f.* FROM Fund f LEFT OUTER JOIN FundDay fd ON f.Symbol = fd.Symbol WHERE fd.Symbol is null and f.IsActive = 1").ToList();
            }
        }

        public static async Task<T> AddEntity<T>(T entity) where T : class
        {
            using (var context = new SqliteContext())
            {

                context.Entry<T>(entity).State = EntityState.Added;
                await context.SaveChangesAsync();
                return entity;
            }

        }

        public static async Task<T> UpdateEntity<T>(T entity) where T : class
        {
            using (var context = new SqliteContext())
            {

                context.Entry<T>(entity).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return entity;
            }

        }

        public static async Task<bool> DeleteEntity<T>(T entity) where T : class
        {
            using (var context = new SqliteContext())
            {
                context.Entry<T>(entity).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                return true;
            }
                
        }

        public static async Task<T> GetEntityByID<T>(object id) where T : class
        {
            using (var context = new SqliteContext())
            {
                return await context.Set<T>().FindAsync(id);
            }
                
        }

    }
}
