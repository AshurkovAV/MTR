using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using BLToolkit.Data.DataProvider;
using BLToolkit.Data.Linq;
using BLToolkit.Mapping;
using Core.Infrastructure;
using Core.Linq;
using DataModel;

namespace Medical.DatabaseCore.Services.Database
{
    public class TortillaRepository : ITortillaRepository
    {
        private readonly string _defaultProvider;
        private readonly string _defaultName;

        public TortillaRepository()
        {
            _defaultProvider = ProviderName.MsSql;
            _defaultName = AppRemoteSettings.TortillaConnectionString;
        }

        private TortillaContext CreateContext()
        {
            return new TortillaContext(_defaultProvider,_defaultName);
        }

        public TransactionResult AddSrzQuery(SrzQueryModel model)
        {
            var result = new TransactionResult();
            try
            {
                var tmpModel = Map.ObjectToObject<ZP1msg>(model);
                tmpModel.id = 0;
                const string none = "none";

                tmpModel.guid = string.IsNullOrWhiteSpace(tmpModel.guid) ? none : tmpModel.guid;
                //FIX pack_guid должен быть пустой строкой
                tmpModel.pack_guid = "";
                tmpModel.enp = string.IsNullOrWhiteSpace(tmpModel.enp) ? none : tmpModel.enp;
                tmpModel.passp = string.IsNullOrWhiteSpace(tmpModel.passp) ? none : tmpModel.passp;
                tmpModel.pen = string.IsNullOrWhiteSpace(tmpModel.pen) ? none : tmpModel.pen;
                tmpModel.fam = string.IsNullOrWhiteSpace(tmpModel.fam) ? none : tmpModel.fam;
                tmpModel.im = string.IsNullOrWhiteSpace(tmpModel.im) ? none : tmpModel.im;
                tmpModel.ot = string.IsNullOrWhiteSpace(tmpModel.ot) ? none : tmpModel.ot;
                tmpModel.dr = string.IsNullOrWhiteSpace(tmpModel.dr) ? none : tmpModel.dr;
                tmpModel.sex = string.IsNullOrWhiteSpace(tmpModel.sex) ? none : tmpModel.sex;
                tmpModel.instype = string.IsNullOrWhiteSpace(tmpModel.instype) ? none : tmpModel.instype;
                tmpModel.inssernum = string.IsNullOrWhiteSpace(tmpModel.inssernum) ? none : tmpModel.inssernum;
                tmpModel.dultype = string.IsNullOrWhiteSpace(tmpModel.dultype) ? none : tmpModel.dultype;
                tmpModel.dulsernum = string.IsNullOrWhiteSpace(tmpModel.dulsernum) ? none : tmpModel.dulsernum;

                using (var db = CreateContext())
                {
                    var insertResult = db.InsertWithIdentity(tmpModel);
                    if (Convert.ToBoolean(insertResult))
                    {
                        result.Id = Convert.ToInt32(insertResult);
                    }
                    else
                    {
                        result.AddError(db.LastError);
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }


        public TransactionResult<IEnumerable<ZP1errors>> GetSrzError(string guid)
        {
            var result = new TransactionResult<IEnumerable<ZP1errors>>();
            try
            {
                using (var db = CreateContext())
                {
                    var queryResult = db.RSP_ZK1msg.Where(p => p.initial_guid.Equals(guid)).ToList();
                    if (queryResult.Any())
                    {
                        foreach (RSP_ZK1msg rspZk1Msg in queryResult)
                        {
                            //LoggingService.DebugFormatted("{0} {1}",rspZk1Msg.id,rspZk1Msg.guid);
                            //TODO ?????
                            RSP_ZK1msg msgCopy = rspZk1Msg;
                            
                            var errorResult = db.ZP1errors.Where(p => p.initial_guid == guid).ToList();
                            if (errorResult.Any())
                            {
                               result.Data = errorResult.Select(p => Map.ObjectToObject<ZP1errors>(p)).ToList();
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception(string.Format("Не найден guid {0}", guid)));
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<IEnumerable<SrzAnswer>> GetSrzAnswer(string guid)
        {
            var result = new TransactionResult<IEnumerable<SrzAnswer>>();
            try
            {
                using (var db = CreateContext())
                {
                    var queryResult = db.RSP_ZK1msg.Where(p => p.initial_guid.Equals(guid)).ToList();
                    if (queryResult.Any())
                    {
                        foreach (RSP_ZK1msg rspZk1Msg in queryResult)
                        {
                            //LoggingService.DebugFormatted("{0} {1}",rspZk1Msg.id,rspZk1Msg.guid);
                            RSP_ZK1msg msgCopy = rspZk1Msg;
                            var qrResult = db.QR.Where(p => p.rsp_zk1id == msgCopy.id).ToList();
                            if (qrResult.Any())
                            {
                                foreach (QR qr in qrResult)
                                {
                                    //LoggingService.DebugFormatted("Код территории:{0} ЕНП:{1} Главный ЕНП:{2}", qr.terr_code, qr.enp, qr.main_enp);
                                    QR qrCopy = qr;
                                    var in1Result = db.IN1.Where(p => p.QRid == qrCopy.id).ToList();
                                    if (in1Result.Count > 0)
                                    {

                                        var resultAnswer = new List<SrzAnswer>(in1Result.Select(p => Map.ObjectToObject<SrzAnswer>(p)));
                                        foreach (var item in resultAnswer)
                                        {
                                            item.main_enp = qrCopy.main_enp;
                                            item.enp = qrCopy.enp;
                                        }
                                        result.Data = resultAnswer;
                                    }
                                }
                            }
                            else
                            {
                                if (rspZk1Msg.ack_code == "CA")
                                {
                                    result.Data = new List<SrzAnswer> { 
                                         new SrzAnswer{
                                              inssernum = "Застрахованный не найден"
                                         } 
                                     };
                                }
                            }
                        }
                    }
                    else
                    {
                        result.Data = new List<SrzAnswer>{ 
                                         new SrzAnswer{
                                              inssernum = "Ответ еще не получен"
                                         } 
                                     };
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZP1msg>> GetSrzQueries(string surname, string name, string patronymic, DateTime? birthday, int? sex, int? docType = null, string docNum = null, string docSer = null, DateTime? formDate = null)
        {
            var result = new TransactionResult<IEnumerable<ZP1msg>>();
            try
            {
                using (var db = CreateContext())
                {
                    Expression<Func<ZP1msg, bool>> predicate = PredicateBuilder.True<ZP1msg>();

                    if (!string.IsNullOrEmpty(surname))
                    {
                        predicate = predicate.And(p => p.fam == surname);
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        predicate = predicate.And(p => p.im == name);
                    }

                    if (!string.IsNullOrEmpty(patronymic))
                    {
                        predicate = predicate.And(p => p.ot == patronymic);
                    }

                    if (birthday.HasValue)
                    {
                        predicate = predicate.And(p => p.dr == birthday.Value.ToString("yyyy-MM-dd"));
                    }

                    if (sex.HasValue)
                    {
                        predicate = predicate.And(p => p.sex == sex.Value.ToString(CultureInfo.InvariantCulture));
                    }

                    if (!string.IsNullOrEmpty(surname))
                    {
                        predicate = predicate.And(p => p.fam == surname);
                    }

                    if (formDate.HasValue)
                    {
                        predicate = predicate.And(p => p.form_date.Value.Year == formDate.Value.Year);
                    }

                    result.Data = db.ZP1msg.Where(predicate).OrderByDescending(p => p.id).ToList();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }
    }

    public class SrzQueryModel
    {
        public int id { get; set; } // int(10)
        public string guid { get; set; } // varchar(50)
        public string pack_guid { get; set; } // varchar(50)
        public string enp { get; set; } // varchar(16)
        public string passp { get; set; } // varchar(20)
        public string pen { get; set; } // varchar(20)
        public string fam { get; set; } // varchar(100)
        public string im { get; set; } // varchar(100)
        public string ot { get; set; } // varchar(100)
        public string dr { get; set; } // varchar(20)
        public string sex { get; set; } // varchar(1)
        public string instype { get; set; } // varchar(5)
        public string inssernum { get; set; } // varchar(20)
        public string dultype { get; set; } // varchar(10)
        public string dulsernum { get; set; } // varchar(100)
        public DateTime? form_date { get; set; } // datetime(3)
    }

    public class SrzAnswer
    {
        public int id { get; set; } // int(10)
        public int? QRid { get; set; } // int(10)
        public string smo_ogrn { get; set; } // varchar(30)
        public string tfoms_ogrn { get; set; } // varchar(30)
        public string date_beg { get; set; } // varchar(30)
        public string date_end { get; set; } // varchar(30)
        public string terr_code { get; set; } // varchar(10)
        public string instype { get; set; } // varchar(5)
        public string inssernum { get; set; } // varchar(100)
        public string enp { get; set; }
        public string main_enp { get; set; }
    }

    public class ZP1errors
    {
        public int id { get; set; } // int(10)
        public int? rsp_zk1_id { get; set; } // int(10)
        public string rsp_zk1_guid { get; set; } // varchar(50)
        public string initial_guid { get; set; } // varchar(50)
        public DateTime? prc_date { get; set; } // datetime(3)
        public string position { get; set; } // varchar(512)
        public string iso_err_code { get; set; } // varchar(20)
        public string iso_err_descr { get; set; } // varchar(8000)
        public string app_err_code { get; set; } // varchar(20)
        public string app_err_descr { get; set; } // varchar(8000)
    }

}
