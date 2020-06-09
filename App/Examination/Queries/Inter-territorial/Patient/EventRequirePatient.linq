<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId == 1 && p.FactPatient.PatientId == 4).Where(p=>
p.AssistanceConditions == null ||
p.AssistanceType == null ||
p.ProfileCodeId == null ||
p.IsChildren == null ||
(p.History == null || p.History == string.Empty) ||
p.EventBegin == null ||
p.EventEnd == null ||
p.DiagnosisGeneral == null ||
p.Result == null ||
p.Outcome == null ||
p.SpecialityCode == null ||
p.PaymentStatus == null ||
(p.Price == null||p.Price == 0)).Select(p=>p.PatientId)