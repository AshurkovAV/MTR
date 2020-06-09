using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Extensions;
using Medical.AppLayer.Models.EditableModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Services
{
    public class OperatorService : IOperatorService
    {
        private readonly IMedicineRepository _repository;

        public OperatorService(IMedicineRepository repository)
        {
            _repository = repository;

            Init();
        }

        private void Init()
        {
            
        }


        public IEnumerable<RefusalModel> LoadRefusal(int id, RefusalType type, int? source = null)
        {
            var result = new List<RefusalModel>();
           
            switch (type)
            {
                case RefusalType.External:
                    var refusalResult = _repository.GetExternalRefuseByMedicalEventId(id);
                    if (refusalResult.Success && refusalResult.Data.Any())
                    {
                        refusalResult.Data.ForEachAction(p =>
                        {
                            var refusal = RefusalModel.CreateInstance();

                            refusal.Id = p.ExternalRefuseId;
                            refusal.Amount = p.Amount;
                            refusal.Comments = p.Comment;
                            refusal.ReasonId = p.ReasonId;
                            refusal.Flag = RefusalStatusFlag.None;
                            refusal.RefusalType = p.Type.HasValue ? (RefusalType) p.Type.Value : RefusalType.Unknown;
                            refusal.Source = (RefusalSource?)p.Source;
                            refusal.Dest = RefusalDest.In;

                            refusal.Flag = refusal.Flag.AddIfTrue(RefusalStatusFlag.Apply, p.IsAgree == true);
                            refusal.Flag = refusal.Flag.AddIfTrue(RefusalStatusFlag.Dismiss, p.IsAgree == false);

                            result.Add(refusal);
                        });
                    }
                    break;
                case RefusalType.MEC:
                    var mecResult = _repository.GetMecByMedicalEventId(id);
                    if (mecResult.Success && mecResult.Data.Any())
                    {
                        mecResult.Data.ForEachAction(p =>
                        {
                            var refusal = RefusalModel.CreateInstance();

                            refusal.Id = p.MECId;
                            refusal.Amount = p.Amount;
                            refusal.Comments = p.Comments;
                            refusal.ReasonId = p.ReasonId;
                            refusal.Flag = RefusalStatusFlag.None;
                            refusal.RefusalType = RefusalType.MEC;
                            refusal.Source = (RefusalSource?)source;
                            refusal.Dest = RefusalDest.Out;

                            refusal.Flag = refusal.Flag.AddIfTrue(RefusalStatusFlag.Lock, p.IsLock);

                            if (refusal.Source.HasValue)
                            {
                                if (refusal.Source == RefusalSource.Local ||
                                    refusal.Source == RefusalSource.LocalCorrected)
                                {
                                    refusal.Dest = RefusalDest.None;
                                }
                                else
                                {
                                    refusal.Dest = RefusalDest.Out;
                                }
                            }
                            else
                            {
                                refusal.Dest = RefusalDest.Out;
                            }

                            result.Add(refusal);
                        });
                    }
                break;
                case RefusalType.MEE:
                    var meeResult = _repository.GetMeeByMedicalEventId(id);
                    if (meeResult.Success && meeResult.Data.Any())
                    {
                        meeResult.Data.ForEachAction(p =>
                        {
                            var refusal = RefusalModel.CreateInstance();

                            refusal.Id = p.MEEId;
                            refusal.Amount = p.Amount;
                            refusal.Comments = p.Comments;
                            refusal.ReasonId = p.ReasonId;
                            refusal.Flag = RefusalStatusFlag.None;
                            refusal.RefusalType = RefusalType.MEE;
                            refusal.Source = (RefusalSource?)source;
                            refusal.Dest = RefusalDest.Out;

                            if (refusal.Source.HasValue)
                            {
                                if (refusal.Source == RefusalSource.Local ||
                                    refusal.Source == RefusalSource.LocalCorrected)
                                {
                                    refusal.Dest = RefusalDest.None;
                                }
                                else
                                {
                                    refusal.Dest = RefusalDest.Out;
                                }
                            }
                            else
                            {
                                refusal.Dest = RefusalDest.Out;
                            }

                            result.Add(refusal);
                        });
                    }

                break;
                case RefusalType.EQMA:
                    var eqmaResult = _repository.GetEqmaByMedicalEventId(id);
                    if (eqmaResult.Success && eqmaResult.Data.Any())
                    {
                        eqmaResult.Data.ForEachAction(p =>
                        {
                            var refusal = RefusalModel.CreateInstance();

                            refusal.Id = p.EQMAId;
                            refusal.Amount = p.Amount;
                            refusal.Comments = p.Comments;
                            refusal.ReasonId = p.ReasonId;
                            refusal.Flag = RefusalStatusFlag.None;
                            refusal.RefusalType = RefusalType.EQMA;
                            refusal.Source = (RefusalSource?)source;

                            if (refusal.Source.HasValue)
                            {
                                if (refusal.Source == RefusalSource.Local ||
                                    refusal.Source == RefusalSource.LocalCorrected)
                                {
                                    refusal.Dest = RefusalDest.None;
                                }
                                else
                                {
                                    refusal.Dest = RefusalDest.Out;
                                }
                            }
                            else
                            {
                                refusal.Dest = RefusalDest.Out;
                            }
                            

                            result.Add(refusal);
                        });
                    }
                break;
            }
            return result;
        }
    }
}
