using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ionx.GatewayAnalytics
{
    using Ionx.Entity.GatewayAnalytics.Data;
    using System.Transactions;

    public class PwgAnalyticsHandler
    {
        public List<Gateway> GetGatewayAnalytics()
        {
            try
            {
                var transactionOptions = new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                using (var analytics = new GatewayAnalyticsEntities())
                {
                    analytics.Configuration.LazyLoadingEnabled = false;
                    return analytics.Gateways.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<Gateway>();
            }
        }

        public Gateway GetGatewayAnalytics(Guid gatewayGuid)
        {
            try
            {
                var transactionOptions = new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                using (var analytics = new GatewayAnalyticsEntities())
                {
                    analytics.Configuration.LazyLoadingEnabled = false;
                    return analytics.Gateways
                        .Include("PerformanceMetric")
                        .Include("Disks")
                        .Include("LatestEvents")
                        .Include("Services")
                        .SingleOrDefault(g => g.GatewayGuid == gatewayGuid);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public SaveResult SaveGatewayAnalytics(Gateway gateway)
        {
            if (gateway == null || gateway.GatewayGuid == Guid.Empty)
            {
                return SaveResult.Failure;
            }

            try
            {
                using (var analytics = new GatewayAnalyticsEntities())
                {
                    // Find or create gateway, then update fields
                    var gatewayEntity = FindOrCreateGateway(analytics, gateway.GatewayGuid);
                    gatewayEntity.TeamViewerID = gateway.TeamViewerID;
                    gatewayEntity.MachineName = gateway.MachineName;
                    gatewayEntity.RunningSince = gateway.RunningSince;
                    gatewayEntity.OperatingSystem = gateway.OperatingSystem;
                    gatewayEntity.CpuInfo = gateway.CpuInfo;
                    gatewayEntity.MemorySize = gateway.MemorySize;
                    gatewayEntity.Latitude = gateway.Latitude;
                    gatewayEntity.Longitude = gateway.Longitude;
                    gatewayEntity.LocationDate = gateway.LocationDate;
                    gatewayEntity.LastReportDate = DateTime.UtcNow;

                    // Find or create performance metrics, then update fields
                    if (gateway.PerformanceMetric != null)
                    {
                        var performanceMetricEntity = FindOrCreatePerformanceMetric(analytics, gateway.GatewayGuid);
                        performanceMetricEntity.Gateway = gatewayEntity;
                        performanceMetricEntity.TotalEventsProcessed = gateway.PerformanceMetric.TotalEventsProcessed;
                        performanceMetricEntity.EventsProcessedPerSec = gateway.PerformanceMetric.EventsProcessedPerSec;
                        performanceMetricEntity.CurrentSignalQuality = gateway.PerformanceMetric.CurrentSignalQuality;
                        performanceMetricEntity.AverageSignalQuality = gateway.PerformanceMetric.AverageSignalQuality;
                        performanceMetricEntity.Reliability = gateway.PerformanceMetric.Reliability;
                        performanceMetricEntity.Stability = gateway.PerformanceMetric.Stability;
                        performanceMetricEntity.Latency = gateway.PerformanceMetric.Latency;
                        performanceMetricEntity.ConnectedNodes = gateway.PerformanceMetric.ConnectedNodes;
                    }

                    // Find or create services, then update fields
                    if (gateway.Services != null)
                    {
                        foreach (var service in gateway.Services)
                        {
                            var serviceEntity = FindOrCreateService(analytics, gateway.GatewayGuid, service.ServiceName);
                            serviceEntity.Gateway = gatewayEntity;
                            serviceEntity.ServiceName = service.ServiceName;
                            serviceEntity.Status = service.Status;
                        }
                    }

                    // Find or create disks, then update fields
                    if (gateway.Disks != null)
                    {
                        foreach (var disk in gateway.Disks)
                        {
                            var diskEntity = FindOrCreateDisk(analytics, gateway.GatewayGuid, disk.Label);
                            diskEntity.Gateway = gatewayEntity;
                            diskEntity.Label = disk.Label;
                            diskEntity.TotalSpace = disk.TotalSpace;
                            diskEntity.UsedSpace = disk.UsedSpace;
                        }
                    }

                    // Update latest events, if applicable
                    if (gateway.LatestEvents != null)
                    {
                        BulkUpdateLatestEvents(analytics, gateway.GatewayGuid, gateway.LatestEvents);
                    }

                    analytics.SaveChanges();
                    return SaveResult.Success;
                }
            }
            catch (Exception ex)
            {
                // TODO Logging
                return SaveResult.Failure;
            }
        }

        private Gateway FindOrCreateGateway(GatewayAnalyticsEntities analytics, Guid gatewayGuid)
        {
            var gatewayEntity = analytics.Gateways.Find(gatewayGuid);

            if (gatewayEntity == null)
            {
                gatewayEntity = new Gateway();
                gatewayEntity.GatewayGuid = gatewayGuid;
                analytics.Gateways.Add(gatewayEntity);
            }

            return gatewayEntity;
        }

        private PerformanceMetric FindOrCreatePerformanceMetric(GatewayAnalyticsEntities analytics, Guid gatewayGuid)
        {
            var performanceMetricEntity = analytics.PerformanceMetrics.Find(gatewayGuid);

            if (performanceMetricEntity == null)
            {
                performanceMetricEntity = new PerformanceMetric();
                analytics.PerformanceMetrics.Add(performanceMetricEntity);
            }

            return performanceMetricEntity;
        }

        private Service FindOrCreateService(GatewayAnalyticsEntities analytics, Guid gatewayGuid, string serviceName)
        {
            var serviceEntity = analytics.Services.SingleOrDefault(s => s.GatewayGuid == gatewayGuid && s.ServiceName == serviceName);

            if (serviceEntity == null)
            {
                serviceEntity = new Service();
                analytics.Services.Add(serviceEntity);
            }

            return serviceEntity;
        }

        private Disk FindOrCreateDisk(GatewayAnalyticsEntities analytics, Guid gatewayGuid, string diskLabel)
        {
            var diskEntity = analytics.Disks.SingleOrDefault(d => d.GatewayGuid == gatewayGuid && d.Label == diskLabel);

            if (diskEntity == null)
            {
                diskEntity = new Disk();
                analytics.Disks.Add(diskEntity);
            }

            return diskEntity;
        }

        private void BulkUpdateLatestEvents(GatewayAnalyticsEntities analytics, Guid gatewayGuid, ICollection<LatestEvent> latestEvents)
        {
            analytics.LatestEvents.RemoveRange(analytics.LatestEvents.Where(le => le.GatewayGuid == gatewayGuid));

            foreach (var e in latestEvents)
            {
                analytics.LatestEvents.Add(new LatestEvent
                    {
                        GatewayGuid = gatewayGuid,
                        EventDateUtc = e.EventDateUtc,
                        AssetName = e.AssetName,
                        EventName = e.EventName,
                        EventInfo = e.EventInfo
                    }
                );
            }
        }
    }
}
