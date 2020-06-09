using System.Collections.Generic;
using System.Linq;
using Core;
using DataModel;
using Medical.DatabaseCore.Services.Database;
using System.ComponentModel.DataAnnotations;

namespace Medical.AppLayer.Linq
{
    public class PLinqActsList : PLinqListBase<ActModel>
    {
        private int? _accountId;
        private int _scope;
        public PLinqActsList(IMedicineRepository repository, int scope, int? accountId = null)
            : base(repository, null)
        {
            _accountId = accountId;
            _scope = scope;
        }

        public override void GenerateData()
        {
            Data = new List<ActModel>();
            if (_accountId.HasValue)
            {
                var resultMee = Repository.GetMeeActByAccountIdAndScope(_accountId.Value, _scope);
                if (resultMee.Success)
                {
                    Data.AddRange(resultMee.Data.Select(p => new ActModel
                    {
                        Id = p.ActMeeId,
                        AccountId = p.AccountId ?? p.AccountMoId,
                        Type = RefusalType.MEE,
                        AcceptPrice = p.AcceptPrice,
                        PenaltyPrice = p.PenaltyPrice,
                        Price = p.Price,
                        RefusalPrice = p.RefusalPrice
                    }));
                }

                var resultEqma = Repository.GetEqmaActByAccountIdAndScope(_accountId.Value, _scope);
                if (resultEqma.Success)
                {
                    Data.AddRange(resultEqma.Data.Select(p => new ActModel
                    {
                        Id = p.ActEqma,
                        AccountId = p.AccountId ?? p.AccountMoId,
                        Type = RefusalType.EQMA,
                        AcceptPrice = p.AcceptPrice,
                        PenaltyPrice = p.PenaltyPrice,
                        Price = p.Price,
                        RefusalPrice = p.RefusalPrice
                    }));
                }
            }
            else
            {
                var resultMee = Repository.GetMeeActByScope(_scope);
                if (resultMee.Success)
                {
                    Data.AddRange(resultMee.Data.Select(p => new ActModel
                    {
                        Id = p.ActMeeId,
                        AccountId = p.AccountId ?? p.AccountMoId,
                        Type = RefusalType.MEE,
                        AcceptPrice = p.AcceptPrice,
                        PenaltyPrice = p.PenaltyPrice,
                        Price = p.Price,
                        RefusalPrice = p.RefusalPrice
                    }));
                }

                var resultEqma = Repository.GetEqmaActByScope(_scope);
                if (resultEqma.Success)
                {
                    Data.AddRange(resultEqma.Data.Select(p => new ActModel
                    {
                        Id = p.ActEqma,
                        AccountId = p.AccountId ?? p.AccountMoId,
                        Type = RefusalType.EQMA,
                        AcceptPrice = p.AcceptPrice,
                        PenaltyPrice = p.PenaltyPrice,
                        Price = p.Price,
                        RefusalPrice = p.RefusalPrice
                    }));
                }
            }
            
        }
    }

    public class ActModel
    {
        [Display(Name = "ID акта")]
        public int Id { get; set; }

        [Display(Name = "ID Счета")]
        public int? AccountId { get; set; }
        [Display(Name = "Тип отказа")]
        public RefusalType Type { get; set; }
        [Display(Name = "Сумма выставленная к оплате"), DataType(DataType.Currency)]
        public decimal? Price { get; set; }
        [Display(Name = "Сумма принятая к оплате"), DataType(DataType.Currency)]
        public decimal? AcceptPrice { get; set; }
        [Display(Name = "Сумма отказа"), DataType(DataType.Currency)]
        public decimal? RefusalPrice { get; set; }
        [Display(Name = "Штрафные санкции"), DataType(DataType.Currency)]
        public decimal? PenaltyPrice { get; set; }
        

    }
}
