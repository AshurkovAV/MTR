using System.Collections.Generic;
using System.Linq;
using BLToolkit.Mapping;
using Core;
using Core.Extensions;
using DataModel;
using Medical.AppLayer.Models.EditableModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Services
{
    public class ZOperatorService : IZOperatorService
    {
        private readonly IMedicineRepository _repository;

        public ZOperatorService(IMedicineRepository repository)
        {
            _repository = repository;

            Init();
        }

        private void Init()
        {
            
        }


        public IEnumerable<ZRefusalModel> LoadRefusal(int id, RefusalType type, TypeSank typeSank, int? source = null)
        {
            var result = new List<ZRefusalModel>();
           
            switch (type)
            {
                case RefusalType.External:
                    var refusalResult = _repository.GetZExternalRefuseByMedicalEventId(id);
                    if (refusalResult.Success && refusalResult.Data.Any())
                    {
                        refusalResult.Data.ForEachAction(p =>
                        {
                            var refusal = ZRefusalModel.CreateInstance();

                            refusal.ZSankId = p.ExternalRefuseId;
                            refusal.Amount = p.Amount;
                            refusal.Comments = p.Comment;
                            refusal.ReasonId = p.ReasonId;
                            refusal.IsAgree = p.IsAgree;
                            refusal.Flag = RefusalStatusFlag.None;
                            refusal.RefusalType = p.Type.HasValue ? p.Type.Value : (int)RefusalType.Unknown;
                            refusal.Source = (RefusalSource?)p.Source;
                            refusal.Dest = RefusalDest.In;
                            refusal.TypeSank = typeSank;

                            refusal.Flag = refusal.Flag.AddIfTrue(RefusalStatusFlag.Apply, p.IsAgree == true);
                            refusal.Flag = refusal.Flag.AddIfTrue(RefusalStatusFlag.Dismiss, p.IsAgree == false);

                            result.Add(refusal);
                        });
                    }
                    break;
                case RefusalType.MEC:
                    var mecResult = _repository.GetSankByZMedicalEventIdAndType( mec => mec.Type == 1 && mec.ZmedicalEventId == id);
                    if (mecResult.Success && mecResult.Data.Any())
                    {
                        mecResult.Data.ForEachAction(p =>
                        {
                            var refusal = ZRefusalModel.CreateInstance();

                            refusal.ZSankId = p.ZSankId;
                            refusal.Amount = p.Amount;
                            refusal.Comments = p.Comments;

                            refusal.ReasonId = p.ReasonId;
                            refusal.Flag = RefusalStatusFlag.None;
                            refusal.RefusalType = (int)RefusalType.MEC;
                            refusal.Source = (RefusalSource?)source;
                            refusal.Dest = RefusalDest.Out;
                            refusal.TypeSank = typeSank;

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
                    var meeResult = _repository.GetSankByZMedicalEventIdAndType( mee => Constants.Mee.Contains(mee.Type) && mee.ZmedicalEventId == id);
                    if (meeResult.Success && meeResult.Data.Any())
                    {
                        meeResult.Data.ForEachAction(p =>
                        {
                            var refusal = ZRefusalModel.CreateInstance();

                            refusal.ZSankId = p.ZSankId;
                            refusal.Amount = p.Amount;
                            refusal.Comments = p.Comments;
                            refusal.ReasonId = p.ReasonId;
                            refusal.Flag = RefusalStatusFlag.None;
                            refusal.RefusalType = p.Type;
                            refusal.Source = (RefusalSource?)source;
                            refusal.Dest = RefusalDest.Out;
                            refusal.TypeSank = typeSank;

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
                    var eqmaResult = _repository.GetSankByZMedicalEventIdAndType(eqma => Constants.Eqma.Contains(eqma.Type) && eqma.ZmedicalEventId == id);
                    if (eqmaResult.Success && eqmaResult.Data.Any())
                    {
                        eqmaResult.Data.ForEachAction(p =>
                        {
                            var refusal = ZRefusalModel.CreateInstance();

                            refusal.ZSankId = p.ZSankId;
                            refusal.Amount = p.Amount;
                            refusal.Comments = p.Comments;
                            refusal.ReasonId = p.ReasonId;
                            refusal.Flag = RefusalStatusFlag.None;
                            refusal.RefusalType = p.Type;
                            refusal.Source = (RefusalSource?)source;
                            refusal.TypeSank = typeSank;

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

        public IEnumerable<ZSlkoefModel> LoadSlkoef(int id)
        {
            var result = new List<ZSlkoefModel>();

            //var slkoefResult = _repository.GetSlKoefByKsgKpgId(_ksgkpgId);

            //if (slkoefResult.Success && slkoefResult.Data.Any())
            //{
            //    slkoefResult.Data.ForEachAction(p =>
            //    {
            //        var slkoef = ZSlkoefModel.CreateInstance();
            //        SlkoefModel = Map.ObjectToObject<ZSlkoefModel>(p, p);

            //        slkoef.ZslKoefId = p.ZslKoefId;
            //        slkoef.NumberDifficultyTreatment = p.NumberDifficultyTreatment;
            //        slkoef.ValueDifficultyTreatment = p.ValueDifficultyTreatment;
            //        slkoef.ZksgKpgId = p.ZksgKpgId;
            //        result.Add(slkoef);
            //    });
            //}

            return result;
        }
    }
}
