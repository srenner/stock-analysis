using Microsoft.EntityFrameworkCore;
using StockLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockLibrary
{
    public class DataAccess
    {

        public static void UpsertFund(Fund fund)
        {
            var fundInDb = GetEntityByID<Fund>(fund.Symbol);

            if(fundInDb == null)
            {
                AddEntity(fund);
            }
            else
            {
                UpdateEntity(fund);
            }

        }

        public static T AddEntity<T>(T entity) where T : class
        {
            using (var context = new SqliteContext())
            {

                context.Entry<T>(entity).State = EntityState.Added;
                context.SaveChanges();
                return entity;
            }

        }

        public static T UpdateEntity<T>(T entity) where T : class
        {
            using (var context = new SqliteContext())
            {

                context.Entry<T>(entity).State = EntityState.Modified;
                context.SaveChanges();
                return entity;
            }

        }

        public static bool DeleteEntity<T>(T entity) where T : class
        {
            using (var context = new SqliteContext())
            {
                context.Entry<T>(entity).State = EntityState.Deleted;
                context.SaveChanges();
                return true;
            }
                
        }

        public static T GetEntityByID<T>(object id) where T : class
        {
            using (var context = new SqliteContext())
            {
                return context.Set<T>().Find(id);
            }
                
        }

    }
}
