﻿using Microsoft.EntityFrameworkCore;
using StockLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLibrary
{
    public class DataAccess
    {

        public static async Task<List<Fund>> GetActiveFunds()
        {
            using (var context = new SqliteContext())
            {
                return await context.Fund.Where(w => w.IsActive == true).ToListAsync();
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
