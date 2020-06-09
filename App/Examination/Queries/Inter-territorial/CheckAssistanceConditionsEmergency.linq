<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId == 18).Where(p=>
(p.AssistanceConditionsV006.IDUMP == 4 //Стационар
&& (Math.Floor(p.ResultV009.IDRMP.Value / 100.0m) != 4 ||
Math.Floor(p.OutcomeV012.IDIZ.Value / 100.0m) != 4 ||
(p.AssistanceTypeV008.IDVMP != 2 && p.AssistanceTypeV008.IDVMP != 21 && p.AssistanceTypeV008.IDVMP != 21) ||
(p.PaymentMethodV010.IDSP != 15) || 
(p.SpecialityCodeV004.IDMSP != 1119) || 
(p.ProfileCode.IDPR != 84 ) /*May be something else*/))) 
.Select(p=>p.MedicalEventId)