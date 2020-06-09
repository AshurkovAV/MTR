using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Core;
using Core.DataStructure;
using Core.Extensions;
using Core.Services;
using Medical.CoreLayer.Models.Common;
using Medical.DatabaseCore.Services.Cache;
using Medical.DatabaseCore.Services.Database;
using Microsoft.Windows.Controls.PropertyGrid.Attributes;
using DataModel;

namespace Medical.DatabaseCore.Services.Classifiers
{
    public class V002ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV002();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item {Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDPR, p.PRNAME)}));
                }
            }
            return data;
        }
    }

    public class V002CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {

            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V002>>)cache.Get("V002Cache");

                data.AddRange(
                    result.Data.Select(
                          p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDPR, p.PRNAME) }));
            }
            return data;
        }
    }

    public class V003ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV003();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDLIC, p.LICNAME) }));
                }
            }
            return data;
        }
    }

    public class V004ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV004();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDMSP, p.MSPNAME) }));
                }
            }
            return data;
        }
    }

    public class V005ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV005();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDPOL, p.POLNAME) }));
                }
            }
            return data;
        }
    }

    public class V005CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V005>>)cache.Get("V005Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.Id,
                            DisplayName = string.Format("{0} {1}", p.IDPOL, p.POLNAME)
                        }));
            }
            return data;
        }
    }

    public class V006aItemsSource
    {
        public IEnumerable<CommonTuple> GetValues()
        {
            var data = new List<CommonTuple>();
            using (var scope = Di.I.BeginLifetimeScope())
            {

                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV006();

                if (result.Success)
                {
                    
                    data.AddRange(
                        result.Data.OrderBy(p => p.IDUMP).Select(
                            p => new CommonTuple
                            {
                                ValueField = p.id,
                                DisplayField = string.Format("{0} {1}", p.IDUMP, p.UMPNAME),
                                DataField = Tuple.Create(new V012ItemsSource().GetValues(p.id), new V009ItemsSource().GetValues(p.id), new V023ItemsSource().GetValues(p.id), new V026ItemsSource().GetValues(p.id))
                            }));
                }
            }
            return data;
        }
    }

    public class V006aCacheItemsSource
    {
        public IEnumerable<CommonTuple> GetValues()
        {
            var data = new List<CommonTuple>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V006>>)cache.Get("V006Cache");

                data.AddRange(
                    result.Data.OrderBy(p => p.IDUMP).Select(
                            p => new CommonTuple
                            {
                                ValueField = p.id,
                                DisplayField = string.Format("{0} {1}", p.IDUMP, p.UMPNAME),
                                DataField = Tuple.Create(new V012ItemsSource().GetValues(p.id), new V009ItemsSource().GetValues(p.id), new V023CacheItemsSource().GetValues(p.id), new V026CacheItemsSource().GetValues(p.id)),
                                DataFieldV = Tuple.Create(new V023ItemsSource().GetValues(p.id))

                            }));
            }
            return data;
        }
    }

    public class V006ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV006();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.OrderBy(p => p.UMPNAME).Select(
                            p => new Item { Value = p.id, DisplayName = string.Format("{0} {1}", p.IDUMP, p.UMPNAME) }));
                }
            }
            return data;
        }
    }

    public class V008ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV008();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.OrderBy(p=>p.IDVMP).Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDVMP, p.VMPNAME) }));
                }
            }
            return data;
        }
    }

    public class V008CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V008>>)cache.Get("V008Cache");

                data.AddRange(
                    result.Data.Select(
                          p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDVMP, p.VMPNAME) }));
            }
            return data;
        }
    }

    public class V009ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV009();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDRMP, p.RMPNAME) }));
                }
            }
            return data;
        }

        public ItemCollection GetValues(int? condition = null)
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV009();
                if (result.Success)
                {
                    if (condition.HasValue) {
                        result.Data = result.Data.Where(p => p.DL_USLOV == condition).ToList();
                    }
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDRMP, p.RMPNAME) }));
                }
            }
            return data;
        }
    }

    public class V009CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V009>>)cache.Get("V009Cache");

                data.AddRange(
                    result.Data.Select(
                          p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDRMP, p.RMPNAME) }));
            }
            return data;
        }

        public ItemCollection GetValues(int? condition = null)
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V009>>) cache.Get("V009Cache");


                if (condition.HasValue)
                {
                    result.Data = result.Data.Where(p => p.DL_USLOV == condition).ToList();
                }
                data.AddRange(
                    result.Data.Select(
                        p => new Item {Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDRMP, p.RMPNAME)}));

            }
            return data;
        }
    }

    public class V010ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV010();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDSP, p.SPNAME) }));
                }
            }
            return data;
        }
    }

    public class V010CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V010>>)repo.Get("V010Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDSP, p.SPNAME) }));

            }
            return data;
        }
    }


    public class V020ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV020();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.IDK_PR, DisplayName = string.Format("{0} {1}", p.IDK_PR, p.K_PRNAME) }));
                }
            }
            return data;
        }
    }

    public class V020CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V020>>)cache.Get("V020Cache");

                data.AddRange(
                    result.Data.Select(
                          p => new Item { Value = p.IDK_PR, DisplayName = string.Format("{0} {1}", p.IDK_PR, p.K_PRNAME) }));
            }
            return data;
        }
    }

    public class V024CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {

            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V024>>)cache.Get("V024Cache");

                data.AddRange(
                    result.Data.Select(
                          p => new Item { Value = p.IDDKK, DisplayName = string.Format("{0} {1}", p.IDDKK, p.DKKNAME) }));
            }
            return data;
        }
    }

    public class F002ItemsSource
    {
        public IEnumerable<CommonTuple> GetValues()
        {
            var data = new List<CommonTuple>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF002();
                if (result.Success)
                {
                    var cache = scope.Resolve<ICacheRepository>();
                    var f010Cache = cache.Get(CacheRepository.F010OkatoToNameCache);
                    data.AddRange(
                        result.Data.OrderBy(p=>p.smocod).Select(
                            p => new CommonTuple
                            {
                                ValueField = p.Id,
                                DisplayField = string.Format("{0} {1}\r\n{2}", p.smocod, p.nam_smok, f010Cache.GetString(p.tf_okato.ToInt32Nullable()))
                            }));
                }
            }
            return data;
        }
    }

    public class ShareDoctorItemsSource
    {
        public IEnumerable<CommonTuple> GetValues()
        {
            var data = new List<CommonTuple>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetShareDoctor();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new CommonTuple
                            {
                                ValueField = p.DoctorId,
                                DisplayField = string.Format("{0} {1} {2} {3}", p.Code, p.Surname, p.DName, p.Patronymic)
                            }));
                }
            }
            return data;
        }
    }



    public class F003ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF003ByOkato(TerritoryService.TerritoryOkato);
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.mcod, p.nam_mok) }));
                }
            }
            return data;
        }
    }

    public class F003FullItemsSource 
    {
        public IEnumerable<CommonTuple> GetValues()
        {
            var data = new List<CommonTuple>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF003();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new CommonTuple
                            {
                                ValueField = p.Id,
                                DisplayField = string.Format("{0} {1}", p.mcod, p.nam_mok)
                            }));
                }
            }
            return data;
        }
    }

    public class F003FullToActItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F003>>) cache.Get("F003Cache");
                
                data.AddRange(
                    result.Data.Select(
                        p => new Item {Value = p.mcod, DisplayName = string.Format("{0} {1}", p.mcod, p.nam_mok)}));
            }
            return data;
        }
    }

    public class F002FullToActItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F002>>) cache.Get("F002Cache");
                data.AddRange(
                    result.Data.Select(
                        p => new Item
                            {Value = p.smocod, DisplayName = string.Format("{0} {1}", p.smocod, p.nam_smop)}));
            }

            return data;
        }
    }

    public class F003CacheFullItemsSource
    {
        public IEnumerable<CommonTuple> GetValues()
        {
            var data = new List<CommonTuple>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F003>>) repo.Get("F003Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new CommonTuple
                        {
                            ValueField = p.Id,
                            DisplayField = string.Format("{0} {1}", p.mcod, p.nam_mok)
                        }));
            }
            return data;
        }
    }

    public class F003CodeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF003ByOkato(TerritoryService.TerritoryOkato);
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.mcod, DisplayName = string.Format("{0} {1}", p.mcod, p.nam_mok) }));
                }
            }
            return data;
        }
    }

    public class F004ItemsSource
    {
        public IEnumerable<CommonTuple> GetValues()
        {
            var data = new List<CommonTuple>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetAll<F004>();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new CommonTuple
                            {
                                ValueField = p.Id,
                                DisplayField = string.Format("{0} {1} {2}, {3}", p.surname, p.fname, p.patronymic, p.position)
                            }));
                }
            }
            return data;
        }
    }

    public class F005ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF005();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDIDST, p.STNAME) }));
                }
            }
            return data;
        }
    }

    public class F005CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F005>>)repo.Get("F005Cache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDIDST, p.STNAME) }));

            }
            return data;
        }
    }

    public class F008ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF008();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDDOC, p.DOCNAME) }));
                }
            }
            return data;
        }
    }

    public class F008CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F008>>)repo.Get("F008Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDDOC, p.DOCNAME) }));

            }
            return data;
        }
    }

    public class F010ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF010();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.KOD_OKATO, DisplayName = string.Format("{0} {1}", p.KOD_TF, p.SUBNAME) }));
                }
            }
            return data;
        }
    }

    public class VidControltemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetVidControl();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.ControlCode, DisplayName = p.ControlName }));
                }
            }
            return data;
        }
    }


    public class ActExpertiStatusItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetActExpertiStatus();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.StatusCode, DisplayName = p.StatusName}));
                }
            }
            return data;
        }
    }

    public class F010CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F010>>)repo.Get("F010Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item { Value = p.KOD_OKATO, DisplayName = string.Format("{0} {1}", p.KOD_TF, p.SUBNAME) }));

            }
            return data;
        }
    }

    public class F010TfItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF010();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.KOD_TF, p.SUBNAME) }));
                }
            }
            return data;
        }
    }

    public class F010CacheTfItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F010>>)repo.Get("F010Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.KOD_TF, p.SUBNAME) }));
             
            }
            return data;
        }
    }

    public class F010TfasIdItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF010();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.KOD_TF.ToInt32(), DisplayName = string.Format("{0} {1}", p.KOD_TF, p.SUBNAME) }));
                }
            }
            return data;
        }
    }

    public class F011ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF011();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDDoc, p.DocName) }));
                }
            }
            return data;
        }
    }

    public class F011CaheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F011>>)repo.Get("F011Cache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item { Value = p.Id, DisplayName = string.Format("{0} {1}", p.IDDoc, p.DocName) }));

            }
            return data;
        }
    }

    public class M001ItemsSource
    {
        public IEnumerable<CommonItem> GetValues()
        {
            var data = new List<CommonItem>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetM001();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new CommonItem
                            {
                                ValueField = p.Id,
                                DisplayField = string.Format("{0} {1}", p.IDDS, p.DSNAME),
                                DataField = p.Payable == 0 
                            }));
                }
            }
            return data;
        }
    }

    public class M001CacheItemsSource
    {
        public IEnumerable<CommonItem> GetValues()
        {
            var data = new List<CommonItem>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<M001>>)cache.Get("M001Cache");

                data.AddRange(
                    result.Data.Select(
                           p => new CommonItem
                           {
                               ValueField = p.Id,
                               DisplayField = string.Format("{0} {1}", p.IDDS, p.DSNAME),
                               DataField = p.Payable == 0
                           }));
            }
            return data;
        }
    }

    public class M001_CacheItemsSource
    {
        public IEnumerable<CommonItem> GetValues()
        {
            var data = new List<CommonItem>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<M001>>)cache.Get("M001Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new CommonItem
                        {
                            ValueFieldStr = p.IDDS,
                            DisplayField = string.Format("{0} {1}", p.IDDS, p.DSNAME),
                            DataField = p.Payable == 0
                        }));
            }
            return data;
        }
    }

    public class V023ItemsSource
    {
        public IEnumerable<CommonItem> GetValues()
        {
            var data = new List<CommonItem>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV023();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.OrderBy(k=>k.K_KSG).Select(
                            p => new CommonItem
                            {
                                ValueFieldStr = p.K_KSG,
                                DisplayField = string.Format("{0} {1}", p.K_KSG, p.N_KSG)
                            }));
                }
            }
            return data;
        }

        public ItemCollection GetValues(int? condition = null)
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV023();
                if (result.Success)
                {
                    if (condition.HasValue)
                    {
                        result.Data = result.Data.Where(p => p.IDUMP == condition).ToList();
                    }
                    data.AddRange(
                        result.Data.Where(x=>x.DATEEND == null).Select(
                            p => new Item {Value = p.K_KSG, DisplayName = string.Format("{0} {1}", p.K_KSG, p.N_KSG)}));
                }
            }
            return data;
        }
    }

    public class V023CacheItemsSource
    {
        public IEnumerable<CommonItem> GetValues()
        {
            var data = new List<CommonItem>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V023>>)cache.Get("V023Cache");

                data.AddRange(
                        result.Data.OrderBy(k => k.K_KSG).Select(
                            p => new CommonItem
                            {
                                ValueFieldStr = p.K_KSG,
                                DisplayField = string.Format("{0} {1}", p.K_KSG, p.N_KSG)
                            }));
            }
            return data;
        }

        public ItemCollection GetValues(int? condition = null)
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V023>>)cache.Get("V023Cache");

                if (condition.HasValue)
                {
                    result.Data = result.Data.Where(p => p.IDUMP == condition && p.DATEEND.IsNotNull()).ToList();
                }
                data.AddRange(
                    result.Data.Where(x => x.DATEEND == null).Select(
                        p => new Item { Value = p.K_KSG, DisplayName = string.Format("{0} {1}", p.K_KSG, p.N_KSG) }));
            }
            return data;
        }
    }

    public class V026ItemsSource
    {
        public IEnumerable<CommonItem> GetValues()
        {
            var data = new List<CommonItem>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV026();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.OrderBy(k => k.K_KPG).Select(
                            p => new CommonItem
                            {
                                ValueFieldStr =  p.K_KPG,
                                DisplayField = string.Format("{0} {1}", p.K_KPG, p.N_KPG)
                            }));
                }
            }
            return data;
        }

        public ItemCollection GetValues(int? condition = null)
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV026();
                if (result.Success)
                {
                    if (condition.HasValue)
                    {
                        result.Data = result.Data.Where(p => p.IDUMP == condition).ToList();
                    }
                    data.AddRange(
                        result.Data.Where(x => x.DATEEND == null).Select(
                            p => new Item { Value = p.K_KPG, DisplayName = string.Format("{0} {1}", p.K_KPG, p.N_KPG) }));
                }
            }
            return data;
        }
    }

    public class V026CacheItemsSource
    {
        public IEnumerable<CommonItem> GetValues()
        {
            var data = new List<CommonItem>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V026>>)cache.Get("V026Cache");

                data.AddRange(
                        result.Data.OrderBy(k => k.K_KPG).Select(
                            p => new CommonItem
                            {
                                ValueFieldStr = p.K_KPG,
                                DisplayField = string.Format("{0} {1}", p.K_KPG, p.N_KPG)
                            }));
            }
            return data;
        }

        public ItemCollection GetValues(int? condition = null)
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V026>>)cache.Get("V026Cache");

                if (condition.HasValue)
                {
                    result.Data = result.Data.Where(p => p.IDUMP == condition).ToList();
                }
                data.AddRange(
                    result.Data.Where(x => x.DATEEND == null).Select(
                        p => new Item { Value = p.K_KPG, DisplayName = string.Format("{0} {1}", p.K_KPG, p.N_KPG) }));
            }
            return data;
        }
    }

    public class ScopeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalScope();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.ScopeID,
                                DisplayName = string.Format("{0} {1}", p.ScopeID, p.Name)
                            }));
                }
            }
            return data;
        }
    }

    public class ParamItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalParam();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.ParamID,
                                DisplayName = p.Name
                            }));
                }
            }
            return data;
        }
    }

    public class F006ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF006();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = string.Format("{0} {1}", p.IDVID, p.VIDNAME)
                            }));
                }
            }
            return data;
        }
    }

    public class F004CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F004>>)cache.Get("F004Cache");
                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.code,
                            DisplayName = string.Format("{0} {1} {2} {3} {4}", p.code, p.fname, p.surname, p.patronymic, p.speciality)
                        }).OrderBy(s => s.DisplayName));

            }
            return data;
        }
    }

    public class F006CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F006>>)cache.Get("F006Cache");
                data.AddRange(
                    result.Data.Where(f=>Constants.Mee.Contains((int?)f.IDVID)).Select(
                        p => new Item
                        {
                            Value = p.Id,
                            DisplayName = string.Format("{0} {1}", p.IDVID, p.VIDNAME)
                        }).OrderBy(s => s.DisplayName));

            }
            return data;
        }
    }

    public class F006CacheEqmaItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F006>>)cache.Get("F006Cache");
                data.AddRange(
                    result.Data.Where(f => Constants.Eqma.Contains((int?)f.IDVID)).Select(
                        p => new Item
                        {
                            Value = p.Id,
                            DisplayName = string.Format("{0} {1}", p.IDVID, p.VIDNAME)
                        }).OrderBy(s => s.DisplayName));

            }
            return data;
        }
    }

    public class F014ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF014();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = string.Format("{0} {1}", p.Osn, p.Comments)
                            }).OrderBy(s=>s.DisplayName));
                }
            }
            return data;
        }
    }

    public class DirectionViewItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetDirectionView();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = string.Format("{0} {1}", p.Id, p.Name)
                            }).OrderBy(s => s.DisplayName));
                }
            }
            return data;
        }
    }

    public class MetIsslItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetMetIssl();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = string.Format("{0} {1}", p.Id, p.Name)
                            }).OrderBy(s => s.DisplayName));
                }
            }
            return data;
        }
    }

    public class N007CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N007>>) cache.Get("N007Cache");
                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_Mrf,
                            DisplayName = string.Format("{0} {1}", p.ID_Mrf, p.Mrf_NAME)
                        }).OrderBy(s => s.DisplayName));

            }
            return data;
        }
    }

    public class N008CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N008>>)cache.Get("N008Cache");
                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_R_M,
                            DisplayName = string.Format("{0} {1}", p.ID_R_M, p.R_M_NAME)
                        }).OrderBy(s => s.DisplayName));

            }
            return data;
        }
    }

    public class N011CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N011>>)cache.Get("N011Cache");
                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_R_I,
                            DisplayName = string.Format("{0} {1}", p.ID_R_I, p.R_I_NAME)
                        }).OrderBy(s => s.DisplayName));

            }
            return data;
        }
    }

    public class N010CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N010>>)cache.Get("N010Cache");
                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_Igh,
                            DisplayName = string.Format("{0} {1}", p.ID_Igh, p.Igh_NAME)
                        }).OrderBy(s => s.DisplayName));

            }
            return data;
        }
    }

    public class N020CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N020>>)cache.Get("N020Cache");
                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_LEKP,
                            DisplayName = string.Format("{0} {1}", p.ID_LEKP, p.MNN)
                        }).OrderBy(s => s.Value));

            }
            return data;
        }
    }

    public class F014CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<F014>>)cache.Get("F014Cache");

                data.AddRange(
                    result.Data.Select(
                           p => new Item
                           {
                               Value = p.Id,
                               DisplayName = string.Format("{0} {1}", p.Osn, p.Comments)
                           }).OrderBy(s => s.DisplayName));
            }
            return data;
        }
    }

    public class F014OsnItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetF014();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Osn,
                                DisplayName = string.Format("{0} {1}", p.Osn, p.Comments)
                            }));
                }
            }
            return data;
        }
    }

    public class UserItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetUsers();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.UserID,
                                DisplayName = string.Format("{0} {1} {2} ({3})", p.LastName, p.FirstName, p.Patronymic, p.Login)
                            }));
                }
            }
            return data;
        }
    }

    public class RoleItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetRoles();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.RoleID,
                                DisplayName = string.Format("{0} {1}", p.RoleID, p.Name)
                            }));
                }
            }
            return data;
        }
    }


    public class ExaminationTypeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalExaminationType();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.ExaminationTypeID,
                                DisplayName = p.Name
                            }));
                }
            }
            return data;
        }
    }

    public class ReportTypeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalReportType();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.ReportTypeID,
                                DisplayName = p.Name
                            }));
                }
            }
            return data;
        }
    }

    public class LocalEmployeeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetLocalEmployee();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.EmployeeId,
                                DisplayName = string.Format("{0} {1} {2} - {3}", p.Surname, p.EName, p.Patronymic, p.Position)
                            }));
                }
            }
            return data;
        }
    }

    public class OldProfileItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalOldProfile();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.OldProfileId,
                                DisplayName = string.Format("{0} {1}", p.Code, p.Name)
                            }));
                }
            }
            return data;
        }
    }

    public class ParticularSignItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalParticularSign();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.ParticularSignId,
                                DisplayName = string.Format("{0} {1}", p.ParticularSignId, p.ParticularSignName) 
                            }));
                }
            }
            return data;
        }
    }

    public class PaymentStatusItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalPaymentStatus();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.PaymentStatusId,
                                DisplayName = string.Format("{0} {1}", p.StatusCode, p.StatusName)
                            }));
                }
            }
            return data;
        }
    }


    public class PositionTypeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            data.AddRange( new List<Item>
            {
               new Item { Value = 0, DisplayName = "Руководитель" },
               new Item { Value = 1, DisplayName = "Главный бухгалтер" }
            });
               
            return data;
        }
    }

    public class HospitalizationTypeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            data.AddRange(new List<Item>
            {
               new Item { Value = 1m, DisplayName = "{0} {1}".F( 1, "Плановая") },
               new Item { Value = 2m, DisplayName = "{0} {1}".F( 2, "Экстренная") }
            });

            return data;
        }
    }

    public class DiagTypeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            data.AddRange(new List<Item>
            {
               new Item { Value = 1, DisplayName = "{0} {1}".F( 1, "Гистологический признак") },
               new Item { Value = 2, DisplayName = "{0} {1}".F( 2, "Маркёр (ИГХ)") }
            });

            return data;
        }
    }

    public class EventTypeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            data.AddRange(new List<Item>
            {
               new Item { Value = 1, DisplayName = "{0} {1}".F( 1, "Посещение по поводу заболевания") },
               new Item { Value = 2, DisplayName = "{0} {1}".F( 2, "Профилактическое посещение") },
               new Item { Value = 3, DisplayName = "{0} {1}".F( 3, "Неотложная помощь") }
            });

            return data;
        }
    }

    public class PcelItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalPcel();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = string.Format("{0} {1}", p.Id, p.Name)
                            }));
                }
            }

            return data;
        }
    }

    public class MedicalAccountItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetMedicalAccountView();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.MedicalAccountId,
                                DisplayName = string.Format("Счет МО № {0} от {1}", p.AccountNumber, p.AccountDate)
                            }));
                }
            }
            return data;
        }
    }

    public class KslpItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalKslp();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.IDSL,
                                DisplayName = string.Format("{0} {1}", p.IDSL, p.USLKOEF)
                            }));
                }
            }

            return data;
        }
    }

    public class DnItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            data.AddRange(new List<Item>
            {
               new Item { Value = 1, DisplayName = "{0} {1}".F( 1, "Состоит") },
               new Item { Value = 2, DisplayName = "{0} {1}".F( 2, "Взят") },
               new Item { Value = 4, DisplayName = "{0} {1}".F( 4, "Снят по причине выздоровления") },
               new Item { Value = 6, DisplayName = "{0} {1}".F( 6, "Снят по другим причинам") }
            });

            return data;
        }
    }

    public class DsItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            data.AddRange(new List<Item>
            {
                new Item { Value = 1, DisplayName = "{0} {1}".F( 1, "Первичный диагноз") },
                new Item { Value = 2, DisplayName = "{0} {1}".F( 2, "Диагноз сопутствующего заболевания") },
                new Item { Value = 3, DisplayName = "{0} {1}".F( 3, "Диагноз осложнения заболевания") },
            });

            return data;
        }
    }

    public class VersionItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalVersion();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.VersionID,
                                DisplayName = p.Version
                            }));
                }
            }
            return data;
        }
    }

    public class ExaminationGroupItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalExaminationGroup();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.ExaminationGroupID,
                                DisplayName = p.Name
                            }));
                }
            }
            return data;
        }
    }

    public class V012ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV012();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = "{0} {1}".F(p.IDIZ, p.IZNAME)
                            }));
                }
            }
            return data;
        }

        public ItemCollection GetValues(int? condition = null)
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV012();
                if (result.Success)
                {
                    if (condition.HasValue) {
                        result.Data = result.Data.Where(p => p.DL_USLOV == condition).ToList();
                    }

                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = "{0} {1}".F(p.IDIZ, p.IZNAME)
                            }));
                }
            }
            return data;
        }
    }

    public class V012CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V012>>)cache.Get("V012Cache");

                data.AddRange(
                    result.Data.Select(
                          p => new Item
                          {
                              Value = p.Id,
                              DisplayName = "{0} {1}".F(p.IDIZ, p.IZNAME)
                          }));
            }
            return data;
        }

        public ItemCollection GetValues(int? condition = null)
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V012>>)cache.Get("V012Cache");
                if (condition.HasValue)
                {
                    result.Data = result.Data.Where(p => p.DL_USLOV == condition).ToList();
                }

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.Id,
                            DisplayName = "{0} {1}".F(p.IDIZ, p.IZNAME)
                        }));
            }
            return data;
        }
    }

    public class V014ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV014();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = "{0} {1}".F(p.IDFRMMP, p.FRMMPNAME)
                            }));
                }
            }
            return data;
        }
    }

    public class V014CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V014>>)cache.Get("V014Cache");

                data.AddRange(
                    result.Data.Select(
                          p => new Item
                          {
                              Value = p.Id,
                              DisplayName = "{0} {1}".F(p.IDFRMMP, p.FRMMPNAME)
                          }));
            }
            return data;
        }
    }
    public class V015ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV015();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = "{0} {1}".F(p.CODE, p.NAME)
                            }));
                }
            }
            return data;
        }
    }

    public class V021ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV021();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = "{0} {1}".F(p.IDSPEC, p.SPECNAME)
                            }));
                }
            }
            return data;
        }
    }

    public class V021CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V021>>)cache.Get("V021Cache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item
                         {
                             Value = p.Id,
                             DisplayName = "{0} {1}".F(p.IDSPEC, p.SPECNAME)
                         }));
            }
            return data;
        }
    }

    public class V027CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V027>>)cache.Get("V027Cache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item
                         {
                             Value = p.Id,
                             DisplayName = "{0} {1}".F(p.IDCZ, p.N_CZ)
                         }));
            }
            return data;
        }
    }

    public class V028CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V028>>)cache.Get("V028Cache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item
                         {
                             Value = p.Id,
                             DisplayName = "{0} {1}".F(p.IDVN, p.N_VN)
                         }));
            }
            return data;
        }
    }

    public class V029CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V029>>)cache.Get("V029Cache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item
                         {
                             Value = p.Id,
                             DisplayName = "{0} {1}".F(p.IDMET, p.N_MET)
                         }));
            }
            return data;
        }
    }

    public class RefusalSourceItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalRefusalSource();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.RefusalSourceID,
                                DisplayName = "{0} {1}".F(p.Code, p.Name)
                            }));
                }
            }
            return data;
        }
    }

    public class ProcessingTypeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetGlobalProcessingType();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.ProcessingTypeId,
                                DisplayName = "{0} {1}".F(p.Code, p.Name)
                            }));
                }
            }
            return data;
        }
    }

    public class RegionalAttributeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetAll<globalRegionalAttribute>();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.ID,
                                DisplayName = "{0} {1}".F(p.ID, p.Name)
                            }));
                }
            }
            return data;
        }
    }

    public class RegionalAttributeCacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<globalRegionalAttribute>>)repo.Get("GlobalRegionalAttributeCache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item
                         {
                             Value = p.ID,
                             DisplayName = "{0} {1}".F(p.ID, p.Name)
                         }));

            }
            return data;
        }
    }

    public class CommonClass
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class DsOnktItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            var result = new List<CommonClass>
            {
                new CommonClass {ID = 0, Name = "при отсутствии подозрения  на злокачественное новообразование"},
                new CommonClass {ID = 1, Name = "при выявлении подозрения  на злокачественное новообразование"}
            };
            data.AddRange(
                result.Select(
                    p => new Item
                    {
                        Value = p.ID,
                        DisplayName = "{0} {1}".F(p.ID, p.Name)
                    }));

            return data;
        }
    }

    public class N018CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N018>>)repo.Get("N018Cache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item
                         {
                             Value = p.ID_REAS,
                             DisplayName = "{0} {1}".F(p.ID_REAS, p.REAS_NAME)
                         }));

            }
            return data;
        }
    }

    public class N019CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N019>>)repo.Get("N019Cache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item
                         {
                             Value = p.ID_CONS,
                             DisplayName = "{0} {1}".F(p.ID_CONS, p.CONS_NAME)
                         }));

            }
            return data;
        }
    }

    public class V025ItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetV025();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.IDPC,
                                DisplayName = "{0} {1}".F(p.IDPC, p.N_PC)
                            }));
                }
            }
            return data;
        }
    }

    public class V025CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<V025>>)repo.Get("V025Cache");

                data.AddRange(
                    result.Data.Select(
                         p => new Item
                         {
                             Value = p.IDPC,
                             DisplayName = "{0} {1}".F(p.IDPC, p.N_PC)
                         }));

            }
            return data;
        }
    }

    public class N001CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N001>>)repo.Get("N001Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_PrOt,
                            DisplayName = "{0} {1}".F(p.ID_PrOt, p.PrOt_NAME)
                        }));

            }
            return data;
        }
    }

    public class N002CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N002>>) repo.Get("N002Cache");
                
                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_St,
                            DisplayName = "{0} {1} - {2}".F(p.ID_St, p.DS_St, p.KOD_St)
                        }));

            }
            return data;
        }
    }

    public class N003CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N003>>)repo.Get("N003Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_T,
                            DisplayName = "{0} {1} - {2}".F(p.DS_T, p.KOD_T, p.T_NAME)
                        }));

            }
            return data;
        }
    }

    public class N004CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N004>>)repo.Get("N004Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_N,
                            DisplayName = "{0} {1} - {2}".F(p.DS_N, p.KOD_N, p.N_NAME)
                        }));

            }
            return data;
        }
    }

    public class N005CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N005>>)repo.Get("N005Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_M,
                            DisplayName = "{0} {1} - {2}".F(p.DS_M, p.KOD_M, p.M_NAME)
                        }));

            }
            return data;
        }
    }

    public class PrConsCacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<globalConsultationOnk>>)repo.Get("ConsultationOnkCache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.Id,
                            DisplayName = "{0} {1}".F(p.Id, p.Name)
                        }));

            }
            return data;
        }
    }

    public class N013CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N013>>)repo.Get("N013Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_TLech,
                            DisplayName = "{0} {1}".F(p.ID_TLech, p.TLech_NAME)
                        }));

            }
            return data;
        }
    }

    public class N014CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N014>>)repo.Get("N014Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_THir,
                            DisplayName = "{0} {1}".F(p.ID_THir, p.THir_NAME)
                        }));

            }
            return data;
        }
    }

    public class N015CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N015>>)repo.Get("N015Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_TLek_L,
                            DisplayName = "{0} {1}".F(p.ID_TLek_L, p.TLek_NAME_L)
                        }));                                 

            }
            return data;
        }
    }

    public class N016CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N016>>)repo.Get("N016Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_TLek_V,
                            DisplayName = "{0} {1}".F(p.ID_TLek_V, p.TLek_NAME_V)
                        }));

            }
            return data;
        }
    }

    public class N017CacheItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<ICacheRepository>();
                var result = (DictionaryCache<IEnumerable<N017>>)repo.Get("N017Cache");

                data.AddRange(
                    result.Data.Select(
                        p => new Item
                        {
                            Value = p.ID_TLuch,
                            DisplayName = "{0} {1}".F(p.ID_TLuch, p.TLuch_NAME)
                        }));

            }
            return data;
        }
    }

    public class HealthGroupItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetAll<V017>();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = "{0} {1}".F(p.IDDR, p.DRNAME)
                            }));
                }
            }
            return data;
        }
    }


    public class JobStatusItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var data = new ItemCollection();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetAll<F009>();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new Item
                            {
                                Value = p.Id,
                                DisplayName = "{0} {1}".F(p.IDStatus, p.StatusName)
                            }));
                }
            }
            return data;
        }
    }

    public class GlobalEqmaOutcomesItemsSource
    {
        public IEnumerable<CommonTuple> GetValues()
        {
            var data = new List<CommonTuple>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IMedicineRepository>();
                var result = repo.GetAll<globalEqmaOutcomes>();
                if (result.Success)
                {
                    data.AddRange(
                        result.Data.Select(
                            p => new CommonTuple
                            {
                                ValueField = p.Id,
                                DisplayField = string.Format("{0} {1}", p.Id, p.OutcomeName)
                            }));
                }
            }
            return data;
        }
    }
    
}
