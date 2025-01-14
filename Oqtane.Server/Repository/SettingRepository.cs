using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oqtane.Models;
using Oqtane.Shared;

namespace Oqtane.Repository
{
    public class SettingRepository : ISettingRepository
    {
        private TenantDBContext _tenant;
        private MasterDBContext _master;

        public SettingRepository(TenantDBContext tenant, MasterDBContext master)
        {
            _tenant = tenant;
            _master = master;
        }

        public IEnumerable<Setting> GetSettings(string entityName)
        {
            if (IsMaster(entityName))
            {
                return _master.Setting.Where(item => item.EntityName == entityName);
            }
            else
            {
                return _tenant.Setting.Where(item => item.EntityName == entityName);
            }
        }

        public IEnumerable<Setting> GetSettings(string entityName, int entityId)
        {
            var settings = GetSettings(entityName);
            return settings.Where(item => item.EntityId == entityId);
        }

        public Setting AddSetting(Setting setting)
        {
            if (IsMaster(setting.EntityName))
            {
                _master.Setting.Add(setting);
                _master.SaveChanges();
            }
            else
            {
                _tenant.Setting.Add(setting);
                _tenant.SaveChanges();
            }
            return setting;
        }

        public Setting UpdateSetting(Setting setting)
        {
            if (IsMaster(setting.EntityName))
            {
                _master.Entry(setting).State = EntityState.Modified;
                _master.SaveChanges();
            }
            else
            {
                _tenant.Entry(setting).State = EntityState.Modified;
                _tenant.SaveChanges();
            }
            return setting;
        }

        public Setting GetSetting(string entityName, int settingId)
        {
            if (IsMaster(entityName))
            {
                return _master.Setting.Find(settingId);
            }
            else
            {
                return _tenant.Setting.Find(settingId);
            }
        }

        public void DeleteSetting(string entityName, int settingId)
        {
            if (IsMaster(entityName))
            {
                Setting setting = _master.Setting.Find(settingId);
                _master.Setting.Remove(setting);
                _master.SaveChanges();
            }
            else
            {
                Setting setting = _tenant.Setting.Find(settingId);
                _tenant.Setting.Remove(setting);
                _tenant.SaveChanges();
            }
        }

        private bool IsMaster(string EntityName)
        {
            return (EntityName == EntityNames.ModuleDefinition || EntityName == EntityNames.Host);
        }
    }
}
