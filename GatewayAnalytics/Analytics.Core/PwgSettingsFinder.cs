using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ionx.GatewayAnalytics
{
    using Ionx.Entity.GatewayAnalytics.Settings;

    public class PwgSettingsFinder
    {
        public AnalyticsConfig FindSettings()
        {
            var transactionOptions = new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            using (var settings = new GatewaySettingsEntities())
            {
                return settings.AnalyticsConfigs.FirstOrDefault();
            }
        }
    }
}
