using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Web.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.DomainServices.Client;

namespace Cotecna.Voc.Silverlight
{
    public static class StaticReferences
    {
        private static VocContext s_ctx;

        private static ObservableCollection<EntryPoint> s_entryPoints;
        private static ObservableCollection<Office> s_offices;

        public static ObservableCollection<EntryPoint> GetEntryPoints()
        {
            return GetEntryPoints(null);
        }

        public static ObservableCollection<Office> GetOffices()
        {
            return s_offices;
        }

        public static ObservableCollection<Office> GetRegionalOffices()
        {
            return s_offices.Where(x => x.OfficeType.GetValueOrDefault() == OfficeTypeEnum.RegionalOffice).ToObservableCollection();
        }

        public static ObservableCollection<Office> GetLocalOffices()
        {
            return s_offices.Where(x => x.OfficeType.GetValueOrDefault() == OfficeTypeEnum.LocalOffice).ToObservableCollection();
        }

        public static ObservableCollection<EntryPoint> GetEntryPoints(Action successfulAction)
        {
            if (s_ctx == null)
                s_ctx = new VocContext();

            if (s_entryPoints == null)
            {
                s_entryPoints = new ObservableCollection<EntryPoint>();
                s_offices = new ObservableCollection<Office>();
                s_ctx.Load(s_ctx.GetEntryPointsQuery().OrderBy(x => x.Name), LoadingEntryPoint, successfulAction);
            }
            else
            {
                if (successfulAction!=null)
                successfulAction();
            }

            return s_entryPoints;
        }

        public static void LoadingEntryPoint(LoadOperation<EntryPoint> op)
        {
            foreach (var item in op.Entities)
                s_entryPoints.Add(item);

            Action successfulAction=((Action)op.UserState);
            s_ctx.Load(s_ctx.GetOfficesQuery().OrderBy(x => x.OfficeId), LoadingOffice, successfulAction);

        }

        public static void LoadingOffice(LoadOperation<Office> op)
        {
            foreach (var item in op.Entities)
                s_offices.Add(item);
            
            Action successfulAction = ((Action)op.UserState);
            if (successfulAction != null)
                successfulAction();
        }

        public static void ReloadOffices(IEnumerable<Office> offices)
        {
            s_offices = null;
            s_offices = new ObservableCollection<Office>();
            foreach (var item in offices)
                s_offices.Add(item);
        }


    }
}
